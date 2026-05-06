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
    public class UserService : IUserService
    {
        public bool Delete(string id)
        {
            var values = AppConfig.Get<List<AgentChatUserInfo>>("users");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("users", values);
            }
            return result;
        }
        public bool Insert(AgentChatUserInfo value)
        {
            var values = AppConfig.Get<List<AgentChatUserInfo>>("users") ?? new();
            value.Id = Guid.NewGuid().ToString("N");
            values.Add(value);
            AppConfig.Set("users", values);
            return true;
        }
        public AgentChatUserInfo? Select(string id)
        {
            var values = AppConfig.Get<List<AgentChatUserInfo>>("users") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<AgentChatUserInfo> Select(string? id, string? name, int? roleId, bool? enable)
        {
            WebResponseInfo<AgentChatUserInfo> result = new();
            var values = AppConfig.Get<List<AgentChatUserInfo>>("users") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (name == null || n.Name == name) &&
            (roleId == null || n.RoleId == roleId) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(AgentChatUserInfo value)
        {
            var values = AppConfig.Get<List<AgentChatUserInfo>>("users") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.RoleId = value.RoleId;
            result.Name = value.Name;
            result.Age = value.Age;
            result.Sex = value.Sex;
            result.Height = value.Height;
            result.Weight = value.Weight;
            result.Remarks = value.Remarks;
            AppConfig.Set("users", values);
            return true;
        }
    }
}
