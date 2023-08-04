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

            Pinpad.abrirPuerto();
            Pinpad.solicitarPago(transId, monto, stan, refNum, tipo, dir, subAfId);
            Pinpad.cerrarPuerto();
        }

        
        
        

    }
}
