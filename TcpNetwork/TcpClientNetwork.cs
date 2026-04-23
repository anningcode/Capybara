using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetwork
{
    public class TcpClientNetwork : IDisposable
    {
        private IDataProtocol? dataProtocol_ { get; set; } = null;
        private TcpClient client_ { get; set; }
        private object? userData_ { get; set; }
        private string hashKey_ { get; set; } = string.Empty;
        public Action<TcpClientNetwork, string>? onRecv { get; set; }
        public Action<TcpClientNetwork>? onDisconnect { get; set; }
        public Action<TcpClientNetwork, bool, string>? onConnect { get; set; }
        public TcpClientNetwork(TcpClient client, IDataProtocol? dataProtocol)
        {
            hashKey_ = Guid.NewGuid().ToString();
            client_ = client;
            dataProtocol_ = dataProtocol;
            _ = OnRecv();
        }
        public TcpClientNetwork()
        {
            hashKey_ = Guid.NewGuid().ToString();
            client_ = new TcpClient();
        }
        public async Task<bool> Start(string ip, int port, IDataProtocol dataProtocol)
        {
            dataProtocol_ = dataProtocol;
            return await Start(ip, port);
        }
        public async Task<bool> Start(string ip, int port)
        {
            try
            {
                await client_.ConnectAsync(ip, port);
                _ = OnRecv();
                onConnect?.Invoke(this, true, "连接成功");
                return true;
            }
            catch (SocketException ex)
            {
                onConnect?.Invoke(this, false, ex.Message);
            }
            catch (Exception ex)
            {
                onConnect?.Invoke(this, false, ex.Message);
            }
            return false;
        }
        public void Stop()
        {
            client_?.Close();
        }
        public void SetUserData(object userData)
        {
            userData_ = userData;
        }
        public string GetHashKey()
        {
            return hashKey_;
        }
        public object? GetUserData()
        {
            return userData_;
        }
        public bool Send(string data)
        {
            try
            {
                NetworkStream stream = client_.GetStream();
                var writer = new StreamWriter(stream);
                writer.Write(data);
                writer.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task OnRecv()
        {
            await Task.Run(async () =>
            {
                try
                {
                    using (NetworkStream stream = client_.GetStream())
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        string dataAll = string.Empty;

                        char[] buffer = new char[4096];
                        int charsRead;

                        while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            dataAll += new string(buffer, 0, charsRead);
                            if (string.IsNullOrEmpty(dataAll)) break;
                            if (dataProtocol_ == null)
                            {
                                onRecv?.Invoke(this, dataAll);
                                dataAll = string.Empty;
                            }
                            else
                            {
                                while (true)
                                {
                                    var tuple = dataProtocol_.MatchRole(dataAll);
                                    if (!tuple.Item2) throw new Exception("数据异常!");
                                    else if (tuple.Item1 > 0)
                                    {
                                        string value = dataAll.Substring(0, tuple.Item1);
                                        onRecv?.Invoke(this, value);
                                        dataAll = dataAll.Substring(tuple.Item1);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                Stop();
                onDisconnect?.Invoke(this);
            });
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
