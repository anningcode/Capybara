using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentModel
    {
        public AgentChatModelInfo? GetModel(int modelId)
        {
            var models = AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("id", modelId));
            if (models.Count != 1) return null;
            return models[0];
        }
        public List<AgentChatModelInfo> GetModels()
        {
            return AgentConfigManager.GetConfig<AgentChatModelInfo>("models");
        }
        public List<AgentChatModelInfo> GetSubAgentModels()
        {
            return AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("isSubAgent", true));
        }
    }
}
