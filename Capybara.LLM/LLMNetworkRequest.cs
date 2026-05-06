using LLMGateway.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.LLM
{
    internal class LLMNetworkRequest
    {
        private WebSocketClient _client { get; set; } = new();
        private LLMChatResponseInfo _response { get; set; } = new();
        public bool Start(string url)
        {
            bool result = _client.Start(url).Result;
            return result;
        }
        public LLMChatResponseInfo Request(string url, LLMChatRequestInfo request, Func<LLMChatResponseInfo, bool> callback)
        {
            try
            {
                if (!Start(url)) 
                {
                    _client.Stop().Wait();
                    throw new Exception("连接websocket失败!");
                }
                _client.Send(JsonConvert.SerializeObject(request)).Wait();
                Receive(callback).Wait();
                _client.Stop().Wait();
            }
            catch (Exception ex)
            {
                _response.Stop = true;
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        private async Task Receive(Func<LLMChatResponseInfo, bool> callback)
        {
            try
            {
                while (true)
                {
                    string? value = await _client.Receive();
                    if (value == null) break;
                    var response = JsonConvert.DeserializeObject<LLMChatResponseInfo>(value);
                    if (response == null) throw new Exception("反序列化失败!");
                    if (response.Stop)
                    {
                        _response = response;
                        break;
                    }
                    else if (!callback.Invoke(response))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _response.Stop = true;
                _response.Success = false;
                _response.Message = ex.Message;
            }
        }
    }
}
