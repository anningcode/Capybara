using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentRole
    {
        public AgentChatRoleInfo? GetRole(AgentChatUserInfo user)
        {
            return GetRole(user.RoleId);
        }
        public AgentChatRoleInfo? GetRole(int agentId)
        {
            var agents = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("id", agentId));
            if (agents.Count != 1) return null;
            return agents[0];
        }
        public AgentChatRoleInfo? GetRole(string agentStrId)
        {
            var agents = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("strId", agentStrId));
            if (agents.Count != 1) return null;
            return agents[0];
        }
    }
}
