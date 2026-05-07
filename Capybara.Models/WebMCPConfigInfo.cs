using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class WebMCPConfigInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; } = "";
        [JsonProperty("appKey")]
        public string AppKey { get; set; } = "";
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = "";
    }
}
