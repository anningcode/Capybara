using Capybara.Models;
using Capybara.Utils;
using LLMGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.LLM
{
    public static class LLMRequest
    {
        public static LLMChatResponseInfo Request(LLMChatRequestInfo request, Func<LLMChatResponseInfo, bool> callback)
        {
            LLMChatResponseInfo response   = new LLMChatResponseInfo();
            LLMNetworkRequest   tcpRequest = new LLMNetworkRequest();
            Tuple<string, int>  config     = Tuple.Create("127.0.0.1", 5000);
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals");
            if (values != null)
            {
                var address = values.FirstOrDefault(n => n.Key == "llmAddress" && n.Enable)?.Value ?? "127.0.0.1:5000";
                if (address.Split(':').Length == 2)
                {
                    try
                    {
                        config = Tuple.Create(address.Split(':')[0], int.Parse(address.Split(':')[1]));
                    }
                    catch 
                    {
                        config = Tuple.Create("127.0.0.1", 5000);
                    }
                }
                else
                {
                    config = Tuple.Create("127.0.0.1", 5000);
                }
                var appKey = values.FirstOrDefault(n => n.Key == "appKey" && n.Enable)?.Value ?? "123456789";
                request.AppKey = appKey;
            }
            if (!tcpRequest.Request(config, request, (LLMChatResponseInfo resp) =>
            {
                if (resp.Stop)
                {
                    response.Think += resp.Think;
                    response.Answer += resp.Answer;
                    response.Content += resp.Content;
                    response.Message = resp.Message;
                    response.Success = resp.Success;
                    response.Stop = resp.Stop;
                    response.ToolCalls = resp.ToolCalls;
                    return true;
                }
                return callback.Invoke(resp);
            }))
            {
                response.Stop = true;
                response.Success = false;
                response.Message = "未知异常!";
            }
            tcpRequest.Dispose();
            return response;
        }
    }
}
