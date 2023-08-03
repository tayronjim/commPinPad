using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace commPinPad.Pages
{
	public class CompraModel : PageModel
    {
        public void OnGet()
        {
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
            string tokenE1 = Request.Form["txtTokenE1"];
            string tokenE2 = Request.Form["txtTokenE2"];


            solicitarCancelar(monto, stan, refNum, tipo, authId, entryMode, tokenES, tokenEZ, tokenE1, tokenE2);
        }

        protected void solicitarCancelar(string monto, string stan, string refNum, string tipo, string authId, string entryMode, string tokenES, string tokenEZ, string tokenE1, string tokenE2)
        {
            string mti = "0420";
            string date = Utils.getDate();
            string time = Utils.getTime();

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
            postCancelar.Add("e1emv", tokenE1);
            postCancelar.Add("e2emv", tokenE2);

            string datosRespCancel = "";

            datosRespCancel = Post.toHostCancel(postCancelar);

        }

        
    }
}
