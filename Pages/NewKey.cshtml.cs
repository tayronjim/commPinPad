using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace commPinPad.Pages
{
	public class NewKeyModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task OnPostGeneraLlave()
        {
            //string datosRespCZ10 = "";
            
            string transId = Request.Form["txtTransIdKeyGen"];
            string stan = Request.Form["txtStanKeyGen"];
            string refNum = Request.Form["txtRefNumKeyGen"];
            


            Pinpad.generaLlave(transId, stan, refNum);


        }

        

    }
}
