using LLMGateway.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    public class AgentChatUserInfo
    {
        // ID
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        // 关联智能体ID
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        // 名称
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 年龄
        [JsonProperty("age")]
        public int Age { get; set; }
        // 性别 1:男,2:女
        [JsonProperty("sex")]
        public int Sex { get; set; }
        // 身高
        [JsonProperty("height")]
        public int Height { get; set; }
        // 体重
        [JsonProperty("weight")]
        public int Weight { get; set; }
        // 备注
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatRoleInfo
    {
        // ID
        [JsonProperty("id")]
        public int Id { get; set; }
        // 名称
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 子智能体ID
        [JsonProperty("subRoleIds")]
        public List<int> SubRoleIds { get; set; } = new List<int>();
        // 模型ID
        [JsonProperty("modelId")]
        public int ModelId { get; set; } = 1;
        // token大小
        [JsonProperty("maxTokens")]
        public int MaxTokens { get; set; } = 4096;
        // 温度
        [JsonProperty("temperature")]
        public double Temperature { get; set; } = 0.7;
        // 思考
        [JsonProperty("thinking")]
        public bool Thinking { get; set; } = true;
        // 提示词列表
        [JsonProperty("prompts")]
        public List<int> Prompts { get; set; } = new List<int>();
        // 技能列表
        [JsonProperty("skills")]
        public List<int> Skills { get; set; } = new List<int>();
        // 工具列表
        [JsonProperty("tools")]
        public List<int> Tools { get; set; } = new List<int>();
        // 中间件列表
        [JsonProperty("middles")]
        public List<int> Middles { get; set; } = new List<int>();
        // 备注
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatModelInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("modelName")]
        public string ModelName { get; set; } = string.Empty;
        [JsonProperty("isSubAgent")]
        public bool IsSubAgent { get; set; } = true;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatPromptInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("promptValue")]
        public string PromptValue { get; set; } = string.Empty;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatSkillInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("skillName")]
        public string SkillName { get; set; } = string.Empty;
        [JsonProperty("confirm")]
        public bool Confirm { get; set; }
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatToolInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("toolName")]
        public string ToolName { get; set; } = string.Empty;
        [JsonProperty("confirm")]
        public bool Confirm { get; set; }
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatMiddleLayerInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("middleName")]
        public string MiddleName { get; set; } = string.Empty;
        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
    public class AgentChatConfigInfo
    {
        [JsonProperty("users")]
        public List<AgentChatUserInfo> Users { get; set; } = new List<AgentChatUserInfo>();
        [JsonProperty("roles")]
        public List<AgentChatRoleInfo> Roles { get; set; } = new List<AgentChatRoleInfo>();
        [JsonProperty("models")]
        public List<AgentChatModelInfo> Models { get; set; } = new List<AgentChatModelInfo>();
        [JsonProperty("prompts")]
        public List<AgentChatPromptInfo> Prompts { get; set; } = new List<AgentChatPromptInfo>();
        [JsonProperty("skills")]
        public List<AgentChatSkillInfo> Skills { get; set; } = new List<AgentChatSkillInfo>();
        [JsonProperty("tools")]
        public List<AgentChatToolInfo> Tools { get; set; } = new List<AgentChatToolInfo>();
        [JsonProperty("middles")]
        public List<AgentChatMiddleLayerInfo> Middles { get; set; } = new List<AgentChatMiddleLayerInfo>();
    }
    public class AgentChatSessionInfo
    {
        // 智能体ID
        [JsonProperty("agentId")]
        public string AgentId { get; set; } = string.Empty;
        // 智能体名称
        [JsonProperty("agentName")]
        public string AgentName { get; set; } = string.Empty;
        // 父智能体
        [JsonProperty("parentAgentId")]
        public string ParentAgentId { get; set; } = string.Empty;
        // 消息ID
        [JsonProperty("msgId")]
        public string MsgId { get; set; } = string.Empty;
        // 智能体消息
        [JsonIgnore]
        [JsonProperty("message")]
        public AgentChatMessageInfo Message { get; set; } = new AgentChatMessageInfo();
        // 角色信息
        [JsonProperty("config")]
        public AgentChatConfigInfo Config { get; set; } = new AgentChatConfigInfo();
        // 上下文
        [JsonProperty("request")]
        public LLMChatRequestInfo Request { get; set; } = new LLMChatRequestInfo();
    }
}
