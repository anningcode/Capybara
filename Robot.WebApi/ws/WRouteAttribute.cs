using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.ws
{
    public class WRouteAttribute : Attribute
    {
        public string route { get; set; } = string.Empty;
        public WRouteAttribute(string value)
        {
            route = value;
        }
    }
}
