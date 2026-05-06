using Newtonsoft.Json;
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
    public abstract class WController : IWController
    {
        private WebSocket? webSocketSession_ { get; set; }
        private HttpSession? httpSession_ { get; set; }
        public bool Send(string msg)
        {
            if (webSocketSession_ == null) return false;
            if (webSocketSession_.State != WebSocketState.Open) return false;
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            ArraySegment<byte> segment = new(buffer);
            webSocketSession_?.SendAsync(segment, WebSocketMessageType.Text, true, default);
            return true;
        }
        public bool Send(WResult msg)
        {
            string data = JsonConvert.SerializeObject(msg);
            return Send(data);
        }
        public bool Send<T>(WResult<T> msg)
        {
            string data = JsonConvert.SerializeObject(msg);
            return Send(data);
        }
        public HttpSession? GetSession()
        {
            return httpSession_;
        }
        public void SetHttpSession(HttpSession? session)
        {
            httpSession_ = session;
        }
        public void SetWebSocketSession(WebSocket? session)
        {
            webSocketSession_ = session;
        }
    }
}
