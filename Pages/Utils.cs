using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace commPinPad.Pages
{
    public static class Utils
    {
        public static string BuildTag(this string text, string tag)
        {
            return "[" + tag + "]" + text + "[/" + tag + "]";
        }
        public static string RegCode(int cont)
        {
            string CODE = "";
            Random n = new Random(Guid.NewGuid().GetHashCode());
            int val = 0;
            while (cont > 0)
            {
                val = n.Next(47, 123);
                if ((val > 48 && val < 57) || (val > 97 && val < 123))
                {
                    CODE += (char)val;
                    cont--;
                }
            }
            return CODE;
        }
        public static string Post(XtrPost param, string urlx)
        {
            try
            {

                using (var client = new WebClient())
                {
                    string tag = "";
                    var values = new NameValueCollection();
                    values["Function"] = param.funcion;
                    for (int a = 0; a < param.param.Count; a++)
                    { tag += param.param[a].valor.BuildTag(param.param[a].tag); }
                    string PostUID = Utils.RegCode(6);
                    tag = PostUID.BuildTag("PostUID") + tag;
                    tag = tag.BuildTag("DATA");
                    values["DATA"] = tag;
                    var response = client.UploadValues(urlx, values);
                    var responseString = Encoding.UTF8.GetString(response);
                    //var responseString0 = Encoding.Default.GetString(response);
                    //var responseString1 = Encoding.Unicode.GetString(response);
                    //var responseString3 = Encoding.ASCII.GetString(response);
                    return Convert.ToString(responseString);
                }
            }
            catch (Exception e)
            {
                return "2".BuildTag("Response") + e.Message.BuildTag("ErrMsg");
            }
        }

        public static string longitudParametros(string parametros)
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

        public static string getDate()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yy MM dd");
        }

        public static string getTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("HH mm ss");
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

        public static string readShortToken(string longToken, ref int i, ref string[] respList)
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

        public static string readLongToken(ref int i, ref string[] respList)
        {
            string headToken = "";
            string valToken = "";
            string longToken = "";
            int j = 1;
            headToken += respList[i - 1];
            do
            {
                headToken += " ";
                headToken += respList[i];
                if (j > 3 && j < 9) { longToken += Convert.ToChar(Int16.Parse(respList[i])); }
                i++; j++;
            } while (j < 10);

            j = Int16.Parse(longToken);

            while (j > 0) { valToken += " " + respList[i]; i++; j--; }

            return headToken + valToken;
        }

    }

    public class XtrPost
    {
        //public RunAfter RunAfter;
        public string funcion, url;
        public bool SkypSwitch;
        public List<xparam> param;
        public XtrPost()
        {
            funcion = string.Empty;
            url = string.Empty;
            param = new List<xparam>();
        }
        public XtrPost(string func)
        {
            funcion = func;
            param = new List<xparam>();
        }
        public class xparam
        {
            public string tag;
            public string valor;
            public xparam()
            {
                tag = string.Empty;
                valor = string.Empty;
            }
        }

        public void Add(string Tag, string Val)
        {
            param.Add(new xparam { tag = Tag, valor = Val });
        }

        

    }


}
