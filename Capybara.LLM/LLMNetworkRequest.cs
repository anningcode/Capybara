using Capybara.Utils;
using LLMGateway.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.LLM
{
    internal class LLMNetworkRequest : IDisposable
    {
        private TcpClient client_ { get; set; } = new TcpClient();
        private Func<LLMChatResponseInfo, bool>? onRecv { get; set; }
        public void Dispose()
        {
            Stop();
        }
        public bool Request(Tuple<string, int> config, LLMChatRequestInfo data, Func<LLMChatResponseInfo, bool> callback)
        {
            try
            {
                onRecv = callback;
                Start(config);
                Send(JsonConvert.SerializeObject(data));
                Receive();
                Stop();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return false;
        }
        private void Start(Tuple<string, int> address)
        {
            client_ = new TcpClient();
            client_.Connect(address.Item1, address.Item2);
        }
        private void Stop()
        {
            client_.Close();
        }
        private void Send(string data)
        {
            NetworkStream stream = client_.GetStream();
            var writer = new StreamWriter(stream);
            writer.Write(data + "\r\n");
            writer.Flush();
        }
        private void Receive()
        {
            using (NetworkStream stream = client_.GetStream())
            {
                using var reader = new StreamReader(stream);
                while (client_.Connected)
                {
                    string? line;
                    line = reader.ReadLine();
                    if (line == null) break;
                    var json = JsonConvert.DeserializeObject<LLMChatResponseInfo>(line);
                    if (json == null) throw new Exception("JSON为null反序列化数据异常!");
                    if (!onRecv?.Invoke(json) ?? true)
                    {
                        break;
                    }
                    if (json.Stop)
                        break;
                }
            }
        }
    }
}
