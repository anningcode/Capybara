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
    public class PromptService : IPromptService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<AgentChatPromptInfo>>("prompts");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("prompts", values);
            }
            return result;
        }

        public bool Insert(AgentChatPromptInfo value)
        {
            var values = AppConfig.Get<List<AgentChatPromptInfo>>("prompts") ?? new();
            int id = 1;
            if (values.Count > 0)
            {
                id = values[values.Count - 1].Id + 1;
            }
            value.Id = id;
            values.Add(value);
            AppConfig.Set("prompts", values);
            return true;
        }

        public AgentChatPromptInfo? Select(int id)
        {
            var values = AppConfig.Get<List<AgentChatPromptInfo>>("prompts") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<AgentChatPromptInfo> Select(int? id, string? prompt, bool? enable)
        {
            WebResponseInfo<AgentChatPromptInfo> result = new();
            var values = AppConfig.Get<List<AgentChatPromptInfo>>("prompts") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (prompt == null || n.PromptValue == prompt) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatPromptInfo value)
        {
            var values = AppConfig.Get<List<AgentChatPromptInfo>>("prompts") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.PromptValue = value.PromptValue;
            result.Remarks = value.Remarks;
            AppConfig.Set("prompts", values);
            return true;
        }
    }
}
