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
    public class ToolService : IToolService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<AgentChatToolInfo>>("tools");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("tools", values);
            }
            return result;
        }

        public bool Insert(AgentChatToolInfo value)
        {
            var values = AppConfig.Get<List<AgentChatToolInfo>>("tools") ?? new();
            value.Id = CommonHelper.FindSmallestMissingPositive(values.Select(n => n.Id).ToList());
            values.Add(value);
            AppConfig.Set("tools", values);
            return true;
        }

        public AgentChatToolInfo? Select(int id)
        {
            var values = AppConfig.Get<List<AgentChatToolInfo>>("tools") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }
        public WebResponseInfo<AgentChatToolInfo> Select(int? id, string? toolName, bool? enable)
        {
            WebResponseInfo<AgentChatToolInfo> result = new();
            var values = AppConfig.Get<List<AgentChatToolInfo>>("tools") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (toolName == null || n.ToolName == toolName) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatToolInfo value)
        {
            var values = AppConfig.Get<List<AgentChatToolInfo>>("tools") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.ToolName = value.ToolName;
            result.Confirm = value.Confirm;
            result.Remarks = value.Remarks;
            AppConfig.Set("tools", values);
            return true;
        }
    }
}
