using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class WebResponseInfo<T> where T : class, new()
    {
        [JsonProperty("code")]
        public int Code { get; set; } = 0;
        [JsonProperty("msg")]
        public string Msg { get; set; } = "成功";
        [JsonProperty("count")]
        public int Count { get; set; } = 0;
        [JsonProperty("data")]
        public List<T> Data { get; set; } = new();
    }
}
