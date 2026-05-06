using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IGeneralService
    {
        WebResponseInfo<WebGeneralConfigInfo> Select(int? id, string? key,string ?value, bool? enable);
        WebGeneralConfigInfo? Select(int id);
        bool Update(WebGeneralConfigInfo model);
        bool Insert(WebGeneralConfigInfo model);
        bool Delete(int id);
    }
}
