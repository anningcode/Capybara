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
        bool Update(AgentChatToolInfo value);
        bool Insert(AgentChatToolInfo value);
        bool Delete(int id);
    }
}
