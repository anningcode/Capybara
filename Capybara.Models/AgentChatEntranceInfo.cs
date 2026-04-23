using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentChatEntranceInfo
    {
        public string file { get; set; } = string.Empty;
        public string param { get; set; } = string.Empty;
        public bool enable { get; set; } = true;
        public string remarks { get; set; } = string.Empty;
    }
}
