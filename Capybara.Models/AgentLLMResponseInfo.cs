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
        public string think { get; set; } = string.Empty;
        // 结论
        public string answer { get; set; } = string.Empty;
        // 内容
        public string content { get; set; } = string.Empty;
        // 消息
        public string message { get; set; } = "未知异常!";
        // 成功
        public bool success { get; set; } = false;
        // 停止,输出结束
        public bool stop { get; set; } = true;
        // 方法
        public List<AgentLLMItemFuncRequestInfo> toolCalls { get; set; } = new List<AgentLLMItemFuncRequestInfo>();
    }
}
