using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IMCPService
    {
        WebResponseInfo<WebMCPConfigInfo> Select(int? id, bool? enable);
        WebMCPConfigInfo? Select(int id);
        bool Update(WebMCPConfigInfo value);
        bool Insert(WebMCPConfigInfo value);
        bool Delete(int id);
    }
}
