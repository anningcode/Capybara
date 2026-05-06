using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class WebUserConfigInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 0;
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("code")]
        public string Code { get; set; } = "";
        [JsonProperty("password")]
        public string Password { get; set; } = "";
        [JsonProperty("enable")]
        public bool Enable { get; set; } = true;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = "";
    }
}
