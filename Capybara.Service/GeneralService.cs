using Capybara.IService;
using Capybara.Models;
using Capybara.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Service
{
    public class GeneralService : IGeneralService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("generals", values);
            }
            return result;
        }

        public bool Insert(WebGeneralConfigInfo value)
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals") ?? new();
            int id = 1;
            if (values.Count > 0)
            {
                id = values[values.Count - 1].Id + 1;
            }
            value.Id = id;
            values.Add(value);
            AppConfig.Set("generals", values);
            return true;
        }

        public WebGeneralConfigInfo? Select(int id)
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public WebResponseInfo<WebGeneralConfigInfo> Select(int? id, string? key, string? value, bool? enable)
        {
            WebResponseInfo<WebGeneralConfigInfo> result = new();
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (key == null || n.Key == key) &&
            (value == null || n.Value == value) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public bool Update(WebGeneralConfigInfo value)
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.Key = value.Key;
            result.Value = value.Value;
            result.Enable = value.Enable;
            result.Remarks = value.Remarks;
            AppConfig.Set("generals", values);
            return true;
        }
    }
}
