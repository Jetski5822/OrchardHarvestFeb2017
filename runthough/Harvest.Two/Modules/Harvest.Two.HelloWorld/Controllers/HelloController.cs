using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harvest.Two.HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;
using Orchard.Environment.Shell.Builders.Models;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Environment.Extensions.Features.Attributes;

namespace Harvest.Two.HelloWorld.Controllers
{
    [OrchardFeature("Harvest.Two.HelloWorld")]
    public class HelloController : Controller
    {
        private readonly ShellBlueprint _blueprint;
        private readonly IServiceProvider _serviceProvider;

        public HelloController(ShellBlueprint blueprint,
            IServiceProvider serviceProvider)
        {
            _blueprint = blueprint;
            _serviceProvider = serviceProvider;
        }

        public IActionResult Home()
        {
            var model = new HelloModel {
                Blueprint = _blueprint,
                Exists = _serviceProvider.GetService<TestInjection>() != null
            };

            return View(model);
        }
    }
}
