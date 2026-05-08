using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Robot.WebApi.http;
using Robot.WebApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Robot.WebApi.ws
{
    public class WebSocketSession : IDisposable
    {
        private List<IWController> sessions_ { get; set; } = new();
        private WebSocket? webSocketSession_ { get; set; }
        private HttpSession? httpSession_ { get; set; }
        public bool Send(string data)
        {
            if (webSocketSession_ == null) return false;
            if (webSocketSession_.State != WebSocketState.Open) return false;
            byte[] buffer = Encoding.UTF8.GetBytes(data);
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
        public async Task ReceiveLoop(WebSocket webSocketSession, HttpSession httpSession, List<IWController> sessions)
        {
            try
            {
                webSocketSession_ = webSocketSession;
                httpSession_ = httpSession;
                sessions_.AddRange(sessions);
                foreach (var item in sessions_)
                {
                    item.SetHttpSession(httpSession);
                    item.SetWebSocketSession(webSocketSession_);
                }

                if (webSocketSession_ == null) return;

                var buffer = new byte[1024 * 4];

                using (var ms = new MemoryStream())
                {
                    while (webSocketSession_.State == WebSocketState.Open)
                    {
                        WebSocketReceiveResult result = await webSocketSession_.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close) { await webSocketSession_.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None); break; }

                        ms.Write(buffer, 0, result.Count);

                        if (result.EndOfMessage)
                        {
                            var messageBytes = ms.ToArray();

                            string message = Encoding.UTF8.GetString(messageBytes);

                            if (!HandleMessage(message)) return;

                            ms.SetLength(0);
                        }
                    }
                }
            }
            catch (WebSocketException wse) when (wse.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                // 客户端提前关闭连接
                Console.WriteLine($"WebSocket连接被客户端意外关闭: {wse.Message}");
            }
            catch (OperationCanceledException)
            {
                // 操作被取消
                Console.WriteLine("WebSocket操作被取消");
            }
            catch (IOException)
            {
                // 客户端重置连接
                Console.WriteLine("客户端重置了连接");
            }
            finally
            {
                webSocketSession?.Dispose();
            }
        }
        private bool HandleMessage(string message)
        {
            var result = HttpParseQueryString(message);
            if (string.IsNullOrWhiteSpace(result.Item1)) return false;

            if (result.Item2.ContainsKey("_SESSIONS_"))
            {
                httpSession_?.Update(result.Item2["_SESSIONS_"]);
                result.Item2.Remove("_SESSIONS_");
            }

            if (httpSession_ == null || !httpSession_.IsAuthorize()) 
            { 
                return false; 
            }

            try
            {
                foreach (var obj in sessions_)
                {
                    var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.GetCustomAttribute<WRouteAttribute>() != null); ;
                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<WRouteAttribute>();
                        if (attr == null) continue;
                        if (attr.route != result.Item1) continue;

                        List<object> methodParameters = new();
                        var parameters = method.GetParameters();
                        foreach (var parameter in parameters)
                        {
                            if (!result.Item2.ContainsKey(parameter.Name ?? "-")) continue;
                            if (parameter.ParameterType == typeof(int))
                            {
                                methodParameters.Add(int.Parse(result.Item2[parameter.Name ?? "-"]));
                            }
                            else if (parameter.ParameterType == typeof(string))
                            {
                                methodParameters.Add(result.Item2[parameter.Name ?? "-"]);
                            }
                            else if (parameter.ParameterType == typeof(long))
                            {
                                methodParameters.Add(long.Parse(result.Item2[parameter.Name ?? "-"]));
                            }
                            else if (parameter.ParameterType == typeof(double))
                            {
                                methodParameters.Add(double.Parse(result.Item2[parameter.Name ?? "-"]));
                            }
                            else if (parameter.ParameterType == typeof(bool))
                            {
                                methodParameters.Add(result.Item2[parameter.Name ?? "-"] == "true");
                            }
                            else if (parameter.ParameterType.IsClass)
                            {
                                methodParameters.Add(JsonConvert.DeserializeObject(result.Item2[parameter.Name ?? "-"], parameter.ParameterType) ?? new object());
                            }
                        }
                        object? v = method.Invoke(obj, methodParameters.ToArray());
                        if (v == null) return true;
                        if (v.GetType() == typeof(int))
                        {
                            Send(new WResult<int> { data = (int)v, route = result.Item1 });
                        }
                        else if (v.GetType() == typeof(string))
                        {
                            Send(new WResult<string> { data = (string)v, route = result.Item1 });
                        }
                        else if (v.GetType() == typeof(long))
                        {
                            Send(new WResult<long> { data = (long)v, route = result.Item1 });
                        }
                        else if (v.GetType() == typeof(double))
                        {
                            Send(new WResult<double> { data = (double)v, route = result.Item1 });
                        }
                        else if (v.GetType() == typeof(bool))
                        {
                            Send(new WResult<bool> { data = (bool)v, route = result.Item1 });
                        }
                        else if (v.GetType().IsClass)
                        {
                            var route = v.GetType().GetProperty("Route");
                            if (route != null)
                            {
                                route.SetValue(v, result.Item1, null);
                            }
                            Send(JsonConvert.SerializeObject(v));
                        }
                        return true;
                    }
                }
            }
            catch
            {
                Send(new WResult { code = -1, message = "未知异常!", route = result.Item1 });
            }
            return false;
        }
        private (string, Dictionary<string, string>) HttpParseQueryString(string url)
        {
            int index = url.IndexOf('?');
            if (index == -1) return (url, new());

            string route = url.Substring(0, index);
            string query = url.Substring(index + 1);
            var parameters = HttpUtility.ParseQueryString(query);
            if (parameters != null)
            {
                Dictionary<string, string> items = new();
                foreach (var item in parameters)
                {
                    if (item == null || null == (string?)item) continue;
                    items[(string)item] = ((string?)parameters[(string)item]) ?? "";
                }
                return (route, items);
            }
            else
            {
                return (route, new());
            }
        }
        public void Dispose()
        {
        }
    }
}
