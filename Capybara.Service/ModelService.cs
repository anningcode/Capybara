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
    public class ModelService : IModelService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<AgentChatModelInfo>>("models");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("models", values);
            }
            return result;
        }

        public bool Insert(AgentChatModelInfo value)
        {
            var values = AppConfig.Get<List<AgentChatModelInfo>>("models") ?? new();
            value.Id = CommonHelper.FindSmallestMissingPositive(values.Select(n => n.Id).ToList());
            values.Add(value);
            AppConfig.Set("models", values);
            return true;
        }

        public AgentChatModelInfo? Select(int id)
        {
            var values = AppConfig.Get<List<AgentChatModelInfo>>("models") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<AgentChatModelInfo> Select(int? id, string? name, bool? isSubAgent, bool? enable)
        {
            WebResponseInfo<AgentChatModelInfo> result = new();
            var values = AppConfig.Get<List<AgentChatModelInfo>>("models") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (name == null || n.ModelName == name) &&
            (isSubAgent == null || n.IsSubAgent == isSubAgent) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatModelInfo value)
        {
            var values = AppConfig.Get<List<AgentChatModelInfo>>("models") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.ModelName = value.ModelName;
            result.IsSubAgent = value.IsSubAgent;
            result.Remarks = value.Remarks;
            AppConfig.Set("models", values);
            return true;
        }
    }
}
