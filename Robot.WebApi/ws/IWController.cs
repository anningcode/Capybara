using Robot.WebApi.http;
using Robot.WebApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.ws
{
    public interface IWController
    {
        bool Send(string msg);
        bool Send(WResult msg);
        bool Send<T>(WResult<T> msg);
        HttpSession? GetSession();
        void SetHttpSession(HttpSession? session);
        void SetWebSocketSession(WebSocket? session);
    }
}
