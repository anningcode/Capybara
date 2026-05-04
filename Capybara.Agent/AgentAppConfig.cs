using Capybara.Models;
using Capybara.Utils;
using LLMGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    internal static class AgentAppConfig
    {
        // 获取用户信息
        public static AgentChatUserInfo? GetUser(string userId)
        {
            return AppConfig.Get<List<AgentChatUserInfo>>("users")?.FirstOrDefault(n => n.Id == userId);
        }
        // 获取所有模型
        public static List<AgentChatModelInfo> GetModel()
        {
            return AppConfig.Get<List<AgentChatModelInfo>>("models") ?? new();
        }
        // 获取模型
        public static AgentChatModelInfo? GetModel(int modelId)
        {
            return AppConfig.Get<List<AgentChatModelInfo>>("models")?.FirstOrDefault(n => n.Id == modelId);
        }
        // 获取提示词
        public static List<AgentChatPromptInfo> GetPrompts(List<int> promptIds)
        {
            return AppConfig.Get<List<AgentChatPromptInfo>>("prompts")?.Where(n => promptIds.Contains(n.Id)).ToList() ?? new();
        }
        // 获取角色
        public static AgentChatRoleInfo? GetRole(int agentId)
        {
            return AppConfig.Get<List<AgentChatRoleInfo>>("roles")?.FirstOrDefault(n => n.Id == agentId);
        }
        // 获取工具
        public static List<AgentChatToolInfo> GetTools(List<string> toolNames)
        {
            return AppConfig.Get<List<AgentChatToolInfo>>("tools")?.Where(n => toolNames.Contains(n.ToolName)).ToList() ?? new();
        }
        // 获取工具
        public static List<AgentChatToolInfo> GetTools(List<int> toolIds)
        {
            return AppConfig.Get<List<AgentChatToolInfo>>("tools")?.Where(n => toolIds.Contains(n.Id)).ToList() ?? new();
        }
        // 获取工具
        public static List<LLMToolDefinitionInfo> GetTools(List<AgentChatToolInfo> tools)
        {
            return AgentToolsManager.GetTools(tools.Select(t => t.ToolName).ToList());
        }
        // 获取skills
        public static List<AgentChatSkillInfo> GetSkills(List<string> skills)
        {
            return AppConfig.Get<List<AgentChatSkillInfo>>("skills")?.Where(n => skills.Contains(n.SkillName)).ToList() ?? new();
        }
        // 获取skills
        public static List<AgentChatSkillInfo> GetSkills(List<int> skills)
        {
            return AppConfig.Get<List<AgentChatSkillInfo>>("skills")?.Where(n => skills.Contains(n.Id)).ToList() ?? new();
        }
        // 获取skills
        public static List<string> GetSkills(List<AgentChatSkillInfo> skills)
        {
            return AgentSkillsManager.GetSkills(skills.Select(t => t.SkillName).ToList());
        }
    }
}
