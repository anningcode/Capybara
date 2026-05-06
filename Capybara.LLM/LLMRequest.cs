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
    public class LLMRequest
    {
        private static LLMNetworkRequest request_ { get; set; } = new LLMNetworkRequest();
        public static LLMChatResponseInfo Request(LLMChatRequestInfo request, Func<LLMChatResponseInfo, bool> callback)
        {
            string url = "ws://127.0.0.1:5000/ws";
            string appKey = "123456789";
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals");
            if (values != null)
            {
                url = values.FirstOrDefault(n => n.Key == "llmAddress" && n.Enable)?.Value ?? url;
                appKey = values.FirstOrDefault(n => n.Key == "appKey" && n.Enable)?.Value ?? appKey;
            }

            return request_.Request($"{url}?appKey={appKey}", request, callback);
        }
    }
}
