using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 一次工具调用请求 / 响应（用于消息中）
/// </summary>
public class LLMFunctionCallRequestInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("arguments")]
    public string Arguments { get; set; } = string.Empty;

    [JsonProperty("response")]
    public string? Response { get; set; } = null;
    [JsonProperty("truncated")]
    public bool Truncated { get; set; } = false;
}
