using System;
namespace commPinPad.Pages
{
	public class Post
	{
		public Post()
		{
		}

        static public string toHost(XtrPost post)
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

            Console.WriteLine("Respuesta Servidor: " + response);

            return response;

        }

        public static string toHostCancel(XtrPost post)
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



            Console.WriteLine("Respuesta Servidor: " + response);

            return response;

        }

        static public string toHostNewKey(string respuesta, string transId, string stan, string refNum, string mti, string date, string time)
        {
            // post respBytes

            XtrPost post = new XtrPost();

            post.funcion = "z10";
            post.Add("z10", respuesta); //report type 1=sales detail

            post.Add("transactionID", transId);
            post.Add("mti", mti);
            //post.Add("propina", "10");
            post.Add("stan", stan);
            post.Add("refNumber", refNum);
            post.Add("cp", "44600");
            post.Add("terminalId", "00000001        ");
            post.Add("localDate", date);
            post.Add("localTime", time);

            string response = Utils.Post(post, AServerVals.url3);

            Console.WriteLine("Respuesta Servidor: " + response);

            return response;

        }

    }
}

