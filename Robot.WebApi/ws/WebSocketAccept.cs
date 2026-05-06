using Microsoft.AspNetCore.Http;
using Robot.WebApi.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.ws
{
    public abstract class WebSocketAccept
    {
        protected readonly WAutofacThreadHelper autofacThreadHelper_;
        protected readonly RequestDelegate next_;
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper)
        {
            next_ = next;
            autofacThreadHelper_ = autofacThreadHelper;
        }
        public virtual async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
        }
        protected object? CreateInstance<T>() where T : class, new()
        {
            using var scope = autofacThreadHelper_.BeginNewScope();

            Type type = typeof(T);
            List<object> param = new List<object>();
            ConstructorInfo[] constructors = type.GetConstructors();
            foreach (var ctor in constructors)
            {
                ParameterInfo[] parameters = ctor.GetParameters();
                if (parameters.Length != 1) continue;
                
                object? constructorValue = scope.ServiceProvider.GetService(parameters[0].ParameterType);
                if (constructorValue == null) continue;
                param.Add(constructorValue);
                break;
            }

            return Activator.CreateInstance(type, param.ToArray());
        }
    }
    public class WebSocketAccept<T1> :
        WebSocketAccept
        where T1 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5, T6> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
        where T6 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            object? t6 = CreateInstance<T6>();
            if (t6 != null) controllers.Add((T6)t6);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5, T6, T7> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
        where T6 : class, IWController, new()
        where T7 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            object? t6 = CreateInstance<T6>();
            if (t6 != null) controllers.Add((T6)t6);
            object? t7 = CreateInstance<T7>();
            if (t7 != null) controllers.Add((T7)t7);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5, T6, T7, T8> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
        where T6 : class, IWController, new()
        where T7 : class, IWController, new()
        where T8 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            object? t6 = CreateInstance<T6>();
            if (t6 != null) controllers.Add((T6)t6);
            object? t7 = CreateInstance<T7>();
            if (t7 != null) controllers.Add((T7)t7);
            object? t8 = CreateInstance<T8>();
            if (t8 != null) controllers.Add((T8)t8);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5, T6, T7, T8, T9> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
        where T6 : class, IWController, new()
        where T7 : class, IWController, new()
        where T8 : class, IWController, new()
        where T9 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            object? t6 = CreateInstance<T6>();
            if (t6 != null) controllers.Add((T6)t6);
            object? t7 = CreateInstance<T7>();
            if (t7 != null) controllers.Add((T7)t7);
            object? t8 = CreateInstance<T8>();
            if (t8 != null) controllers.Add((T8)t8);
            object? t9 = CreateInstance<T9>();
            if (t9 != null) controllers.Add((T9)t9);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
    public class WebSocketAccept<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> :
        WebSocketAccept
        where T1 : class, IWController, new()
        where T2 : class, IWController, new()
        where T3 : class, IWController, new()
        where T4 : class, IWController, new()
        where T5 : class, IWController, new()
        where T6 : class, IWController, new()
        where T7 : class, IWController, new()
        where T8 : class, IWController, new()
        where T9 : class, IWController, new()
        where T10 : class, IWController, new()
    {
        public WebSocketAccept(RequestDelegate next, WAutofacThreadHelper autofacThreadHelper) : base(next, autofacThreadHelper)
        {
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next_.Invoke(context); return;
            }
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);
            if (session == null || !session.IsAuthorize()) return;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            WebSocketSession value = new WebSocketSession();
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Add(key, value);
            }
            List<IWController> controllers = new List<IWController>();
            object? t1 = CreateInstance<T1>();
            if (t1 != null) controllers.Add((T1)t1);
            object? t2 = CreateInstance<T2>();
            if (t2 != null) controllers.Add((T2)t2);
            object? t3 = CreateInstance<T3>();
            if (t3 != null) controllers.Add((T3)t3);
            object? t4 = CreateInstance<T4>();
            if (t4 != null) controllers.Add((T4)t4);
            object? t5 = CreateInstance<T5>();
            if (t5 != null) controllers.Add((T5)t5);
            object? t6 = CreateInstance<T6>();
            if (t6 != null) controllers.Add((T6)t6);
            object? t7 = CreateInstance<T7>();
            if (t7 != null) controllers.Add((T7)t7);
            object? t8 = CreateInstance<T8>();
            if (t8 != null) controllers.Add((T8)t8);
            object? t9 = CreateInstance<T9>();
            if (t9 != null) controllers.Add((T9)t9);
            object? t10 = CreateInstance<T10>();
            if (t10 != null) controllers.Add((T10)t10);
            await value.ReceiveLoop(webSocket, session, controllers);
            lock (WSessionManager.sessions)
            {
                WSessionManager.sessions.Remove(key);
            }
            value.Dispose();
        }
    }
}
