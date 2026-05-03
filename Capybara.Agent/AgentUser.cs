using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentUser
    {
        public AgentChatUserInfo? GetUser(string userId)
        {
            var users = AgentConfigManager.GetConfig<AgentChatUserInfo>("users", ("id", userId));
            if (users.Count != 1) return null;
            return users[0];
        }
    }
}
