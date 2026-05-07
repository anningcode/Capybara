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
    public class MCPService : IMCPService
    {
        public bool Delete(int id)
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("mcps");
            if (values == null) return false;
            bool result = values.RemoveAll(n => n.Id == id) > 0;
            if (result)
            {
                AppConfig.Set("mcps", values);
            }
            return result;
        }

        public bool Insert(WebMCPConfigInfo value)
        {
            var values = AppConfig.Get<List<WebMCPConfigInfo>>("mcps") ?? new();
            value.Id = CommonHelper.FindSmallestMissingPositive(values.Select(n => n.Id).ToList());
            values.Add(value);
            AppConfig.Set("mcps", values);
            return true;
        }

        public WebResponseInfo<WebMCPConfigInfo> Select(int? id, bool? enable)
        {
            WebResponseInfo<WebMCPConfigInfo> result = new();
            var values = AppConfig.Get<List<WebMCPConfigInfo>>("mcps") ?? new();
            result.Data = values.Where(n =>
            (id == null || n.Id == id) &&
            (enable == null || n.Enable == enable)).ToList();
            result.Count = result.Data.Count;
            return result;
        }

        public WebMCPConfigInfo? Select(int id)
        {
            var values = AppConfig.Get<List<WebMCPConfigInfo>>("mcps") ?? new();
            return values.FirstOrDefault(n => n.Id == id);
        }

        public bool Update(WebMCPConfigInfo value)
        {
            var values = AppConfig.Get<List<WebMCPConfigInfo>>("mcps") ?? new();
            var result = values.FirstOrDefault(n => n.Id == value.Id);
            if (result == null) return false;
            result.Endpoint = value.Endpoint;
            result.AppKey = value.AppKey;
            result.Enable = value.Enable;
            result.Remarks = value.Remarks;
            AppConfig.Set("mcps", values);
            return true;
        }
    }
}
