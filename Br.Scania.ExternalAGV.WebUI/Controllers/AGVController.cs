using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebUI.Controllers
{
    public class AGVController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Monitor()
        {
            return View();
        }
    }
}