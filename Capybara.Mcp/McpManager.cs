using LLMGateway.Models;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Newtonsoft.Json.Linq;

namespace Capybara.Mcp
{
    public class McpServerConfigInfo
    {
        public string endpoint { get; set; } = string.Empty;
        public string apiKey { get; set; } = string.Empty;
    }
    public class McpManager : IDisposable
    {
        private readonly List<McpClient> clients_ = new List<McpClient>();
        private readonly List<McpClientTool> tools_ = new List<McpClientTool>();
        private bool disposed_ { get; set; } = false;
        public McpManager(List<McpServerConfigInfo> configs)
        {
            foreach (var config in configs)
            {
                var httpOptions = new HttpClientTransportOptions
                {
                    Endpoint = new Uri(config.endpoint),
                    AdditionalHeaders = new Dictionary<string, string> { { "Authorization", $"Bearer {config.apiKey}" } }
                };
                var mcpClient = McpClient.CreateAsync(new HttpClientTransport(httpOptions)).GetAwaiter().GetResult();
                var tools = mcpClient.ListToolsAsync().GetAwaiter().GetResult();
                clients_.Add(mcpClient);
                tools_.AddRange(tools);
            }
        }
        public List<LLMToolDefinitionInfo> GetTools(List<string> tools)
        {
            List<LLMToolDefinitionInfo> result = new List<LLMToolDefinitionInfo>();
            foreach (var item in tools_)
            {
                if (tools.Contains(item.Name))
                {
                    LLMToolDefinitionInfo toolInfo = new LLMToolDefinitionInfo
                    {
                        Name = item.Name,
                        Description = item.Description,
                        Arguments = new List<LLMToolDefinitionArgumentInfo>()
                    };

                    JObject schema = JObject.Parse(item.JsonSchema.ToString());
                    var properties = schema["properties"] as JObject;
                    var required = schema["required"] as JArray;
                    if (properties == null) continue;

                    foreach (var prop in properties)
                    {
                        LLMToolDefinitionArgumentInfo param = new LLMToolDefinitionArgumentInfo();
                        var name = prop.Key;
                        var type = prop.Value?["type"]?.ToString();
                        var description = prop.Value?["description"]?.ToString();
                        if (name == null || type == null || description == null) continue;

                        param.Name = name;
                        param.Type = type;
                        param.Description = description;
                        param.IsRequired = required != null && required.Any(r => r.ToString() == name);
                        toolInfo.Arguments.Add(param);
                    }
                    result.Add(toolInfo);
                }
            }
            return result;
        }
        public string Invoke(LLMFunctionCallRequestInfo tool)
        {
            var func = tools_.FirstOrDefault(n => n.Name == tool.Name);
            if (func == null) return "方法不存在!";
            var arguments = new Dictionary<string, object?>();

            var paramList = JObject.Parse(tool.Arguments);
            if(paramList == null) return "参数错误!";

            foreach (var property in paramList.Properties())
            {
                arguments[property.Name] = property.Value.ToObject<object>();
            }
            var functionArguments = new AIFunctionArguments(arguments);
            var result = func.InvokeAsync(functionArguments).GetAwaiter().GetResult();
            return result?.ToString() ?? "";
        }
        public void Dispose()
        {
            if (disposed_) return;
            disposed_ = true;
            foreach (var client in clients_)
            {
                client.DisposeAsync().GetAwaiter().GetResult();
            }

        }
    }
}
