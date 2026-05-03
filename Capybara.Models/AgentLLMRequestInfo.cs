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
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 参数
        [JsonProperty("arguments")]
        public string Arguments { get; set; } = string.Empty;
        // 工具响应
        [JsonProperty("response")]
        public string? Response { get; set; } = null;
    };

    public class AgentLLMItemRequestInfo
    {
        // 类型 user, assistant, tool
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
        // 思考
        [JsonProperty("think")]
        public string Think { get; set; } = string.Empty;
        // 最终结果
        [JsonProperty("answer")]
        public string Answer { get; set; } = string.Empty;
        // 内容
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
        // 工具列表
        [JsonProperty("toolCalls")]
        public List<AgentLLMItemFuncRequestInfo> ToolCalls { get; set; } = new List<AgentLLMItemFuncRequestInfo>();
    };

    public class AgentLLMToolCallsArgumentRequestInfo
    {
        // 参数名称
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 参数类型
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
        // 描述
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
    };

    public class AgentLLMToolCallsRequestInfo
    {
        // 方法名称
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 描述
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        // 参数列表
        [JsonProperty("arguments")]
        public List<AgentLLMToolCallsArgumentRequestInfo> Arguments { get; set; } = new List<AgentLLMToolCallsArgumentRequestInfo>();
    };

    public class AgentLLMRequestInfo
    {
        // apiKey
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        // 大模型地址
        [JsonProperty("address")]
        public string Address { get; set; } = "127.0.0.1:8000";
        // 请求内容
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;
        // 最大token数量
        [JsonProperty("maxTokens")]
        public int MaxTokens { get; set; } = 4096;
        // 温度参数
        [JsonProperty("temperature")]
        public double Temperature { get; set; } = 0.7;
        // 启动思考模式
        [JsonProperty("thinking")]
        public bool Thinking { get; set; } = true;
        // 上下文列表
        [JsonProperty("context")]
        public List<AgentLLMItemRequestInfo> Context { get; set; } = new List<AgentLLMItemRequestInfo>();
        // 工具列表
        [JsonProperty("tools")]
        public List<AgentLLMToolCallsRequestInfo> Tools { get; set; } = new List<AgentLLMToolCallsRequestInfo>();
    }
}
