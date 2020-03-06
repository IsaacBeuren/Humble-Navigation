using Br.Scania.ExternalAGV.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class CommandsController : ControllerBase
    {
        [HttpPost]
        public ActionResult Moviments(ManualMoviments moviments)
        {
            return Ok();
        }
    }
}