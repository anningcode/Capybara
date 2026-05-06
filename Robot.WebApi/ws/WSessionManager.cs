using Newtonsoft.Json;
using Robot.WebApi.http;
using Robot.WebApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.ws
{
    public class WSessionManager
    {
        public static Dictionary<string, WebSocketSession> sessions { get; set; } = new();
        public void Send(string msg, Func<HttpSession?, bool> callback)
        {
            Dictionary<string, WebSocketSession>? items;
            lock (sessions)
            {
                items = new Dictionary<string, WebSocketSession>(sessions);
            }
            foreach (var item in items)
            {
                if (callback(item.Value.GetSession()))
                {
                    item.Value.Send(msg);
                }
            }
        }
        public void Send(WResult msg, Func<HttpSession?, bool> callback)
        {
            Send(JsonConvert.SerializeObject(msg), callback);
        }
        public void Send<T>(WResult<T> msg, Func<HttpSession?, bool> callback)
        {
            Send(JsonConvert.SerializeObject(msg), callback);
        }
    }
}
