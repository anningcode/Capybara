using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentPrompt
    {
        public List<AgentChatPromptInfo> GetPrompts(List<int> promptIds)
        {
            return AgentConfigManager.GetConfig<AgentChatPromptInfo>("prompts", ("id", promptIds));
        }
    }
}
