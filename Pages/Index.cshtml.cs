using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace commPinPad.Pages
{
    public class IndexModel : PageModel
    {
        public static bool _continue;
        public static SerialPort serialPort1 = new SerialPort();
        public string respuesta = "";
        public List<SelectListItem>? portList { get; set; }
        RijndaelManaged rm;

        //public string saleAmount = "120.00";


        CultureInfo cultura = new CultureInfo("es-MX");


        public string puertos = "";

        public string txtLog { get; set; }
        public string txtRecibido { get; set; }

        public string MessageBox = " ";


        public string txtSendHexa = " ";

        public bool hasData = false;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            getDate();


            String[] puertos = SerialPort.GetPortNames();


            portList = new List<SelectListItem> { };
            foreach (string Buffer in puertos)
            {

                portList.Add(
                    new SelectListItem { Text = Buffer, Value = Buffer }
                );
            }
        }

        public void OnPost()
        {
            hasData = true;

            String[] puertos = SerialPort.GetPortNames();

            portList = new List<SelectListItem> { };
            foreach (string Buffer in puertos)
            {

                portList.Add(
                    new SelectListItem { Text = Buffer, Value = Buffer }
                );
            }

        }

        public async Task OnPostAbrirPuerto()
        {
            Console.WriteLine("OnPostAbrirPuerto");
            txtRecibido = "OnPostAbrirPuerto";
            abrirPuerto();
        }

        public async Task OnPostCerrarPuerto()
        {
            Console.WriteLine("OnPostCerrarPuerto");
            txtRecibido = "OnPostCerrarPuerto";
            cerrarPuerto();
        }

        public async Task OnPostEnviarTexto()
        {
            Console.WriteLine("OnPostEnviarTexto");
            abrirPuerto();
            string texto = Request.Form["txttexto"];
            enviarTexto(texto);
            cerrarPuerto();
        }

        public async Task OnPostCancelarCompra()
        {
            Console.WriteLine("OnPostSolicitarPago");

            string monto = Request.Form["txtMonto"];
            string tipo = Request.Form["txtTipo"];
            string authId = Request.Form["txtCodAuth"];
            string stan = Request.Form["txtStan"];
            string refNum = Request.Form["txtRefNum"];
            string entryMode = Request.Form["txtEntryMode"];
            string tokenES = Request.Form["txtTokenES"];
            string tokenEZ = Request.Form["txtTokenEZ"];

            solicitarCancelar( monto,  stan,  refNum,  tipo,  authId,  entryMode, tokenES, tokenEZ);
        }

        public async Task OnPostPrueba()
        {
            Console.WriteLine("Prueba");
            string factMessage = "";
            int first = 0;
            string str1 = "";
            int longEX = 0;
            string str2 = "";
            //factMessage = "! ER00002 bandera actualizacion llavez! EX00013 llave cifrada";
            //first = factMessage.IndexOf("! EX");

            //Console.WriteLine(first);
            //str1 = factMessage.Substring((first + 4), 5);
            //Console.WriteLine(str1);
            //longEX = Int32.Parse(str1);
            //Console.WriteLine(longEX);
            //str2 = factMessage.Substring((first + 10), longEX);
            //Console.WriteLine(str2);

            factMessage = "33 32 69 80 0 0 0 0 2 32 b a n d e r a _ a c t u a l i z a c i o n _ l l a v e z 33 32 69 88 48 48 48 49 51 32 l l a v e _ c i f r a d a";
            //first = factMessage.IndexOf("33 32 69 88 48 48 48 54 56 32");

            String[] respList = factMessage.Split(' ');

            int i = 0;
            int j = 5;

            while (i < respList.Length)
            {
                if (respList[i] == "33" && respList[i + 1] == "32" && respList[i + 2] == "69" && respList[i + 3] == "88")
                {
                    first = i;
                    i += 4;
                    break;
                }
                i++;
            }
            while (j > 0) { str1 += (char)(Int32.Parse(respList[i])); j--; i++; }
            Console.WriteLine(str1);
            longEX = Int32.Parse(str1);
            j = longEX;
            i++;
            while (j > 0) { str2 += respList[i]; j--; i++; }

            Console.WriteLine(str2);

        }

        public async Task OnPostSolicitarPago()
        {
            Console.WriteLine("OnPostSolicitarPago");
            string monto = Request.Form["txtMonto"];
            string transId = Request.Form["txtTransId"];
            string stan = Request.Form["txtStan"];
            string refNum = Request.Form["txtRefNum"];
            string tipo = Request.Form["txtTipo"];
            string dir = Request.Form["txtDir"];
            string subAfId = Request.Form["txtSubAfiliadoId"];
            


            abrirPuerto();
            //string texto = Request.Form["txttexto"];
            solicitarPago(transId, monto, stan, refNum, tipo, dir, subAfId);
            cerrarPuerto();
        }

        public async Task OnPostGeneraLlave()
        {
            //string datosRespCZ10 = "";
            string tokenEX = "";
            string monto = Request.Form["txtMonto"];
            string transId = Request.Form["txtTransIdKeyGen"];
            string stan = Request.Form["txtStanKeyGen"];
            string refNum = Request.Form["txtRefNumKeyGen"];
            string mti = "0200";
            string date = getDate();
            string time = getTime();

            abrirPuerto();
            Console.WriteLine("OnPostGeneraLlave");
            txtRecibido = "OnPostGeneraLlave";
            String txtHexa = comando_Z10_solicitarKeyGen();
            enviarCodigos(txtHexa);
            Console.WriteLine("Genera y manda codigo Z10, generar llave");
            if (respuesta.Replace(" ", "") == "6") { _continue = true; Read(); }
            string datosRespCZ10 = postToHostNewKey(respuesta, transId, monto, stan, refNum, mti, date, time);
            Console.WriteLine(datosRespCZ10);
            tokenEX = getDatosEX(datosRespCZ10);
            txtHexa = comando_Z11_insertarKeyGen(tokenEX);
            enviarCodigos(txtHexa);
            if (respuesta.Replace(" ", "") == "6") { _continue = true; Read(); }
            Console.WriteLine(respuesta);
            cerrarPuerto();
        }

        public async Task OnPostRespC53()
        {
            string resultCode210 = "?�ISO0240000770210323A84000E818012000000000000048061071119010200130113005307110711071105100000001301000202P0000000001        0274139942            00000000484000070& 0000500070! Q100002 04! Q200002 04! C400012 000000001032! ER00002 00";

            Console.WriteLine("readCode210");
            var respuesta = readCode210(resultCode210);
            
            string authId = respuesta.Item1;
            string respCode = respuesta.Item2;
            string AuthData = respuesta.Item3;
            Console.WriteLine(respuesta.Item1);
            Console.WriteLine(respuesta.Item2);
            Console.WriteLine(respuesta.Item3);
            comando_C54(authId, respCode, AuthData);
        }

        protected void abrirPuerto()
        {
            try
            {
                serialPort1 = new SerialPort();
                serialPort1.PortName = "/dev/tty.usbmodem1101";
                serialPort1.BaudRate = 9600;
                serialPort1.DataBits = 8;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;

            }
            catch { Console.WriteLine("Port error, Please check the serial port"); }

            try
            {
                serialPort1.Close();
                serialPort1.Open();

                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                _continue = true;

                String txtHexa = "";
                txtHexa = comando_72();
                enviarCodigos(txtHexa);
                Console.WriteLine(respuesta);
            }
            catch { Console.WriteLine("Port error, Please check the serial port"); }

            txtLog = "puerto abierto\r\n";
            Console.WriteLine("Puerto Abierto");
            //return "puerto abierto\r\n";
        }

        public void cerrarPuerto()
        {
            _continue = false;
            try
            {
                String txtHexa = "";
                txtHexa = comando_72();
                enviarCodigos(txtHexa);
                Console.WriteLine(respuesta);

                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Close();

            }
            catch { Console.WriteLine("Error Cerrando Puerto"); }
            Console.WriteLine("Puerto Cerrado");
            //return "Puerto Cerrado\r\n";
        }

        public void enviarTexto(string texto)
        {
            String txtHexa = comando_textScreen(texto);
            enviarCodigos(txtHexa);
            Console.WriteLine("Texto enviado");
            //return "Texto enviado";
        }

        protected string comando_Z10_solicitarKeyGen()
        {
            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.
            string com_Z10 = "Z10";    // Solicitud de llave aleatoria
            string hexCom_Z10 = "";
            string salida1 = "";
            string hexString = "";

            byte[] bytes = Encoding.Default.GetBytes(com_Z10);
            hexCom_Z10 = BitConverter.ToString(bytes);
            hexCom_Z10 = hexCom_Z10.Replace("-", " ");

            hexString = hexCom_Z10 + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            return salida1;
        }

        protected string comando_Z11_insertarKeyGen(string tokenEX)
        {
            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.
            string com_Z11 = "Z11";    // Solicitud insertar llave 
            string hexCom_Z11 = "";
            string eyeCatcher = "!";
            string tipo = "EX";
            string hexEyeCatcher = "21";
            string espacio = " ";
            string hexEspacio = "20";
            string longEx = "00068";
            string hexLongEx = "";
            string c1TokenEx = "";
            string c2TokenEx = "";
            string c3TokenEx = "";
            string c4TokenEx = "";
            string c5TokenEx = "";
            string hexTokenEx = "";
            string salida1 = "";
            string hexString = "";

            byte[] bytes = Encoding.Default.GetBytes(com_Z11);
            hexCom_Z11 = BitConverter.ToString(bytes);
            hexCom_Z11 = hexCom_Z11.Replace("-", " ");

            // hexTokenEx = tokenEX;

            
            bytes = Encoding.Default.GetBytes(tokenEX);
            hexTokenEx = BitConverter.ToString(bytes);
            hexTokenEx = hexTokenEx.Replace("-", " ");
            

            //hexTokenEx = hexEyeCatcher + " " + hexEspacio + " " + hexLongEx + " " + hexEspacio + " ";
            //hexTokenEx += c1TokenEx + " " + c2TokenEx + " " + c3TokenEx + " " + c4TokenEx + " " + c5TokenEx;

            hexString = hexCom_Z11 + " " + hexTokenEx + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            return salida1;
        }

        protected void solicitarPago(string transId, string monto, string stan, string refNum, string tipo,string dir, string subAfId)
        {
            String txtHexa = "";
            string date = "";
            string time = "";
            txtHexa = comando_C50(monto);
            enviarCodigos(txtHexa);
            Console.WriteLine(respuesta);
            if (respuesta.Replace(" ", "") == "6") { _continue = true; Read(); }
            Console.WriteLine(respuesta);
            if (respuesta.Replace(" ", "") == "4") { return; }

            if (respuesta.Length < 11) { MessageBox += respuesta; return; }
            string validador = respuesta.Replace(" ", "").Substring(0, 11);
            if (validador == "26753484848")
            {
                txtLog += "Recibe respuesta Comando C50 positivo\n";
                Console.WriteLine("Recibe respuesta Comando C50 positivo");
                _continue = true;
                date = getDate();
                time = getTime();
                txtHexa = comando_C51(monto, date, time);
                enviarCodigos(txtHexa);
                //Console.WriteLine(respuesta);
                //string miniResp = respuesta.Replace(" ", "");
                //while(miniResp == "6" || miniResp == "4") {  _continue = true; Read(); Console.WriteLine(respuesta); miniResp = respuesta.Replace(" ", ""); }
                if (respuesta.Replace(" ", "") == "6") { _continue = true; Read(); }
                Console.WriteLine(respuesta);
                if (respuesta.Replace(" ", "") == "4") { return; }
            }
            Console.WriteLine(respuesta);
            
            if (respuesta.Length < 11) { MessageBox += respuesta; return; }
            validador = respuesta.Replace(" ", "").Substring(0, 11);
            if (validador == "26753514848" || validador == "26753515050") // respuesta C53
            {
                if (validador == "26753515050")
                {
                    txtLog += "Recibe respuesta Comando C53 Contacless\n";
                    Console.WriteLine("Recibe respuesta Comando C53 Contacless");
                }
                else { txtLog += "Recibe respuesta Comando C53 Operación exitosa\n"; Console.WriteLine("Recibe respuesta Comando C53 Operación exitosa"); }

                XtrPost postCompra = getValuesRespC53(respuesta);

                string mti = "0200";
                postCompra.Add("mti", mti);
                postCompra.Add("localDate", date);
                postCompra.Add("localTime", time);
                postCompra.Add("monto", monto);
                postCompra.Add("transactionID", transId);
                postCompra.Add("stan", stan);
                postCompra.Add("refNumber", refNum);
                postCompra.Add("cardType", tipo);
                postCompra.Add("dir", dir);
                postCompra.Add("subAfId", subAfId);

                Console.WriteLine("**Monto: "+ monto);
                Console.WriteLine("**Tipo tarejta: " + tipo);
                Console.WriteLine("**Stan: " + stan);
                Console.WriteLine("**RefNumber: " + refNum);

                string datosRespC53 = "";
                datosRespC53 = postToHost(postCompra);

                /*  ----Cerrar Pinpad en pruebas------  */

                txtHexa = comando_72();
                enviarCodigos(txtHexa);
                
                return;

                /*  -----                       -----  */

                //Console.WriteLine(datosRespC53);
                _continue = true;
                try
                {
                    var resCode210 = readCode210(datosRespC53);
                    string authId = resCode210.Item1;
                    Console.WriteLine("**autorizacionId: " + authId);
                    string respCode = resCode210.Item2;
                    Console.WriteLine("**respCode: "+respCode);
                    string AuthData = resCode210.Item3;
                    Console.WriteLine("**AuthData: " + AuthData);
                    txtHexa = comando_C54(authId, respCode, AuthData);
                }
                catch (Exception e) { Console.WriteLine(e); txtHexa = comando_72(); }
                

                
                //txtHexa = comando_C54();
                enviarCodigos(txtHexa);
                //Console.WriteLine(respuesta);
                if (respuesta.Replace(" ", "") == "6") { _continue = true; Read(); }
                Console.WriteLine(respuesta);
                if (respuesta.Replace(" ", "") == "4") { return; }
                Console.WriteLine(respuesta);
            }
            if (validador == "26753524848" || validador == "26753525050")  // respuesta C54 
            {
                if (validador == "26753515050") { txtLog += "Recibe respuesta Comando C54 Contacless\n"; Console.WriteLine("Recibe respuesta Comando C54 Contacless"); }
                else { txtLog += "Recibe respuesta Comando C54 Operación exitosa\n"; Console.WriteLine("Recibe respuesta Comando C54 Operación exitosa"); }

                // Completado fuera de linea, no requiere enviar nada al host

            }
        }

        protected void solicitarCancelar( string monto, string stan, string refNum, string tipo, string authId, string entryMode, string tokenES, string tokenEZ)
        {
            string mti = "0420";
            string date = getDate();
            string time = getTime();

            XtrPost postCancelar = new XtrPost();

            postCancelar.Add("authId", authId);
            postCancelar.Add("mti", mti);
            postCancelar.Add("monto", monto);
            postCancelar.Add("entryMode", entryMode);
            postCancelar.Add("stan", stan);
            postCancelar.Add("refNumber", refNum);
            postCancelar.Add("localDate", date);
            postCancelar.Add("localTime", time);
            postCancelar.Add("cardType", tipo);
            postCancelar.Add("tokenES", tokenES);
            postCancelar.Add("tokenEZ", tokenEZ);


            string datosRespCancel = "";
            
            datosRespCancel = postToHostCancel(postCancelar);

        }

        protected string comando_72()
        {
            string salida1 = "";
            string hexString = "";


            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.


            string com_72 = "72";    // Este mensaje es utilizado para que el punto de venta 
                                     // recupere y almacene el PAN(número de tarjeta) e inicie el
                                     // proceso financiero
            string hexCom_72 = "";



            byte[] bytes;

            bytes = Encoding.Default.GetBytes(com_72);
            hexCom_72 = BitConverter.ToString(bytes);
            hexCom_72 = hexCom_72.Replace("-", " ");



            hexString = hexCom_72 + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            txtLog += "Genera y Envia Comando 72\n";
            Console.WriteLine("Genera y Envia Comando 72");

            return salida1;
        }

        protected string comando_C50(string monto)
        {
            string salida1 = "";
            string hexString = "";
            string parametros = "";

            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.

            string longitud = "";       // tamanio en bytes a hexadecimal de los parametros
            string com_C50 = "C50";    // Este mensaje es utilizado para que el punto de venta 
                                       // recupere y almacene el PAN(número de tarjeta) e inicie el
                                       // proceso financiero
            string hexCom_C50 = "";
            string hexCom_C1 = "C1";         //  Enmascaramiento de la cuenta.
            string com_TimeOut = "30";       //  Time Out ECR, valor recomendado 30 seg en BCD
            string hexCom_TimeOut = "";
            string com_TransDate = getDate();  // "23 06 22";  // Representa cualquier fecha válida (22 de jun de 2023)
                                               // a 6 dígitos en BCD en el formato YYMMDD
            string hexCom_TransDate = "";
            string com_TransTime = getTime();   // "18 18 00";  // Representa cualquier hora válida a 6
                                                // Digitos en BCD en el formato HHMMSS  (4:27:00)
            string hexCom_TransTime = "";
            string com_TxnType = "87";//string com_TxnType = "B7";      //  Tipo de Transaccion  //  **  PENDIENTE REVISAR  **  //
            string hexCom_TxnType = "";
            string com_SaleAmount = monto;  // "120.34";   //  Monto de venta  (12 pesos 34 centavos) 000004D2
            string com_PAN_BIN = "00";      //  00 PAN Completo
                                            //  01 BIN tarjeta a 6 posiciones
                                            //  02 BIN tarjeta a 8 posiciones
            string hexCom_PAN_BIN = "";


            byte[] bytes;

            bytes = Encoding.Default.GetBytes(com_C50);
            hexCom_C50 = BitConverter.ToString(bytes);
            hexCom_C50 = hexCom_C50.Replace("-", " ");

            hexCom_TimeOut = hexCom_C1 + " 01 " + com_TimeOut;

            hexCom_TransDate = hexCom_C1 + " 03 " + com_TransDate;

            hexCom_TransTime = hexCom_C1 + " 03 " + com_TransTime;

            hexCom_TxnType = hexCom_C1 + " 01 " + com_TxnType;

            com_SaleAmount = Decimal.Parse(com_SaleAmount, cultura).ToString("0.00").Replace(",", ".");

            com_SaleAmount = com_SaleAmount.Replace(".", "");
            int tmp_com_SaleAmount = Int32.Parse(com_SaleAmount);
            string hexAmount = tmp_com_SaleAmount.ToString("X");
            int longHex = hexAmount.Length;
            while (longHex < 8)
            {
                hexAmount = "0" + hexAmount;
                longHex++;
            }

            hexAmount = hexCom_C1 + " 04 " + hexAmount;

            hexCom_PAN_BIN = hexCom_C1 + " 01 " + com_PAN_BIN;

            parametros = hexCom_TimeOut + " " + hexCom_TransDate + " " + hexCom_TransTime + " " + hexCom_TxnType + " " + hexAmount + " " + hexCom_PAN_BIN;

            longitud = longitudParametros(parametros);

            hexString = hexCom_C50 + " " + longitud + " " + parametros + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            txtLog += "Genera y Envia Comando C50\n";
            Console.WriteLine("Genera y Envia Comando C50");

            return salida1;
        }

        string comando_C51(string monto, string date, string time)
        {
            string salida1 = "";
            string hexString = "";
            string parametros = "";

            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.
            string longitud = "";       // tamanio en bytes a hexadecimal de los parametros
            string com_C51 = "C51";    // Este mensaje es utilizado para que el punto de venta 
                                       // recupere y almacene el PAN(número de tarjeta) e inicie el
                                       // proceso financiero
            string hexCom_C51 = "";
            string hexCom_C1 = "C1";    //  Enmascaramiento de la cuenta.
            string hexCom_E1 = "E1";    //  
            string com_TimeOut = "30";  // Time Out ECR, valor recomendado 30 seg en BCD
            string hexCom_TimeOut = "";
            string com_TransDate = date;   // "23 06 22";  // Representa cualquier fecha válida (22 de jun de 2023)
                                           // a 6 dígitos en BCD en el formato YYMMDD
            string hexCom_TransDate = "";
            string com_TransTime = time;   // "18 18 10";  // Representa cualquier hora válida a 6
                                           // Digitos en BCD en el formato HHMMSS  (4:27:00)
            string hexCom_TransTime = "";
            string com_TxnType = "87";//string com_TxnType = "B7";      //  Tipo de Transaccion  //  **  PENDIENTE REVISAR  **  //
            string hexCom_TxnType = "";
            string com_SaleAmount = monto;  // "120.34";   //  Monto de venta  (12 pesos 34 centavos) 000004D2
            string com_otherAmount = "00 00 00 00";    //  Siempre dejar en 00 00 00 00
            string hexCom_otherAmount = "";
            string com_currCode = "04 84";   //  Representa el código de moneda de cada país(04 84 México)
            string hexCom_currCode = "";
            string com_merchantDecision = "01";   //  01 (Forzar en línea)
                                                  //  00 Transacciones fuera de línea
            string hexCom_merchantDecision = "";
            string com_listaDeObjetos = "5F 2A 82 84 95 9A 9C 9F 02 9F 03 9F 09 9F 10 9F 1A 9F 1E 9F 26 9F 27 9F 33 9F 34 9F 35 9F 36 9F 37 9F 41 9F 53 9F 6E";   // Lista de objetos mínimos requeridos
            string hexCom_listaDeObjetos = "";
            string com_PAN = "00";       //  00 PAN Completo
                                         //  01 PAN Enmascarado 
            string hexCom_PAN = "";

            byte[] bytes;

            bytes = Encoding.Default.GetBytes(com_C51);
            hexCom_C51 = BitConverter.ToString(bytes);
            hexCom_C51 = hexCom_C51.Replace("-", " ");

            hexCom_TimeOut = hexCom_C1 + " 01 " + com_TimeOut;

            hexCom_TransDate = hexCom_C1 + " 03 " + com_TransDate;

            hexCom_TransTime = hexCom_C1 + " 03 " + com_TransTime;

            hexCom_TxnType = hexCom_C1 + " 01 " + com_TxnType;

            com_SaleAmount = Decimal.Parse(com_SaleAmount, cultura).ToString("0.00").Replace(",", ".");
            com_SaleAmount = com_SaleAmount.Replace(".", "");
            int tmp_com_SaleAmount = Int32.Parse(com_SaleAmount);
            string hexAmount = tmp_com_SaleAmount.ToString("X");
            int longHex = hexAmount.Length;
            while (longHex < 8)
            {
                hexAmount = "0" + hexAmount;
                longHex++;
            }

            hexAmount = hexCom_C1 + " 04 " + hexAmount;

            hexCom_otherAmount = hexCom_C1 + " 04 " + com_otherAmount;

            hexCom_currCode = hexCom_C1 + " 02 " + com_currCode;

            hexCom_merchantDecision = hexCom_C1 + " 01 " + com_merchantDecision;

            hexCom_listaDeObjetos = hexCom_E1 + " 27 " + com_listaDeObjetos;

            hexCom_PAN = hexCom_C1 + " 01 " + com_PAN;

            parametros = hexCom_TimeOut + " " + hexCom_TransDate + " " + hexCom_TransTime + " " + hexCom_TxnType + " " + hexAmount + " " + hexCom_otherAmount + " " + hexCom_currCode + " " + hexCom_merchantDecision + " " + hexCom_listaDeObjetos + " " + hexCom_PAN;

            longitud = longitudParametros(parametros);

            hexString = hexCom_C51 + " " + longitud + " " + parametros + " " + ETX;
            //Console.WriteLine(hexString);
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            txtLog += "Genera y Envia Comando C51\n";
            Console.WriteLine("Genera y Envia Comando C51");
            Console.WriteLine(salida1);

            return salida1;
        }

        string comando_C54(string authId, string respCode, string AuthData)   //  Varios de los datos que se mandan en C54 deben ser recuperados del Host
        {
            string salida1 = "";
            string hexString = "";
            string parametros = "";

            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.
            string longitud = "";       // tamanio en bytes a hexadecimal de los parametros
            string com_C54 = "C54";    //  Tag Manda respuesta del host al pin pad          
            string hexCom_C54 = "";
            string hexCom_C1 = "C1";    //  Enmascaramiento de la cuenta.
            string hexCom_91 = "91";    //  Tag
            string hexCom_E2 = "E2";    //  Tag
            string comRespHost = "00";   // 00 = Respuesta Exitosa
                                         // 01 = Rechazo del Host
                                         // 02 = No hubo respuesta delHost
                                         // 03 = Abortar transacción-sincronizar pinpad
                                         // Otro = Fallido
            string hexComRespHost = "";
            string com_AuthorizationCode = authId;  // "2CA025";  // 32 43 41 30 32 35
                                                      // Para el número de aurtorización 2CA025 (Campo 38 ISO)
                                                      // En transacción referida (venta forzada),
                                                      // debe enviarse la autorización solicitada vía voz.
            string hexCom_AuthorizationCode = "";

            string com_ResponseCode = respCode;  // "00";        // 00= Aprobada
                                                   // Otro = No autorizada
                                                   // (Campo 39 ISO, Vea Anexo A ISO)
            string hexCom_ResponseCode = "";
            string com_AuthenticationData = AuthData; // "E3 59 4B AA 75 C0 6D FE 30 30";    // Datos variables 
                                                                                // Dato opcional, solo sí existe
                                                                                // en la respuesta del Emisor. (Tag 91 del Campo 55 ISO)
                                                                                // Aplica solo para tarjeta Chip y Contactless.
                                                                                // Para tarjeta con Banda Magnética y Digitada viajará
                                                                                // vacío, longitud cero(91 00) 
                                                                                // El caso de la modalidad de Contacless CHIP y MSD
                                                                                // de no ser informado este viaja con longitud CERO, o
                                                                                // contenido vacío(91 00), ya que no existe resultado del
                                                                                // segundo Generate AC a enlistar.
            string hexCom_AuthenticationData = "";
            string com_TransDate = getDate();   // "23 06 22";  // Representa cualquier fecha válida (1 de ago de 2022)
                                                // a 6 dígitos en BCD en el formato YYMMDD
            string hexCom_TransDate = "";
            string com_TransTime = getTime();   // "18 27 30";  // Representa cualquier hora válida a 6
                                                // Digitos en BCD en el formato HHMMSS  (4:27:00)
            string hexCom_TransTime = "";
            string com_ListTagObjetos = "";          // Para el caso de la modalidad 
                                                     // de Contacless CHIP y MSD, este campo viaja con longitud CERO,
                                                     // o contenido vacío(E2 00).No se requiere enlistar datos EMV por
                                                     // que no se genera el Cryptograma del segundo Generate AC.
            string hexCom_ListTagObjetos = "";

            byte[] bytes;

            bytes = Encoding.Default.GetBytes(com_C54);
            hexCom_C54 = BitConverter.ToString(bytes);
            hexCom_C54 = hexCom_C54.Replace("-", " ");

            hexComRespHost = hexCom_C1 + " 01 " + comRespHost;

            bytes = Encoding.Default.GetBytes(com_AuthorizationCode);
            hexCom_AuthorizationCode = BitConverter.ToString(bytes);
            hexCom_AuthorizationCode = hexCom_AuthorizationCode.Replace("-", " ");

            hexCom_AuthorizationCode = hexCom_C1 + " 06 " + hexCom_AuthorizationCode;

            bytes = Encoding.Default.GetBytes(com_ResponseCode);
            hexCom_ResponseCode = BitConverter.ToString(bytes);
            hexCom_ResponseCode = hexCom_ResponseCode.Replace("-", " ");

            bytes = Encoding.Default.GetBytes(com_AuthenticationData);
            hexCom_AuthenticationData = BitConverter.ToString(bytes);
            hexCom_AuthenticationData = hexCom_AuthenticationData.Replace("-", " ");
            var longAuthData = com_AuthenticationData.Length;

            hexCom_ResponseCode = hexCom_C1 + " 02 " + hexCom_ResponseCode;

            hexCom_AuthenticationData = hexCom_91 + " 00 " + com_AuthenticationData;  // (el tamaño debe calcularse, no fijo)

            hexCom_TransDate = hexCom_C1 + " 03 " + com_TransDate;

            hexCom_TransTime = hexCom_C1 + " 03 " + com_TransTime;

            hexCom_ListTagObjetos = hexCom_E2 + " 00 " + com_ListTagObjetos; // (el tamaño debe calcularse, no fijo)


            parametros = hexComRespHost + " " + hexCom_AuthorizationCode + " " + hexCom_ResponseCode + " " + hexCom_AuthenticationData + " " + hexCom_TransDate + " " + hexCom_TransTime + " " + hexCom_ListTagObjetos;

            longitud = longitudParametros(parametros);

            hexString = hexCom_C54 + " " + longitud + " " + parametros + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            txtLog += "Genera y Envia Comando C54\n";
            Console.WriteLine("Genera y Envia Comando C54");

            return salida1;
        }

        string longitudParametros(string parametros)
        {
            parametros = parametros.Replace(" ", "");
            int longBytes = parametros.Length / 2;
            string longHex = longBytes.ToString("X");
            int i = longHex.Length;

            while (i < 4)
            {
                longHex = "0" + longHex;
                i++;
            }
            return longHex;
        }

        static string comando_textScreen(string texto)
        {
            string STX = "02";  // Start of Text. Indica el inicio de un mensaje de datos.
            string ETX = "03";  // End of Text. Indica el fin de un mensaje de datos.
            string LRC = "";    // Longitudinal Redundancy Check Character.
            string com_Z2 = "Z2";   // Este mensaje es utilizado por el punto de venta para solicitar el despliegue de un mensaje en Pinpad.
            string hexCom_z2 = "";
            string hexCom_1A = "1A";   // Limpiar display
            string salida1 = "";
            string salida2 = "";

            string decString = texto; // "Tay --  >;)";

            byte[] bytes = Encoding.Default.GetBytes(decString);
            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", " ");

            bytes = Encoding.Default.GetBytes(com_Z2);
            hexCom_z2 = BitConverter.ToString(bytes);
            hexCom_z2 = hexCom_z2.Replace("-", " ");

            hexString = hexCom_z2 + " " + hexCom_1A + " " + hexString + " " + ETX;
            LRC = strAddXOR(hexString);
            salida1 = STX + " " + hexString + " " + LRC;

            hexCom_z2 = string.Join("", com_Z2.Select(c => String.Format("{0:X2}", Convert.ToInt32(c))));
            var hexString2 = string.Join("", decString.Select(c => String.Format("{0:X2}", Convert.ToInt32(c))));
            hexString2 = hexCom_z2 + hexCom_1A + hexString2 + ETX;
            LRC = strAddXOR(hexString2);
            salida2 = STX + hexString2 + LRC;

            return salida1;
        }

        public string getDate()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yy MM dd");
        }

        public string getTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("HH mm ss");
        }

        void enviarCodigos(string txtHexa)
        {
            txtSendHexa += txtHexa + "\n\n";
            txtHexa = txtHexa.Replace(" ", "");
            int txtHexaSize = (txtHexa.Length) / 2;
            byte[] dataByte = new byte[txtHexaSize];
            List<byte> tempList = new List<byte>();

            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardOutBuffer();

                for (int j = 0; j < txtHexa.Length; j++)
                {
                    if (txtHexa[j] == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            tempList.Add(Convert.ToByte(txtHexa.Substring(j, 2), 16));
                            j++;
                        }
                        catch
                        {
                            Console.WriteLine("Please check the hexadecimal data format error");
                        }
                    }
                }
                dataByte = tempList.ToArray();
                try
                {
                    serialPort1.Write(dataByte, 0, txtHexaSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Port sending failed. The system will close the current serial port error.\n" + ex);
                    serialPort1.Close();
                }
                if (txtHexaSize > 2) { _continue = true; Read(); }


            }
            else { Console.WriteLine("El puerto no esta abierto"); }

        }

        public void Read()
        {
            int i = 0;
            txtRecibido += "BytesToRead: " + serialPort1.BytesToRead + "\n";
            int tmpInt = 0;
            byte[] cache;
            int offset = 0;
            int bytesToRead = 0;

            respuesta = "";

            while (_continue)
            {
                try
                {
                    bytesToRead = (int)serialPort1.BytesToRead;
                    Console.WriteLine("bytesToRead: " + bytesToRead);
                    if (bytesToRead > 0 && _continue)
                    {
                        cache = new byte[bytesToRead];
                        txtRecibido += "BytesToRead: " + serialPort1.BytesToRead + "\n";
                        tmpInt = serialPort1.Read(cache, offset, bytesToRead);

                        for (int j = 0; j < bytesToRead; j++)
                        {
                            if (bytesToRead > 1 && cache[0] == 6 && j==0) { j++; Console.WriteLine("Responde: 6 *"); }
                            respuesta += cache[j] + " ";
                        }
                        txtRecibido += "respBytes: " + respuesta + "\n";
                        txtRecibido += "\n";
                        interpreteRespuesta();

                        serialPort1.DiscardInBuffer();
                        bytesToRead = 0;
                        _continue = false;
                        //return;

                    }
                    Thread.Sleep(100);
                    i++;
                    txtRecibido += i + "\n";

                }
                catch (TimeoutException) { }
            }
        }

        public static string strAddXOR(string strNumHexa)
        {
            int i = 0;
            long lSuma = 0;
            long lngLongitud = 0;
            string strSumaxOR = "";
            string strHexa = strNumHexa.Replace(" ", "");

            try
            {
                lngLongitud = strHexa.Length;
                if (lngLongitud < 2) return "";
                for (i = 0; i < lngLongitud; i = i + 2)
                {
                    lSuma = lSuma ^ Convert.ToInt64(strHexa.Trim().Substring(i, 2), 16);
                }

                strSumaxOR = Uri.HexEscape((char)lSuma);
                strSumaxOR = strSumaxOR.Substring(strSumaxOR.Length - 2, 2);
                return strSumaxOR;
            }

            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        void interpreteRespuesta()
        {

            string tmpRespuesta = respuesta.Trim();
            String[] respList = tmpRespuesta.Split(' ');
            byte[] bytes = new byte[respList.Length];

            for (int j = 0; j < respList.Length; j++)
            {
                bytes[j] = Convert.ToByte(respList[j]);
            }


            String codInicio = respList[0];
            Boolean continua = true;
            txtLog += "Responde: ";
            Console.WriteLine("Responde: " + respuesta);
            switch (codInicio)
            {
                case "6": txtLog += "06 Recepción de un mensaje exitosa\n"; continua = false; break;
                case "4": txtLog += "04 End Of Transmission.Cerrando la conexión.\n"; continua = false; break;
                case "21": txtLog += "21  Mensaje no recibido de forma exitosa\n"; continua = false; break;
                case "2": txtLog += "02 Inicio de un mensaje "; break;
            }
            int i = 1;
            string tipoMensaje = "";
            string estatus = "";
            string strLongitud = "";
            int intLongitud = 0;
            List<string[]> parametros = new List<string[]>();
            string tag = "";
            int tmpLong = 0;
            string tmpParam = "";
            string[] data = new String[3];




            if (continua)
            {

                do { tipoMensaje += Convert.ToChar(bytes[i]); i++; } while (i <= 3);
                do { estatus += Convert.ToChar(bytes[i]); i++; } while (i <= 5);
                do
                {
                    strLongitud += bytes[i].ToString("X");
                    i++;
                } while (i <= 7);
                intLongitud = Convert.ToInt32(strLongitud, 16);

                do
                {
                    data = new string[] { "", "", "" };
                    if (bytes[i] == 193)  // C1  
                    {
                        tag = bytes[i].ToString("X");
                        tmpParam = "";
                        i++;
                        tmpLong = bytes[i];
                        i++;
                        if (tmpLong > 0)
                        {
                            for (int j = 0; j < tmpLong; j++)
                            {
                                tmpParam += respList[i] + " ";
                                i++;
                            }
                            data = new string[] { tag, tmpLong.ToString(), tmpParam };
                            parametros.Add(data);
                        }
                        else { data = new string[] { tag, tmpLong.ToString(), "" }; parametros.Add(data); }

                    }
                    if (bytes[i] == 226)  //    E2
                    {
                        tag = bytes[i].ToString("X");
                        tmpParam = "";
                        i++;
                        tmpLong = bytes[i];
                        i++;
                        if (tmpLong > 0)
                        {
                            for (int j = 0; j < tmpLong; j++)
                            {
                                tmpParam += respList[i] + " ";
                                i++;
                            }
                            data = new string[] { tag, tmpLong.ToString(), tmpParam };
                            parametros.Add(data);
                        }
                        else { data = new string[] { tag, tmpLong.ToString(), "" }; parametros.Add(data); }

                    }
                    if (bytes[i] == 225)  // E1
                    {
                        tag = bytes[i].ToString("X");
                        tmpParam = "";
                        i++;
                        tmpLong = bytes[i];
                        i++;
                        if (tmpLong > 0)
                        {
                            for (int j = 0; j < tmpLong; j++)
                            {
                                tmpParam += respList[i] + " ";
                                i++;
                            }
                            data = new string[] { tag, tmpLong.ToString(), tmpParam };
                            parametros.Add(data);
                        }
                        else { data = new string[] { tag, tmpLong.ToString(), "" }; parametros.Add(data); }
                    }
                    if (bytes[i] == 159 && bytes[i + 1] == 110)  // 9F6E
                    {
                        tag = bytes[i].ToString("X");
                        tmpParam = "";
                        tmpParam += respList[i] + respList[i + 1];
                        i = i + 2;
                        data = new string[] { tag, "", tmpParam };
                        parametros.Add(data);
                    }
                    if (bytes[i] == 33 && bytes[i + 1] == 32)  // Token 
                    {
                        i = i + 2;
                        if (bytes[i] == 69 && bytes[i + 1] == 83) { }  // Token ES
                        if (bytes[i] == 82 && bytes[i + 1] == 49) { }  // Token R1
                        if (bytes[i] == 69 && bytes[i + 1] == 90) { }  // Token EZ
                        if (bytes[i] == 69 && bytes[i + 1] == 89) { }  // Token EY
                        if (bytes[i] == 67 && bytes[i + 1] == 90) { }  // Token CZ
                        tag = "Token " + bytes[i].ToString("X") + bytes[i].ToString("X");
                        i = i + 2;
                        string tmp = "";
                        for (int j = 0; j < 5; j++) { tmp += (char)bytes[i]; i++; }
                        tmpLong = Convert.ToInt32(tmp);
                        tmpParam = "";
                        for (int j = 0; j <= tmpLong; j++) { tmpParam += respList[i] + " "; i++; }
                        data = new string[] { tag, tmpLong.ToString(), tmpParam };
                        parametros.Add(data);

                    }

                    if (bytes[i] == 3)  // 3 - fin de instrucciones
                    { break; }

                } while (i <= intLongitud);



                // txtLog.Text += "Tipo: " + tipoMensaje + ", estatus " + statusResp(estatus) + "\n";

                foreach (string[] param in parametros)
                {
                    //     txtLog.Text += "Tag: " + param[0] + " Long: " + param[1] + " parametros: " + param[2] + "\n";
                }

                switch (tipoMensaje)
                {
                    case "C50":
                        enviarCodigos("06");
                        txtLog += "Se contesta con codigo 06\n";
                        break;
                    case "C53":
                        enviarCodigos("06");
                        txtLog += "Se contesta con codigo 06\n";
                        break;
                    case "C54": enviarCodigos("06"); txtLog += "Se contesta con codigo 06\n"; break;
                }
            }

        }

        string postToHostCancel(XtrPost post)
        {
            post.funcion = "cancel";
            //post.Add("authId", authId);
            //post.Add("mti", mti);
            //post.Add("monto", monto);
            //post.Add("entryMode", entryMode);
            //post.Add("stan", stan);
            //post.Add("refNumber", refNum);
            post.Add("cp", "44600");
            post.Add("terminalId", "0000000100000051");
            post.Add("subAfiliadoId", "12005");
            //post.Add("localDate", date);
            //post.Add("localTime", time);
            //post.Add("cardType", tipo);
            //post.Add("tokenES", tokenES);
            //post.Add("tokenEZ", tokenEZ);

            string response = Utils.Post(post, AServerVals.url2);

            txtLog += "\r\rRespuesta Servidor: " + response + "\n\r";

            Console.WriteLine("Respuesta Servidor: " + response);

            return response;

        }

        string postToHost(XtrPost post)
        {
            // post respBytes

            //XtrPost post = new XtrPost();

            post.funcion = "c53";
            //post.Add("c53", respuesta); //report type 1=sales detail

            //post.Add("transactionID", transId);
            //post.Add("mti", mti);
            //post.Add("monto", monto);
            //post.Add("propina", "10");
            //post.Add("stan", stan);
            //post.Add("refNumber", refNum);
            post.Add("cp", "44600");
            post.Add("terminalId", "0000000100000051");
            post.Add("subAfiliadoId", "12005");
            //post.Add("localDate", date);
            //post.Add("localTime", time);
            //post.Add("cardType", tipo);
            //post.Add("dir", dir);
            //post.Add("subAfId", subAfId);

            string response = Utils.Post(post, AServerVals.url2);

            txtLog += "\r\rRespuesta Servidor: " + response + "\n\r";

            Console.WriteLine("Respuesta Servidor: " + response);

            return response;

        }

        string postToHostNewKey(string respuesta, string transId, string monto, string stan, string refNum, string mti, string date, string time)
        {
            // post respBytes

            XtrPost post = new XtrPost();

            post.funcion = "z10";
            post.Add("z10", respuesta); //report type 1=sales detail

            post.Add("transactionID", transId);
            post.Add("mti", mti);
            post.Add("monto", monto);
            //post.Add("propina", "10");
            post.Add("stan", stan);
            post.Add("refNumber", refNum);
            post.Add("cp", "44600");
            post.Add("terminalId", "00000001        ");
            post.Add("localDate", date);
            post.Add("localTime", time);

            string response = Utils.Post(post, AServerVals.url3);

            txtLog += "\r\rRespuesta Servidor: " + response + "\n\r";

            Console.WriteLine("Respuesta Servidor: " + response);

            return response; // datosRespuestaC53(response);

        }

        protected XtrPost getValuesRespC53(string respuestaC53)
        {

            //  $STX = "02"; 
            //  $C53 = "43 35 33";
            //  $status = "00";  //  (exitoso)
            //  $long = "01 90";

            //  C1 Numero de tarjeta(V 2:10)  C1 08 41 52 31 69 24 37 65 80
            //  C1 Tarjetabiente(V :26)   C1 1A 42 41 4E 43 4D 45 52 20 46 49 43 54 49 43 49 4F 2F 4A 55 41 4E 41 20 20 20 20
            //  C1 Track II(V 2:37)   C1 00
            //  C1 Track I(V :79)     C1 00
            //  C1 CVV2 (V 3:4)       C1 00
            //  C1 Modo Lectura       C1 02 30 35
            //  E1 Campos EMV         225 92 79 7 160 0 0 0 3 16 16 159 18 13 66 65 78 67 79 77 69 82 32 86 73 83 65 80 12 86 73 83 65 32 67 82 69 68 73 84 79 95
            //					      48 0 95 52 1 1 159 52 3 1 3 2 194 1 0 149 5 128 128 0 136 0 159 39 1 128 159 38 8 111 177 7 118 45 244 246 60 155 2 104 0 159 57 1 5 138 0 153 0 159 110 0 
            //  E2 CamposEMV Completo 226 139 95 42 2 4 132 130 2 28 0 132 7 160 0 0 0 3 16 16 149 5 128 128 0 136 0 154 3 35 3 40 156 1 0 159 2 6 0 0 0 0 18 18 159
            //					      3 6 0 0 0 0 0 0 159 9 2 0 140 159 16 7 6 1 10 3 164 184 0 159 26 2 4 132 159 30 8 51 65 54 55 48 57 53 50 159 38 8 111 177 7 118 45 244 246 60 159 39 1 128 159 51 3 224 176 200 159 52 3 1 3 2 159 53 1 34 159 54 2 0 9 159 55 4 116 167 16 38 159 65 4 0 0 0 65 159 83 1 82 159 110 0 
            //  Token ES              33 32 69 83 48 48 48 54 48 32 80 88 83 80 51 48 118 48 56 95 54 56 32 32 32 32 32 32 32 32 80 88 48 48 48 48 48 48 48 48 48 48
            //					      51 65 54 55 48 57 53 50 53 48 48 48 48 48 48 48 48 48 48 48 48 48 48 48 48 48 48 48
            //  Token R1              33 32 82 49 48 48 48 48 48 32
            //  Token EZ              33 32 69 90 48 48 48 57 56 32 48 48 50 48 49 48 48 48 48 56 50 49 69 49 48 48 48 48 50 56 48 48 48 48 48 51 57 48 48 49 48 53 51
            //					      55 65 48 48 48 57 68 70 53 57 51 54 56 55 49 50 68 65 54 70 48 52 65 65 52 53 53 50 69 53 57 69 67 55 53 52 57 68 49 68 53 53 54 54 51 67 66 69 52 67 55 48 52 48 55 49 56 69 54 50 49 69 55 53 48
            //  Token EY			  33 32 69 89 48 48 48 48 48 32
            //  Toekn CZ			  33 32 67 90 48 48 48 52 48 32 48 48 48 49 48 48 48 48 48 48 48 48 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32
            //					      32 32 32 32 32 32 32
            //  exitoso				  03
            //  LRC					  69 (por calcular)


            XtrPost post = new XtrPost();

            String[] respList = respuestaC53.Split(' ');

            //  header resp C53
            string stx = respList[0];
            string tokenC53 = respList[1] + respList[2] + respList[3];
            string status = respList[4] + respList[5];
            string longC53Hex = Int32.Parse(respList[6]).ToString("X") + Int32.Parse(respList[7]).ToString("X");
            int longC53Dec = Convert.ToInt32(longC53Hex, 16);

            Console.WriteLine(longC53Dec);

            bool continuar = true;
            int i = 8;
            string initToken = "";
            string longToken = "";
            int posC53 = 0;

            String[] valuesC53 = new String[15];
            //  {
            //      [0]=>C1 Numero de tarjeta
            //      [1]=>C1 Tarjetabiente
            //      [2]=>C1 Track II
            //      [3]=>C1 Track I
            //      [4]=>C1 CVV2
            //      [5]=>C1 Modo Lectura
            //      [6]=>E1 Campos EMV
            //      [7]=>E2 CamposEMV Completo
            //      [8]=>Token ES
            //      [9]=>Token R1
            //      [10]=>Token EZ
            //      [11]=>Token EY
            //      [12]=>Toekn CZ
            //  }

            if (continuar)
            {
                do{
                    initToken = respList[i]; i++;

                        switch (initToken)
                        {
                            case "225": //E1
                            case "226": //E2
                                longToken = respList[i]; i++;
                                valuesC53[posC53] = initToken + " " + longToken + readShortToken(longToken, ref i, ref respList);
                                posC53++;
                                break;
                            case "193": //C1  
                                longToken = respList[i]; i++;
                                valuesC53[posC53] = initToken + " " + longToken + readShortToken(longToken, ref i, ref respList); 
                                posC53++; 
                                break;
                            case "33":  // !
                            //Console.WriteLine("pre-entra: " + respList[i]);
                            valuesC53[posC53] = readLongToken(ref i, ref respList); 
                                posC53++; 
                                break;
                        default:
                            valuesC53[posC53] = "xxxxx";  i++; Console.WriteLine("default: "+i);
                            posC53++;
                            break;
                    }
                    
                } while (i < longC53Dec);
                int p = 0;
                
                foreach (string values in valuesC53  ) {

                    switch (p)
                    {
                        case 0: post.Add("pan", values); break;
                        case 1: post.Add("titular", values); break;
                        case 2: post.Add("track2", values); break;
                        case 3: post.Add("track1", values); break;
                        case 4: post.Add("cvv2", values); break;
                        case 5: post.Add("modLectura", values); break;
                        case 6: post.Add("e1emv", values); break;
                        case 7: post.Add("e2emv", values); break;
                        case 8: post.Add("tokenES", values); break;
                        case 9: post.Add("tokenR1", values); break;
                        case 10: post.Add("tokenEZ", values); break;
                        case 11: post.Add("tokenEY", values); break;
                        case 12: post.Add("tokenCZ", values); break;
                        default:break;
                    }

                    Console.WriteLine(p + "]: " + values);
                    p++;
                }
                Console.WriteLine("** 5]modLectura: " + valuesC53[5]);
                Console.WriteLine("** 8]TokenES: "  + valuesC53[8]);
                Console.WriteLine("** 10]TokenEZ: " + valuesC53[10]);

            }
            return post;
        }

        string getValuesRespZ10(string resp) {
            String[] respList = resp.Split(' ');


            //  header resp Z10
            string stx = respList[0];
            string tokenZ10 = respList[1] + respList[2] + respList[3];
            string status = respList[4] + respList[5];
            string longZ10Hex = respList[6] + respList[7];
            int longZ10Dec = Convert.ToInt32(longZ10Hex, 16);

            bool continuar = true;
            int i = 8;
            string initToken = "";
            string longToken = "";
            int posZ10 = 0;

            String[] valuesZ10 = new String[3];

            if (continuar)
            {
                do
                {
                    valuesZ10[posZ10] = readLongToken(ref i, ref respList);
                    posZ10++;
                    break;
                       
                } while (i < longZ10Dec);
                foreach (string values in valuesZ10)
                {
                    Console.WriteLine(values);
                }

            }

            return "";
        }

        string readShortToken(string longToken, ref int i, ref string[] respList)
        {
            string valToken = "";
            int j = Int16.Parse(longToken);
            while (j > 0)
            {
                valToken += " ";
                valToken += respList[i]; i++; j--;
            }
            return valToken;
        }

        string readLongToken(ref int i, ref string[] respList)
        {
            string headToken = "";
            string valToken = "";
            string longToken = "";
            int j = 1;
            headToken += respList[i-1];
            do {
                headToken += " ";
                headToken += respList[i]; 
                if(j>3 && j < 9) { longToken += Convert.ToChar(Int16.Parse(respList[i])); }
                i++; j++;
            } while (j<10);

            j = Int16.Parse(longToken);

            while (j > 0){ valToken += " "+respList[i]; i++; j--; }

            return headToken + valToken;
        }

        (string, string, string) readCode210(string resultCode210)
        {

            int j = 0;
            int fijo = 0;
            string headerIso = "";
            string mti = "";
            string bitMapPrimary = "";
            string bitMapSecundary = "";       //  C1
            string processingCode = "";        //  C3
            string transactionAmount = "";     //  C4
            string transmissionDateTime = "";  //  C7
            string stan = "";                  //  C11
            string localTransactionTime = "";  //  C12
            string localTransactionDate = "";  //  C13
            string settlementDate = "";        //  C15
            string captureDate = "";           //  C17
            string giro = "";                  //  C18
            string modoTarjeta = "";           //  C22
            string cardNumber = "";            //  C23
            string pointService = "";          //  C25
            string acquiringInstCode = "";     //  C32
            string track2 = "";                //  C35
            string idTrans = "";               //  C37
            string authorizationIdResp = "";   //  C38
            string responseCode = "";          //  C39
            string cardAcceptorId = "";        //  C41
            string cardAcceptorName = "";      //  C43
            string additionalData = "";        //  C48
            string transactionCode = "";       //  C49
            string personalIdNumber = "";      //  C52
            string additionalAmounts = "";     //  C54
            string c55EMVFullGrade = "";       //  C55
                string c55_5f2a = "";
                string c55_71 = "";
                string c55_82 = "";
                string c55_84 = "";
                string c55_91 = "";
                string c55_95 = "";
                string c55_9a = "";
                string c55_9c = "";
                string c55_9f02 = "";
                string c55_9f03 = "";
                string c55_9f09 = "";
                string c55_9f10 = "";
                string c55_9f1a = "";
                string c55_9f1e = "";
                string c55_9f26 = "";
                string c55_9f27 = "";
                string c55_9f33 = "";
                string c55_9f34 = "";
                string c55_9f35 = "";
                string c55_9f36 = "";
                string c55_9f37 = "";
                string c55_9f41 = "";
                string c55_9f53 = "";
                string c55_9f6e = "";
            string puntos = "";                //  C58
            string campana = "";               //  C59
            string posTerminalData = "";       //  C60
            string postalCode = "";            //  C62
            string posAditionalData = "";      //  C63
            string accountId2 = "";            //  C103



            string long210 = resultCode210.Substring(0, 2);
            Console.WriteLine(long210);
            headerIso = resultCode210.Substring(2, 12);
            Console.WriteLine(headerIso);
            mti = resultCode210.Substring(14, 4);
            Console.WriteLine(mti);
            bitMapPrimary = resultCode210.Substring(18, 16);
            Console.WriteLine(bitMapPrimary);
            int posResult = 34;
            
            string bin = Convert.ToString(Convert.ToInt64(bitMapPrimary.ToString(), 16), 2);
            string bin2 = "";

            j = bin.Length;
            while (j < 64){bin = "0"+bin; j++; }

            j = 0;
            while (j < 64) { bin2 = "0" + bin2; j++; }

            if (bin[0].ToString().Equals("1")) { /* C1  secBitMap F16 */
                fijo = 16;
                bitMapSecundary = resultCode210.Substring(posResult, fijo); 
                posResult += fijo;
                Console.WriteLine("bitMapSecundary: " + bitMapSecundary);
                bin2 = Convert.ToString(Convert.ToInt64(bitMapSecundary.ToString(), 16), 2);
                j = bin.Length;
                while (j < 64) { bin2 = "0" + bin2; j++; }
            }

            if (bin[2].ToString().Equals("1")) { /* C3  ProcessingCode F6 */
                fijo = 6;
                processingCode = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[3].ToString().Equals("1")) { /* C4  TransactionAmount F12 */
                fijo = 12;
                transactionAmount = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[6].ToString().Equals("1")) { /* C7  TransmissionDateTime F10 */
                fijo = 10;
                transmissionDateTime = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[10].ToString().Equals("1")) { /* C11  STAN F6 */
                fijo = 6;
                stan = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[11].ToString().Equals("1")) { /* C12  Localtime F6 */
                fijo = 6;
                localTransactionTime = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[12].ToString().Equals("1")) { /* C13  LocalDate F4 */
                fijo = 4;
                localTransactionDate = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[14].ToString().Equals("1")) { /* C15  SettlementDate F4 */
                fijo = 4;
                settlementDate = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
                
            }

            if (bin[16].ToString().Equals("1")) { /* C17  CaptureDate F4 */
                fijo = 4;
                captureDate = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[17].ToString().Equals("1")) { /* C18  Giro F4 */
                fijo = 4;
                giro = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[21].ToString().Equals("1")) { /* C22  ModoTarjeta F3 */
                fijo = 3;
                modoTarjeta = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[22].ToString().Equals("1")) { /* C23  CardNumber F3 */
                fijo = 3;
                cardNumber = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[24].ToString().Equals("1")) { /* C25  PointOfService F2 */
                fijo = 2;
                pointService = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[31].ToString().Equals("1")) { /* C32  AcquiringInstCode V2:11 */
                string longC32 = resultCode210.Substring(posResult, 2);
                posResult += 2;
                int iC32 = Int16.Parse(longC32);
                acquiringInstCode = resultCode210.Substring(posResult, iC32);
                posResult += iC32;
            }

            if (bin[34].ToString().Equals("1")) { /* C35  Track2 V2:37 */
                string longC35 = resultCode210.Substring(posResult, 2);
                posResult += 2;
                int iC35 = Int16.Parse(longC35);
                track2 = resultCode210.Substring(posResult, iC35);
                posResult += iC35;
            }

            if (bin[36].ToString().Equals("1")) { /* C37  IdTrans F12 */
                fijo = 12;
                idTrans = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[37].ToString().Equals("1")) { /* C38  AuthorizationIdResp F6 */
                fijo = 6;
                authorizationIdResp = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
                }

            if (bin[38].ToString().Equals("1")) { /* C39  ResponseCode F2 */
                fijo = 2;
                responseCode = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[40].ToString().Equals("1")) { /* C41  CardAcceptorId F16 */
                fijo = 16;
                cardAcceptorId = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[42].ToString().Equals("1")) { /* C43  CardAcceptorName F40 */
                fijo = 40;
                cardAcceptorName = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[47].ToString().Equals("1")) { /* C48  AdditionalData V3:27 */
                string longC48 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC48 = Int16.Parse(longC48);
                additionalData = resultCode210.Substring(posResult, iC48);
                posResult += iC48;
            }

            if (bin[48].ToString().Equals("1")) { /* C49  TransactionCode F3 */
                fijo = 3;
                transactionCode = resultCode210.Substring(posResult, fijo);
                posResult += fijo;
            }

            if (bin[51].ToString().Equals("1")) { /* C52  PersonalIdNumber AN16   (Pendiente) */ }

            if (bin[53].ToString().Equals("1")) { /* C54  AdditionalAmounts V3:12  (no se maneja propina) */ }
            
            if (bin[54].ToString().Equals("1")) { /* C55  EMVFullGrade V3:999 */
                
                int posc55 = 0;
                string longC55 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC55 = Int16.Parse(longC55);
                c55EMVFullGrade = resultCode210.Substring(posResult, iC55);

                // 5f2a
                //getValuesC55(ref c55_5f2a, ref posc55, c55EMVFullGrade, "5f2a");

                // 71/72
                getValuesC55(ref c55_71, ref posc55, c55EMVFullGrade, "71");

                // 82
                //getValuesC55(ref c55_82, ref posc55, c55EMVFullGrade, "82");

                // 84
                //getValuesC55(ref c55_84, ref posc55, c55EMVFullGrade, "84");

                // 91
                getValuesC55(ref c55_91, ref posc55, c55EMVFullGrade, "91");

                // 95
                //getValuesC55(ref c55_95, ref posc55, c55EMVFullGrade, "95");
                
                // 9a
                //getValuesC55(ref c55_9a, ref posc55, c55EMVFullGrade, "9a");
                
                // 9c
                //getValuesC55(ref c55_9c, ref posc55, c55EMVFullGrade, "9c");
                
                // 9f02
                //getValuesC55(ref c55_9f02, ref posc55, c55EMVFullGrade, "9f02");
                
                // 9f03
                //getValuesC55(ref c55_9f03, ref posc55, c55EMVFullGrade, "9f03");
                
                // 9f09
                //getValuesC55(ref c55_9f09, ref posc55, c55EMVFullGrade, "9f09");
                
                // 9f10
                //getValuesC55(ref c55_9f10, ref posc55, c55EMVFullGrade, "9f10");
                
                // 9f1a
                //getValuesC55(ref c55_9f1a, ref posc55, c55EMVFullGrade, "9f1a");
                
                // 9f1e
                //getValuesC55(ref c55_9f1e, ref posc55, c55EMVFullGrade, "9f1e");
                
                // 9f26
                //getValuesC55(ref c55_9f26, ref posc55, c55EMVFullGrade, "9f26");
                
                // 9f27
                //getValuesC55(ref c55_9f27, ref posc55, c55EMVFullGrade, "9f27");
                
                // 9f33
                //getValuesC55(ref c55_9f33, ref posc55, c55EMVFullGrade, "9f33");
                
                // 9f34
                //getValuesC55(ref c55_9f34, ref posc55, c55EMVFullGrade, "9f34");
                
                // 9f35
                //getValuesC55(ref c55_9f35, ref posc55, c55EMVFullGrade, "9f35");
                
                // 9f36
                //getValuesC55(ref c55_9f36, ref posc55, c55EMVFullGrade, "9f36");
                
                // 9f37
                //getValuesC55(ref c55_9f37, ref posc55, c55EMVFullGrade, "9f37");
                
                // 9f41
                //getValuesC55(ref c55_9f41, ref posc55, c55EMVFullGrade, "9f41");
                
                // 9f53
                //getValuesC55(ref c55_9f53, ref posc55, c55EMVFullGrade, "9f53");
                
                // 9f6e
                //getValuesC55(ref c55_9f6e, ref posc55, c55EMVFullGrade, "9f6e");
                
                posResult += iC55;

            }

            if (bin[57].ToString().Equals("1")) { /* C58  Puntos V:420 */
                string longC58 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC58 = Int16.Parse(longC58);
                puntos = resultCode210.Substring(posResult, iC58);
                posResult += iC58;
            }

            if (bin[58].ToString().Equals("1")) { /* C59  Campaña V:999 */
                //  campos tokens 2 y 6
                string longC59 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC59 = Int16.Parse(longC59);
                campana = resultCode210.Substring(posResult, iC59);
                posResult += iC59;

                longC59 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                iC59 = Int16.Parse(longC59);
                campana += resultCode210.Substring(posResult, iC59);
                posResult += iC59;
            }

            if (bin[59].ToString().Equals("1")) { /* C60  PosTerminalData V3:16 */
                string longC60 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC60 = Int16.Parse(longC60);
                posTerminalData = resultCode210.Substring(posResult, iC60);
                posResult += iC60;
            }

            if (bin[61].ToString().Equals("1")) { /* C62  PostalCode ANS13 */
                string longC62 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC62 = Int16.Parse(longC62);
                postalCode = resultCode210.Substring(posResult, iC62);
                posResult += iC62;
            }

            if (bin[62].ToString().Equals("1")) { /* C63  PosAditionalData V:999 */
                string longC63 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC63 = Int16.Parse(longC63);
                posAditionalData = resultCode210.Substring(posResult, iC63);
                posResult += iC63;
            }

            if (bin2[38].ToString().Equals("1")) { /* C103  AccountId2 V2:28 */
                string longC103 = resultCode210.Substring(posResult, 3);
                posResult += 3;
                int iC103 = Int16.Parse(longC103);
                accountId2 = resultCode210.Substring(posResult, iC103);
                posResult += iC103;
            }

            return (authorizationIdResp, responseCode, c55_91);

        }

        void getValuesC55(ref string token, ref int posc55, string c55EMVFullGrade, string tag) {
            
            string c55_tag_token = "";
            string c55_tag_long = "";
            string c55_tag_body = "";

            int longTag = tag.Length;

            c55_tag_token = c55EMVFullGrade.Substring(posc55, longTag);
            if(tag == c55_tag_token) {
                posc55 += longTag;
                c55_tag_long = c55EMVFullGrade.Substring(posc55, 2); posc55 += 2;
                int iC55_5f2a = Int16.Parse(c55_tag_long);
                c55_tag_body = c55EMVFullGrade.Substring(posc55, iC55_5f2a); posc55 += iC55_5f2a;

                token = c55_tag_body;
            }
            else {
                token = "";
            }
            
        }

        string getDatosEX(string factMessage) {
            //Console.WriteLine(factMessage);

           // string factMessage = "";
            int first = 0;
            string str1 = "";
            int longEX = 0;
            string str2 = "";
            //factMessage = "! ER00002 bandera actualizacion llavez! EX00013 llave cifrada";
            first = factMessage.IndexOf("! EX");

            //Console.WriteLine(first);
            str1 = factMessage.Substring((first + 4), 5);
            //Console.WriteLine(str1);
            longEX = Int32.Parse(str1);
            //Console.WriteLine(longEX);
            str2 = factMessage.Substring(first, (longEX + 10));
            //Console.WriteLine(str2);

            //factMessage = "33 32 69 80 0 0 0 0 2 32 b a n d e r a _ a c t u a l i z a c i o n _ l l a v e z 33 32 69 88 48 48 48 49 51 32 l l a v e _ c i f r a d a";
            //first = factMessage.IndexOf("33 32 69 88 48 48 48 54 56 32");
            /*
            String[] respList = factMessage.Split(' ');

            int i = 0;
            int j = 5;

            while (i < respList.Length)
            {
                if (respList[i] == "33" && respList[i + 1] == "32" && respList[i + 2] == "69" && respList[i + 3] == "88")
                {
                    first = i;
                    str2 = respList[i] + " " + respList[i + 1] + " " + respList[i + 2] + " " + respList[i + 3];
                    i += 4;
                    break;
                }
                i++;
            }
            while (j > 0) {
                str1 += (char)(Int32.Parse(respList[i]));
                str2 += " " + respList[i];
                j--;
                i++;
            }
            
            Console.WriteLine(str1);
            longEX = Int32.Parse(str1);
            j = longEX;
            i++;
            while (j > 0) { str2 += respList[i]; j--; i++; }
            */
            //Console.WriteLine(str2);

            return str2;
        }
    }
}
