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
    public class WebUserService : IWebUserService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("webusers", values);
            }
            return result;
        }

        public bool Insert(WebUserConfigInfo value)
        {
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers") ?? new();
            int id = 1;
            if (values.Count > 0)
            {
                id = values[values.Count - 1].Id + 1;
            }
            value.Id = id;
            values.Add(value);
            AppConfig.Set("webusers", values);
            return true;
        }

        public WebUserConfigInfo? Select(int id)
        {
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<WebUserConfigInfo> Select(int? id, string? name, string? code, bool? enable)
        {
            WebResponseInfo<WebUserConfigInfo> result = new();
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (name == null || n.Name == name) &&
            (code == null || n.Code == code) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(WebUserConfigInfo value)
        {
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.Name = value.Name;
            result.Code = value.Code;
            //result.Password = value.Password;
            result.Enable = value.Enable;
            result.Remarks = value.Remarks;
            AppConfig.Set("webusers", values);
            return true;
        }
    }
}
