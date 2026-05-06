using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class WebGeneralConfigInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 0;
        [JsonProperty("key")]
        public string Key { get; set; } = "";
        [JsonProperty("value")]
        public string Value { get; set; } = "";
        [JsonProperty("enable")]
        public bool Enable { get; set; } = true;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = "";
    }
}
