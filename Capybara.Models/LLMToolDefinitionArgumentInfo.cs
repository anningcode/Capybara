using Newtonsoft.Json;

namespace LLMGateway.Models;

/// <summary>
/// 工具定义中的参数描述
/// </summary>
public class LLMToolDefinitionArgumentInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("isRequired")]
    public bool IsRequired { get; set; } = true;
}
