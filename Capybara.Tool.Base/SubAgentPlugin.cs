using Capybara.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class SubAgentPlugin : IToolPlugin
    {
        [AgentFunction("create_sub_agent")]
        [Description("创建子智能体")]
        public string CreateSubAgent(
            [Description("智能体名称")] string agentName,
            [Description("提示词")][NoRequired] string prompt,
            [Description("问题")] string content,
            [Description("回答温度")] double temperature,
            [Description("最大token")] int maxTokens,
            [Description("模型名称")] string model,
            [Description("技能列表")][NoRequired] List<string> skills,
            [Description("工具列表")][NoRequired] List<string> tools)
        {
            return string.Empty;
        }

        [AgentFunction("load_sub_agent")]
        [Description("加载子智能体")]
        public string LoadSubAgent(
            [Description("使用用户提供的角色ID作为参数")] int id, 
            [Description("问题")] string content)
        {
            return string.Empty;
        }

        [AgentFunction("reuse_sub_agent")]
        [Description("复用子智能体")]
        public string ReuseSubAgent(
            [Description("要复用的子智能体ID,加载或创建的子智能体ID")] string agentId,
            [Description("问题")] string content) 
        {
            return string.Empty;
        }

        [AgentFunction("wait_for_agents")]
        [Description("等待子智能体结果,配合create_sub_agent、load_sub_agent、reuse_sub_agent一起使用,")]
        public string WaitForAgent()
        {
            return string.Empty;
        }
    }
}
