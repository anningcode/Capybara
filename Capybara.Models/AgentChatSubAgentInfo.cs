using Newtonsoft.Json;
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
        [JsonProperty("count")]
        public int Count { get; set; } = 0;
        // 子智能体ID列表
        [JsonProperty("ids")]
        public List<string> Ids { get; set; } = new List<string>();
    }
}
