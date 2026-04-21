using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentLLMItemFuncRequestInfo
    {
        // 方法名称
        public string name { get; set; } = string.Empty;
        // 参数
        public string arguments { get; set; } = string.Empty;
        // 工具响应
        public string? response { get; set; } = null;
    };

    public class AgentLLMItemRequestInfo
    {
        // 类型 user, assistant, tool
        public string role { get; set; } = string.Empty;
        // 思考
        public string think { get; set; } = string.Empty;
        // 最终结果
        public string answer { get; set; } = string.Empty;
        // 内容
        public string content { get; set; } = string.Empty;
        // 工具列表
        public List<AgentLLMItemFuncRequestInfo> toolCalls { get; set; } = new List<AgentLLMItemFuncRequestInfo>();
    };

    public class AgentLLMToolCallsArgumentRequestInfo
    {
        // 参数名称
        public string name { get; set; } = string.Empty;
        // 参数类型
        public string type { get; set; } = string.Empty;
        // 描述
        public string description { get; set; } = string.Empty;
    };

    public class AgentLLMToolCallsRequestInfo
    {
        // 方法名称
        public string name { get; set; } = string.Empty;
        // 描述
        public string description { get; set; } = string.Empty;
        // 参数列表
        public List<AgentLLMToolCallsArgumentRequestInfo> arguments { get; set; } = new List<AgentLLMToolCallsArgumentRequestInfo>();
    };

    public class AgentLLMRequestInfo
    {
        // apiKey
        public string apiKey { get; set; } = string.Empty;
        // 大模型地址
        public string address { get; set; } = "127.0.0.1:8000";
        // 请求内容
        public string model { get; set; } = string.Empty;
        // 最大token数量
        public int maxTokens { get; set; } = 4096;
        // 温度参数
        public double temperature { get; set; } = 0.7;
        // 启动思考模式
        public bool thinking { get; set; } = true;
        // 上下文列表
        public List<AgentLLMItemRequestInfo> context { get; set; } = new List<AgentLLMItemRequestInfo>();
        // 工具列表
        public List<AgentLLMToolCallsRequestInfo> tools { get; set; } = new List<AgentLLMToolCallsRequestInfo>();
    }
}
