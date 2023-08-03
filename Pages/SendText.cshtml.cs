using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace commPinPad.Pages
{
	public class SendTextModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task OnPostEnviarTexto()
        {
            Console.WriteLine("OnPostEnviarTexto");
            Pinpad.abrirPuerto();
            string texto = Request.Form["txttexto"];
            enviarTexto(texto);
            Pinpad.cerrarPuerto();
        }

        public void enviarTexto(string texto)
        {
            String txtHexa = Pinpad.comando_textScreen(texto);
            Pinpad.enviarCodigos(txtHexa);
            Console.WriteLine("Texto enviado");
            //return "Texto enviado";
        }


    }
}
