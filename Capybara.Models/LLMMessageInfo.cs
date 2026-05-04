using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 一条对话消息（支持多角色与思考）
/// </summary>
public class LLMMessageInfo
{
    [JsonProperty("role")]
    public LLMRole Role { get; set; } = LLMRole.User;

    [JsonProperty("think")]
    public string Think { get; set; } = string.Empty;

    [JsonProperty("answer")]
    public string Answer { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty("toolCalls")]
    public List<LLMFunctionCallRequestInfo> ToolCalls { get; set; } = new();
}
