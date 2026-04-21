using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Capybara.Tool
{
    public class AgentFunctionAttribute : Attribute
    {
        public string funcName { get; set; } = string.Empty;
        public AgentFunctionAttribute(string funcName)
        {
            this.funcName = funcName;
        }
    }
}
