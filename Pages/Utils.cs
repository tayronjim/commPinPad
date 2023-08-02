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
