using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class AntennaController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Antenna/OpenReadAntenna")]
        public ActionResult OpenReadAntenna()
        {
            Process myProcess = new Process();
            var str = "";

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "C:\\AntennaGPS\\ReadAntenna.exe";
                //myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself.
                // Given that is is started without a window so you cannot terminate it
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.
            }
            catch (Exception e)
            {
                str = e.Message;
                Console.WriteLine(e.Message);
                return Unauthorized();
            }

            return base.Content("teste" + str, "text/html", Encoding.UTF8);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Antenna/StartPLCByID")]
        public ActionResult StartPLCByID(int ID)
        {

            //StartBusiness context = new StartBusiness();
            //bool ret = context.UpdateByID(ID);
            //if (ret == null)
            //{
            //    return NotFound();
            //}
            return Ok(/*ret*/);

        }

    }
}