using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public class AgentSkills
    {
        private AgentSkillsManager skillsManager_ { get; set; } = AgentSkillsManager.Instance;
        public List<AgentChatSkillInfo> GetSkills(List<string> skills)
        {
            return AgentConfigManager.GetConfig<AgentChatSkillInfo>("skills", ("skillName", skills));
        }
        public List<AgentChatSkillInfo> GetSkills(List<int> tools)
        {
            return AgentConfigManager.GetConfig<AgentChatSkillInfo>("skills", ("id", tools));
        }
        public List<string> GetSkills(List<AgentChatSkillInfo> skills)
        {
            return skillsManager_.GetSkills(skills.Select(t => t.SkillName).ToList());
        }
    }
}
