using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public class AgentTools
    {
        private AgentToolsManager toolsManager_ { get; set; } = AgentToolsManager.Instance;
        public List<AgentChatToolInfo> GetTools(List<string> toolNames)
        {
            return AgentConfigManager.GetConfig<AgentChatToolInfo>("tools", ("toolName", toolNames));
        }
        public List<AgentChatToolInfo> GetTools(List<int> toolIds)
        {
            return AgentConfigManager.GetConfig<AgentChatToolInfo>("tools", ("id", toolIds));
        }
        public List<AgentLLMToolCallsRequestInfo> GetTools(List<AgentChatToolInfo> tools)
        {
            return toolsManager_.GetTools(tools.Select(t => t.ToolName).ToList());
        }
    }
}
