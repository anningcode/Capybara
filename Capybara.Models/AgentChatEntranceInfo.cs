using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentChatEntranceInfo
    {
        [JsonProperty("file")]
        public string File { get; set; } = string.Empty;
        [JsonProperty("param")]
        public string Param { get; set; } = string.Empty;
        [JsonProperty("enable")]
        public bool Enable { get; set; } = true;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
}
