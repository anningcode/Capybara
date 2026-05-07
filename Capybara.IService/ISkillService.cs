using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface ISkillService
    {
        WebResponseInfo<AgentChatSkillInfo> Select(int? id, string? skillName, bool? enable);
        AgentChatSkillInfo? Select(int id);
        bool Update(AgentChatSkillInfo value);
        bool Insert(AgentChatSkillInfo value);
        bool Delete(int id);
    }
}
