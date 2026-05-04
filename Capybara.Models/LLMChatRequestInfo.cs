using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 完整聊天请求
/// </summary>
public class LLMChatRequestInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("maxTokens")]
    public int MaxTokens { get; set; } = 4096;

    [JsonProperty("temperature")]
    public double Temperature { get; set; } = 0.7;

    [JsonProperty("thinking")]
    public bool Thinking { get; set; } = true;

    [JsonProperty("context")]
    public List<LLMMessageInfo> Context { get; set; } = new();

    [JsonProperty("tools")]
    public List<LLMToolDefinitionInfo> Tools { get; set; } = new();
}
