using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchard.Environment.Shell.Builders.Models;

namespace Harvest.Two.HelloWorld.Models
{
    public class HelloModel
    {
        public ShellBlueprint Blueprint { get; set; }
        public bool Exists { get; set; }
    }
}
