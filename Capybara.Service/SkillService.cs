using Capybara.IService;
using Capybara.Models;
using Capybara.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Capybara.Service
{
    public class SkillService : ISkillService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<AgentChatSkillInfo>>("skills");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("skills", values);
            }
            return result;
        }

        public bool Insert(AgentChatSkillInfo value)
        {
            var values = AppConfig.Get<List<AgentChatSkillInfo>>("skills") ?? new();
            int id = 1;
            if (values.Count > 0)
            {
                id = values[values.Count - 1].Id + 1;
            }
            value.Id = id;
            values.Add(value);
            AppConfig.Set("skills", values);
            return true;
        }

        public AgentChatSkillInfo? Select(int id)
        {
            var values = AppConfig.Get<List<AgentChatSkillInfo>>("skills") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<AgentChatSkillInfo> Select(int? id, string? skillName, bool? enable)
        {
            WebResponseInfo<AgentChatSkillInfo> result = new();
            var values = AppConfig.Get<List<AgentChatSkillInfo>>("skills") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (skillName == null || n.SkillName == skillName) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatSkillInfo value)
        {
            var values = AppConfig.Get<List<AgentChatSkillInfo>>("skills") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.SkillName = value.SkillName;
            result.Confirm = value.Confirm;
            result.Remarks = value.Remarks;
            AppConfig.Set("skills", values);
            return true;
        }
    }
}
