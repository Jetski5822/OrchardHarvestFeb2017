using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orchard.Environment.Shell;

namespace Harvest.LiveDemo2Again.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShellSettings _settings;

        public HomeController(ShellSettings settings) {
            _settings = settings;
        }

        public IActionResult Index() {
            return new JsonResult("{ tenant: '" + _settings .Name + "'}");
        }
    }
}
