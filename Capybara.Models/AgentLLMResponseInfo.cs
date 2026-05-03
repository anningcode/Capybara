using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentLLMResponseInfo
    {
        // 思考
        [JsonProperty("think")]
        public string Think { get; set; } = string.Empty;
        // 结论
        [JsonProperty("answer")]
        public string Answer { get; set; } = string.Empty;
        // 内容
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
        // 消息
        [JsonProperty("message")]
        public string Message { get; set; } = "未知异常!";
        // 成功
        [JsonProperty("success")]
        public bool Success { get; set; } = false;
        // 停止,输出结束
        [JsonProperty("stop")]
        public bool Stop { get; set; } = true;
        // 方法
        [JsonProperty("toolCalls")]
        public List<AgentLLMItemFuncRequestInfo> ToolCalls { get; set; } = new List<AgentLLMItemFuncRequestInfo>();
    }
}
