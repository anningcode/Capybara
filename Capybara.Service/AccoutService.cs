using Capybara.IService;
using Capybara.Models;
using Capybara.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Service
{
    public class AccoutService : IAccoutService
    {
        public WebUserConfigInfo? Login(string code, string password)
        {
            var values = AppConfig.Get<List<WebUserConfigInfo>>("webusers");
            if(values == null) return null;
            return values.FirstOrDefault(n => n.Code == code && n.Password == password && n.Enable);
        }
    }
}
