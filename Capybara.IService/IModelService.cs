using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IModelService
    {
        WebResponseInfo<AgentChatModelInfo> Select(int? id, string? name, bool? isSubAgent, bool? enable);
        AgentChatModelInfo? Select(int id);
        bool Update(AgentChatModelInfo model);
        bool Insert(AgentChatModelInfo model);
        bool Delete(int id);
    }
}
