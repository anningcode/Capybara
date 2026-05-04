using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 完整聊天响应
/// </summary>
public class LLMChatResponseInfo
{
    [JsonProperty("think")]
    public string Think { get; set; } = string.Empty;

    [JsonProperty("answer")]
    public string Answer { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty("toolCalls")]
    public List<LLMFunctionCallRequestInfo> ToolCalls { get; set; } = new();

    [JsonProperty("success")]
    public bool Success { get; set; } = true;

    [JsonProperty("stop")]
    public bool Stop { get; set; } = false;

    [JsonProperty("message")]
    public string Message { get; set; } = "成功";
}
