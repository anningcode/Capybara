using Capybara.IService;
using Capybara.Models;
using Capybara.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Service
{
    public class RoleService : IRoleService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<AgentChatRoleInfo>>("roles");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("roles", values);
            }
            return result;
        }

        public bool Insert(AgentChatRoleInfo value)
        {
            var values = AppConfig.Get<List<AgentChatRoleInfo>>("roles") ?? new();
            int id = 1;
            if (values.Count > 0)
            {
                id = values[values.Count - 1].Id + 1;
            }
            value.Id = id;
            values.Add(value);
            AppConfig.Set("roles", values);
            return true;
        }

        public List<AgentChatRoleInfo> Select()
        {
            return AppConfig.Get<List<AgentChatRoleInfo>>("roles") ?? new();
        }

        public AgentChatRoleInfo? Select(int id)
        {
            var values = AppConfig.Get<List<AgentChatRoleInfo>>("roles") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<AgentChatRoleInfo> Select(int? id, string? name, int? modelId, bool? thinking, bool? enable)
        {
            WebResponseInfo<AgentChatRoleInfo> result = new();
            var values = AppConfig.Get<List<AgentChatRoleInfo>>("roles") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (name == null || n.Name == name) &&
            (modelId == null || n.ModelId == modelId) &&
            (thinking == null || n.Thinking == thinking) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatRoleInfo value)
        {
            var values = AppConfig.Get<List<AgentChatRoleInfo>>("roles") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.Name = value.Name;
            result.SubRoleIds = value.SubRoleIds;
            result.ModelId = value.ModelId;
            result.MaxTokens = value.MaxTokens;
            result.Temperature = value.Temperature;
            result.Thinking = value.Thinking;
            result.Prompts = value.Prompts;
            result.Skills = value.Skills;
            result.Tools = value.Tools;
            result.Middles = value.Middles;
            result.Remarks = value.Remarks;
            AppConfig.Set("roles", values);
            return true;
        }
    }
}
