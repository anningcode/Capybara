using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IRoleService
    {
        WebResponseInfo<AgentChatRoleInfo> Select(int? id, string? name, int? modelId,bool? thinking, bool? enable);
        AgentChatRoleInfo? Select(int id);
        bool Update(AgentChatRoleInfo model);
        bool Insert(AgentChatRoleInfo model);
        bool Delete(int id);
    }
}
