using Capybara.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public class LLMNetworkRequest : IDisposable
    {
        private TcpClient client_ { get; set; } = new TcpClient();
        private Tuple<string, int> address_ { get; set; } = Tuple.Create("127.0.0.1", 12345);
        private Func<AgentLLMResponseInfo, bool> onRecv { get; set; }
        public LLMNetworkRequest(string address, Func<AgentLLMResponseInfo, bool> callback)
        {
            onRecv = callback;
            var list = address.Split(':');
            try
            {
                address_ = Tuple.Create(list[0], int.Parse(list[1]));
            }
            catch
            {
                Console.WriteLine($"地址错误,地址:{address}");
            }
        }
        public void Dispose()
        {
            Stop();
        }
        public bool Request(AgentLLMRequestInfo data)
        {
            try
            {
                Start();
                Send(JsonConvert.SerializeObject(data));
                Receive();
                Stop();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        private void Start()
        {
            client_ = new TcpClient();
            client_.Connect(address_.Item1, address_.Item2);
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
                    var json = JsonConvert.DeserializeObject<AgentLLMResponseInfo>(line);
                    if (json == null) throw new Exception("JSON为null反序列化数据异常!");
                    if (!onRecv.Invoke(json))
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
