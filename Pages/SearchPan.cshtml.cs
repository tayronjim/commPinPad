using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace commPinPad.Pages
{
	public class SearchPanModel : PageModel
    {
        

        public void OnGet()
        {
        }

       
      

        public async Task OnPostBuscarPan()
        {
            Console.WriteLine("OnPostBuscarPan");
            string pan = Request.Form["txtpan"];
            string tipo = Request.Form["txtMonto"];
            Console.WriteLine(tipo);
            buscarPan(pan);
        }

        void buscarPan(string pan) {
            buscarCsv(pan);
        }

        void buscarCsv(string pan) {
            string dir = Environment.CurrentDirectory+"/Pages/files/Bin_03072023.csv";
            System.IO.StreamReader archivo = new System.IO.StreamReader(dir);

            string separador = ",";
            string linea;
            archivo.ReadLine();

            string bin = "";
            string producto = "";
            string tipo = "";
            string marca = "";

            while ((linea = archivo.ReadLine()) != null)
            {
                string[] fila = linea.Split(separador);

                bin = fila[0];
                producto = fila[1];
                tipo = fila[2];
                marca = fila[3];
                if(bin == pan) { break; }
            }
            Console.WriteLine(pan);
            Console.WriteLine("bin "+bin+" producto "+ producto + " y tipo "+ tipo);
        }
    }
}
