using Capybara.IEntrance;
using Capybara.Models;
using Newtonsoft.Json;
using NexusNetNetwork;
using NLog;
using TcpNetwork;

namespace Capybara.NexusNetEntrance
{
    public class ChatNexusNetEntrance : IChatEntrance
    {
        // 日志
        private static Logger logger_ = LogManager.Setup().LoadConfigurationFromFile("config/nlog.config").GetCurrentClassLogger();
        private NexusNetInstance chatClient_ { get; set; } = NexusNetInstance.Instance();
        private Dictionary<string, TcpClientNetwork> tcpClientList_ { get; set; } = new Dictionary<string, TcpClientNetwork>();
        public Action<AgentChatMessageInfo>? onRequest { get; set; }
        public ChatNexusNetEntrance(string param)
        {
            chatClient_.Subscribe(this);
        }
        public bool Response(AgentChatMessageInfo response)
        {
            try
            {
                lock (tcpClientList_)
                {
                    if (!tcpClientList_.ContainsKey(response.sessionId)) return false;
                    if (!tcpClientList_[response.sessionId].Send(JsonConvert.SerializeObject(response) + "\n")) return false;
                    if (response.type == 12)
                    {
                        tcpClientList_[response.sessionId].Stop();
                    }
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
            TcpClientNetwork? client = null;
            lock (tcpClientList_)
            {
                if (tcpClientList_.ContainsKey(sessionId))
                    client = tcpClientList_[sessionId];
            }
            client?.Stop();
        }
        // 聊天入口
        [Subscribe("AgentChatRequest")]
        public void ChatRequest(string json)
        {
            try
            {
                var chat = JsonConvert.DeserializeObject<AgentChatMessageInfo>(json);
                if (chat == null) return;
                ConnectTcpServer(chat.address, chat.sessionId);
                onRequest?.Invoke(chat);
            }
            catch (Exception ex)
            {
                logger_.Error(ex.Message);
            }
        }
        private async void ConnectTcpServer(string address, string sessionId)
        {
            TcpClientNetwork tcpClient = new TcpClientNetwork();
            tcpClient.onConnect = OnConnect;
            tcpClient.onDisconnect = OnDisconnect;
            tcpClient.onRecv = OnRecv;
            string[] addressList = address.Split(':');
            lock (tcpClientList_)
            {
                if (tcpClientList_.ContainsKey(sessionId))
                {
                    tcpClientList_[sessionId].Stop();
                    tcpClientList_.Remove(sessionId);
                }
                tcpClientList_.Add(sessionId, tcpClient);
            }
            var result = await tcpClient.Start(addressList[0], int.Parse(addressList[1]));
            if (!result) throw new Exception("连接服务器失败!");
        }
        private void OnRecv(TcpClientNetwork session, string data)
        {

        }
        private void OnDisconnect(TcpClientNetwork session)
        {
            lock (tcpClientList_)
            {
                var item = tcpClientList_.FirstOrDefault(x => x.Value == session);
                if (!string.IsNullOrEmpty(item.Key))
                {
                    tcpClientList_.Remove(item.Key);
                }
            }
        }
        private void OnConnect(TcpClientNetwork session, bool status, string message)
        {
            if (!status)
            {
                logger_.Error($"连接服务器失败: {message}");
            }
            else
            {
                logger_.Info($"连接服务器成功!");
            }
        }
    }
}
