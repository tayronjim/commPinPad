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
using System.Security.Cryptography;

namespace commPinPad.Pages
{
    public class IndexModel : PageModel
    {
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
            Utils.getDate();


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
            Pinpad.abrirPuerto();
        }

        public async Task OnPostCerrarPuerto()
        {
            Console.WriteLine("OnPostCerrarPuerto");
            txtRecibido = "OnPostCerrarPuerto";
            Pinpad.cerrarPuerto();
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

        
        public async Task OnPostRespC53()
        {
            string resultCode210 = "?�ISO0240000770210323A84000E818012000000000000048061071119010200130113005307110711071105100000001301000202P0000000001        0274139942            00000000484000070& 0000500070! Q100002 04! Q200002 04! C400012 000000001032! ER00002 00";

            Console.WriteLine("readCode210");
            var respuesta = Pinpad.readCode210(resultCode210);
            
            string authId = respuesta.Item1;
            string respCode = respuesta.Item2;
            string AuthData = respuesta.Item3;
            Console.WriteLine(respuesta.Item1);
            Console.WriteLine(respuesta.Item2);
            Console.WriteLine(respuesta.Item3);
            Pinpad.comando_C54(authId, respCode, AuthData);
        }

        
        
        
        
        
        
        
        
    }
}
