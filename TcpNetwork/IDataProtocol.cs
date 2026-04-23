using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetwork
{
    public interface IDataProtocol
    {
        (int, bool) MatchRole(string buffer);
    }
}
