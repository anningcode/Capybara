using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentChatSubAgentInfo
    {
        // 子智能体数量
        public int count { get; set; } = 0;
        // 子智能体ID列表
        public List<string> ids { get; set; } = new List<string>();
    }
}
