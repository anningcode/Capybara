using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IUserService
    {
        WebResponseInfo<AgentChatUserInfo> Select(string? id, string? name, int ?roleId, bool? enable);
        AgentChatUserInfo? Select(string id);
        bool Update(AgentChatUserInfo value);
        bool Insert(AgentChatUserInfo value);
        bool Delete(string id);
    }
}
