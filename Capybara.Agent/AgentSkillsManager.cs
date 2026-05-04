using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public static class AgentSkillsManager
    {
        // 加载技能列表
        public static List<string> GetSkills(List<string> skills)
        {
            List<string> result = new List<string>();
            if(!File.Exists("./capybara/skills/SKILL.md")) return new List<string>();
            foreach (var skill in skills)
            {
                string path = $"./capybara/skills/{skill}/SKILL.md";
                if (!File.Exists(path)) continue;
                string value = GetSkillDetail(path);
                if (string.IsNullOrEmpty(value)) continue;
                result.Add(value);
            }
            if (result.Count == 0) return new List<string>();
            string skillDescriptio = GetSkillDescription("./capybara/skills/SKILL.md");
            if(string.IsNullOrEmpty(skillDescriptio)) return new List<string>();
            return new List<string> { skillDescriptio, "## 技能列表(Skills) \n " + string.Join("\n", result) };
        }
        // 加载技能描述
        private static string GetSkillDescription(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return "";
            }
        }
        // 加载技能详情
        private static string GetSkillDetail(string path)
        {
            try
            {
                string result = "";
                int status = 0;
                int type = 0;
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (status < 10)
                        {
                            if (line == "---") type++;
                            if (type == 1 || type == 2) { status++; result += line + "\n"; }
                            if (type == 2) break;
                        }
                    }
                }
                return result;
            }
            catch
            {
                return "";
            }
        }
    }
}
