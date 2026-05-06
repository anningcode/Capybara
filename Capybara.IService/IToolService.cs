using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IToolService
    {
        WebResponseInfo<AgentChatToolInfo> Select(int? id, string? toolName, bool? enable);
        AgentChatToolInfo? Select(int id);
        bool Update(AgentChatToolInfo model);
        bool Insert(AgentChatToolInfo model);
        bool Delete(int id);
    }
}
