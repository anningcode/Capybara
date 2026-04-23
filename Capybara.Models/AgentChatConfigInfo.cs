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
        public string id { get; set; } = string.Empty;
        // 关联智能体ID
        public int roleId { get; set; }
        // 名称
        public string name { get; set; } = string.Empty;
        // 年龄
        public int age { get; set; }
        // 性别 1:男,2:女
        public int sex { get; set; }
        // 身高
        public int height { get; set; }
        // 体重
        public int weight { get; set; }
        // 备注
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatRoleInfo
    {
        // ID
        public int id { get; set; }
        // 名称
        public string name { get; set; } = string.Empty;
        // 子智能体ID
        public List<int> subRoleIds { get; set; } = new List<int>();
        // llm地址
        public string llmAddress { get; set; } = string.Empty;
        // 模型ID
        public int modelId { get; set; } = 1;
        // token大小
        public int maxTokens { get; set; } = 4096;
        // 温度
        public double temperature { get; set; } = 0.7;
        // 思考
        public bool thinking { get; set; } = true;
        // 提示词列表
        public List<int> prompts { get; set; } = new List<int>();
        // 技能列表
        public List<int> skills { get; set; } = new List<int>();
        // 工具列表
        public List<int> tools { get; set; } = new List<int>();
        // 中间件列表
        public List<int> middles { get; set; } = new List<int>();
        // 备注
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatModelInfo
    {
        public int id { get; set; }
        public string modelName { get; set; } = string.Empty;
        public bool isSubAgent { get; set; } = true;
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatPromptInfo
    {
        public int id { get; set; }
        public string promptValue { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatSkillInfo
    {
        public int id { get; set; }
        public string skillName { get; set; } = string.Empty;
        public bool confirm { get; set; }
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatToolInfo
    {
        public int id { get; set; }
        public string toolName { get; set; } = string.Empty;
        public bool confirm { get; set; }
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatMiddleLayerInfo
    {
        public int id { get; set; }
        public string middleName { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
    }
    public class AgentChatConfigInfo
    {
        public List<AgentChatUserInfo> users { get; set; } = new List<AgentChatUserInfo>();
        public List<AgentChatRoleInfo> roles { get; set; } = new List<AgentChatRoleInfo>();
        public List<AgentChatModelInfo> models { get; set; } = new List<AgentChatModelInfo>();
        public List<AgentChatPromptInfo> prompts { get; set; } = new List<AgentChatPromptInfo>();
        public List<AgentChatSkillInfo> skills { get; set; } = new List<AgentChatSkillInfo>();
        public List<AgentChatToolInfo> tools { get; set; } = new List<AgentChatToolInfo>();
        public List<AgentChatMiddleLayerInfo> middles { get; set; } = new List<AgentChatMiddleLayerInfo>();
    }
    public class AgentChatSessionInfo
    {
        // 智能体ID
        public string agentId { get; set; } = string.Empty;
        // 智能体名称
        public string agentName { get; set; } = string.Empty;
        // 父智能体
        public string parentAgentId { get; set; } = string.Empty;
        // 消息ID
        public string msgId { get; set; } = string.Empty;
        // 智能体消息
        [JsonIgnore]
        public AgentChatMessageInfo message { get; set; } = new AgentChatMessageInfo();
        // 角色信息
        public AgentChatConfigInfo config { get; set; } = new AgentChatConfigInfo();
        // 上下文
        public AgentLLMRequestInfo request { get; set; } = new AgentLLMRequestInfo();
    }
}
