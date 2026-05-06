using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.http
{
    public class HttpSession
    {
        private static SessionKeyManager sessionKeyManager_ = new();
        private long requestTime_ { get; set; } = 0;
        // 请求时间
        [JsonProperty]
        private long requestTime
        {
            get
            {
                if (requestTime_ == 0)
                {
                    requestTime_ = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
                return requestTime_;
            }
            set
            {
                requestTime_ = value;
            }
        }
        // 是否授权
        [JsonProperty]
        private bool isAuth { get; set; } = false;
        // 会话管理
        [JsonProperty]
        private Dictionary<string, object> sessions { get; set; } = new();
        public static HttpSession? GetSession(string sessionStr)
        {
            HttpSession result = new HttpSession();
            try
            {
                (bool ok, string data) = sessionKeyManager_.GetDecryptValue(sessionStr);
                if (!ok) return null;
                var session = JsonConvert.DeserializeObject<HttpSession>(data);
                if (session == null) return null;

                if (session.requestTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds() - SessionKeyManager.minute_ * 60)
                {
                    session.isAuth = false;
                }
                result.requestTime = session.requestTime;
                result.isAuth = session.isAuth;
                result.sessions = session.sessions;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public static HttpSession GetSession(HttpRequest context)
        {
            return GetSession(context.HttpContext);
        }
        public static HttpSession GetSession(ActionExecutingContext context)
        {
            return GetSession(context.HttpContext);
        }
        public static HttpSession GetSession(HttpContext context)
        {
            return (HttpSession)context.Items["_SESSIONS_"];
        }
        public static bool IsSession(HttpContext context)
        {
            return context.Items.ContainsKey("_SESSIONS_");
        }
        public static bool IsSession(ActionExecutingContext context)
        {
            return IsSession(context.HttpContext);
        }
        public static void UpdateSession(ActionExecutedContext context)
        {
            HttpSession session = (HttpSession)context.HttpContext.Items["_SESSIONS_"];
            session.requestTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string json = JsonConvert.SerializeObject(session);
            string sessionValue = sessionKeyManager_.GetEncryptValue(json);
            context.HttpContext.Response.Headers.Add("_SESSIONS_", sessionValue);
            context.HttpContext.Response.Cookies.Append("_SESSIONS_", sessionValue);
        }
        public HttpSession() { }
        public HttpSession(ActionExecutingContext context)
        {
            CreateSession(context.HttpContext);
        }
        public HttpSession(HttpContext context)
        {
            CreateSession(context);
        }
        public bool Update(string sessionStr)
        {
            try
            {
                (bool ok, string data) = sessionKeyManager_.GetDecryptValue(sessionStr);
                if (!ok) return false;
                var session = JsonConvert.DeserializeObject<HttpSession>(data);
                if (session == null) return false;
                requestTime = session.requestTime;
                isAuth = session.isAuth;
                sessions = session.sessions;
                return true;
            }
            catch { }
            return false;
        }
        public void CreateSession(HttpContext context)
        {
            while (true)
            {
                try
                {
                    string? json = null;
                    if (context.Request.Headers.ContainsKey("_SESSIONS_"))
                    {
                        json = context.Request.Headers["_SESSIONS_"];
                    }
                    else if (context.Request.Query.ContainsKey("_SESSIONS_"))
                    {
                        json = context.Request.Query["_SESSIONS_"];
                        json = json?.Replace(' ', '+');
                    }
                    else if (context.Request.Cookies.ContainsKey("_SESSIONS_"))
                    {
                        json = context.Request.Cookies["_SESSIONS_"];
                    }
                    if (json == null) break;
                    (bool ok, string data) = sessionKeyManager_.GetDecryptValue(json);
                    if (!ok) break;
                    var session = JsonConvert.DeserializeObject<HttpSession>(data);
                    if (session == null) break;
                    requestTime = session.requestTime;
                    isAuth = session.isAuth;
                    sessions = session.sessions;
                }
                catch { }
                break;
            }
            context.Items["_SESSIONS_"] = this;
        }
        public object this[string key]
        {
            get
            {
                return sessions[key];
            }
            set
            {
                sessions[key] = value;
            }
        }
        public bool ContainsKey(string key)
        {
            return sessions.ContainsKey(key);
        }
        public void Clear()
        {
            sessions.Clear();
        }
        public bool IsAuthorize()
        {
            return isAuth;
        }
        public void SetAuthorize(bool value = true)
        {
            isAuth = value;
        }
    }
}
