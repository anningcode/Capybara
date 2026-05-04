using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 单个工具定义（名称 + 描述 + 参数列表）
/// </summary>
public class LLMToolDefinitionInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("arguments")]
    public List<LLMToolDefinitionArgumentInfo> Arguments { get; set; } = new();
}
