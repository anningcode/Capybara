using Capybara.IEntrance;
using Capybara.Models;
using Newtonsoft.Json;
using NLog;
using TcpNetwork;

namespace Capybara.TcpEntrance
{
    public class TcpDataProtocol : IDataProtocol
    {
        public (int, bool) MatchRole(string buffer)
        {
            int index = 0;
            while (buffer.Length > index)
            {
                if (buffer[index] == '\n')
                    return (++index, true);
                index++;
            }
            return (0, true);
        }
    }
    public class ChatTcpEntrance : IChatEntrance
    {
        private static Logger logger_ = LogManager.Setup().LoadConfigurationFromFile("config/nlog.config").GetCurrentClassLogger();
        private TcpListenerNetwork server_ { get; set; } = new TcpListenerNetwork();
        public Action<AgentChatMessageInfo>? onRequest { get; set; }
        public ChatTcpEntrance(string param)
        {
            server_.onDisconnect = OnDisconnect;
            server_.onConnect = OnConnect;
            server_.onRecv = OnRecv;
            _ = server_.Start(int.Parse(param), new TcpDataProtocol());
        }
        public bool Response(AgentChatMessageInfo response)
        {
            try
            {
                TcpClientNetwork? session = GetSession(response.sessionId);
                if (session == null) return false;
                if (!session.Send(JsonConvert.SerializeObject(response) + "\n")) return false;
                if (response.type == 8)
                {
                    session.Stop();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger_.Error(ex.Message);
                return false;
            }
        }
        public void Stop(string sessionId)
        {
            TcpClientNetwork? session = GetSession(sessionId);
            if (session == null) return;
            session.Stop();
        }
        private void OnRecv(TcpClientNetwork session, string json)
        {
            try
            {
                var chat = JsonConvert.DeserializeObject<AgentChatMessageInfo>(json);
                if (chat == null) return;
                session.SetUserData(chat.sessionId);
                onRequest?.Invoke(chat);
            }
            catch (Exception ex)
            {
                logger_.Error(ex.Message);
            }
        }
        private void OnDisconnect(TcpClientNetwork session)
        {
        }
        private void OnConnect(TcpClientNetwork session)
        {
        }
        private TcpClientNetwork? GetSession(string sessionId)
        {
            foreach (var session in server_.GetClients())
            {
                string? userData = (string?)session.GetUserData();
                if (userData != null && userData == sessionId)
                    return session;
            }
            return null;
        }
    }
}
