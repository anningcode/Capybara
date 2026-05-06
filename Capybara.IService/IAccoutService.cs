using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.IService
{
    public interface IAccoutService
    {
        WebUserConfigInfo? Login(string code, string password);
    }
}
