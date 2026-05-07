using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IWebUserService
    {
        WebResponseInfo<WebUserConfigInfo> Select(int? id, string? name,string?code,bool?enable);
        WebUserConfigInfo? Select(int id);
        bool Update(WebUserConfigInfo value);
        bool Insert(WebUserConfigInfo value);
        bool Delete(int id);
    }
}
