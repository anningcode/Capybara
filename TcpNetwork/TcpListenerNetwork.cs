using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetwork
{
    public class TcpListenerNetwork
    {
        private Dictionary<string, TcpClientNetwork> clientList_ { get; set; } = new Dictionary<string, TcpClientNetwork>();
        private TcpServerNetwork server_ { get; set; } = new TcpServerNetwork();
        public Action<TcpClientNetwork, string>? onRecv { get; set; }
        public Action<TcpClientNetwork>? onDisconnect { get; set; }
        public Action<TcpClientNetwork>? onConnect { get; set; }
        public Action<string>? onError { get; set; }
        public TcpListenerNetwork()
        {
            server_.onConnect = OnConnect;
            server_.onClose = OnClose;
            server_.onError = OnError;
        }
        public async Task Start(int port, IDataProtocol? dataProtocol)
        {
            await server_.Start(port, dataProtocol);
        }
        public async Task Start(int port)
        {
            await server_.Start(port);
        }
        public void Stop()
        {
            server_.Stop();
        }
        public void Stop(string hashKey)
        {
            TcpClientNetwork? client = null;
            lock (clientList_)
            {
                if (clientList_.ContainsKey(hashKey))
                    client = clientList_[hashKey];
            }
            client?.Stop();
        }
        public List<TcpClientNetwork> GetClients()
        {
            List<TcpClientNetwork> result = new List<TcpClientNetwork>();
            lock (clientList_)
            {
                foreach (var item in clientList_)
                {
                    result.Add(item.Value);
                }
            }
            return result;
        }
        public TcpClientNetwork? GetClient(string hashKey)
        {
            lock (clientList_)
            {
                if (clientList_.ContainsKey(hashKey))
                    return clientList_[hashKey];
            }
            return null;
        }
        private void OnConnect(TcpClientNetwork client)
        {
            client.onDisconnect = OnDisconnect;
            client.onRecv = OnRecv;
            lock (clientList_)
            {
                clientList_.Add(client.GetHashKey(), client);
            }
            onConnect?.Invoke(client);
        }
        private void OnError(string err)
        {
            onError?.Invoke(err);
        }
        private void OnClose()
        {
            lock (clientList_)
            {
                foreach (var item in clientList_)
                {
                    item.Value.Dispose();
                }
                clientList_.Clear();
            }
        }
        private void OnRecv(TcpClientNetwork client, string data)
        {
            onRecv?.Invoke(client, data);
        }
        private void OnDisconnect(TcpClientNetwork client)
        {
            lock (clientList_)
            {
                if (clientList_.ContainsKey(client.GetHashKey()))
                    clientList_.Remove(client.GetHashKey());
            }
            onDisconnect?.Invoke(client);
        }
    }
}
