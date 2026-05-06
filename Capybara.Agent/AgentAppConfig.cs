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
            return AppConfig.Get<List<AgentChatUserInfo>>("users")?.FirstOrDefault(n => n.Id == userId && n.Enable);
        }
        // 获取所有模型
        public static List<AgentChatModelInfo> GetModel()
        {
            return AppConfig.Get<List<AgentChatModelInfo>>("models")?.Where(n=>n.Enable).ToList() ?? new();
        }
        // 获取模型
        public static AgentChatModelInfo? GetModel(int modelId)
        {
            return AppConfig.Get<List<AgentChatModelInfo>>("models")?.FirstOrDefault(n => n.Id == modelId && n.Enable);
        }
        // 获取提示词
        public static List<AgentChatPromptInfo> GetPrompts(List<int> promptIds)
        {
            return AppConfig.Get<List<AgentChatPromptInfo>>("prompts")?.Where(n => promptIds.Contains(n.Id) && n.Enable).ToList() ?? new();
        }
        // 获取角色
        public static AgentChatRoleInfo? GetRole(int agentId)
        {
            return AppConfig.Get<List<AgentChatRoleInfo>>("roles")?.FirstOrDefault(n => n.Id == agentId && n.Enable);
        }
        // 获取工具
        public static List<AgentChatToolInfo> GetTools(List<string> toolNames)
        {
            return AppConfig.Get<List<AgentChatToolInfo>>("tools")?.Where(n => toolNames.Contains(n.ToolName) && n.Enable).ToList() ?? new();
        }
        // 获取工具
        public static List<AgentChatToolInfo> GetTools(List<int> toolIds)
        {
            return AppConfig.Get<List<AgentChatToolInfo>>("tools")?.Where(n => toolIds.Contains(n.Id) && n.Enable).ToList() ?? new();
        }
        // 获取工具
        public static List<LLMToolDefinitionInfo> GetTools(List<AgentChatToolInfo> tools)
        {
            return AgentToolsManager.GetTools(tools.Where(n=>n.Enable).Select(t => t.ToolName).ToList());
        }
        // 获取skills
        public static List<AgentChatSkillInfo> GetSkills(List<string> skills)
        {
            return AppConfig.Get<List<AgentChatSkillInfo>>("skills")?.Where(n => skills.Contains(n.SkillName) && n.Enable).ToList() ?? new();
        }
        // 获取skills
        public static List<AgentChatSkillInfo> GetSkills(List<int> skills)
        {
            return AppConfig.Get<List<AgentChatSkillInfo>>("skills")?.Where(n => skills.Contains(n.Id) && n.Enable).ToList() ?? new();
        }
        // 获取skills
        public static List<string> GetSkills(List<AgentChatSkillInfo> skills)
        {
            return AgentSkillsManager.GetSkills(skills.Where(n => n.Enable).Select(t => t.SkillName).ToList());
        }
    }
}
