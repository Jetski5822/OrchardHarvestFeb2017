using Microsoft.AspNetCore.Mvc;

namespace Harvest.Demo2.Module2
{
    public class TestController : Controller
    {
        public IActionResult Home()
        {
            return new JsonResult("{ working: true }");
        }
    }
}
