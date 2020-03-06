using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebUI.Controllers
{

    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}