using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IPromptService
    {
        WebResponseInfo<AgentChatPromptInfo> Select(int? id, string ? prompt, bool? enable);
        AgentChatPromptInfo? Select(int id);
        bool Update(AgentChatPromptInfo value);
        bool Insert(AgentChatPromptInfo value);
        bool Delete(int id);
    }
}
