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
            string url = $"{AppConfig.Get<string>("llmAddress")}?appKey={AppConfig.Get<string>("appKey")}";
            return request_.Request(url, request, callback);
        }
    }
}
