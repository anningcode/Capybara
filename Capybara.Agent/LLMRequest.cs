using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public class LLMRequest
    {
        private AgentLLMResponseInfo response_ { get; set; } = new AgentLLMResponseInfo();
        private Func<AgentLLMResponseInfo, bool>? onResponse { get; set; }
        public LLMRequest() { }
        public AgentLLMResponseInfo Request(AgentLLMRequestInfo request, Func<AgentLLMResponseInfo, bool> callback)
        {
            response_ = new AgentLLMResponseInfo();
            onResponse = callback;
            LLMNetworkRequest tcpRequest = new LLMNetworkRequest(request.address, OnResponse);
            if (!tcpRequest.Request(request))
            {
                response_.stop = true;
                response_.success = false;
                response_.message = "未知异常!";
            }
            tcpRequest.Dispose();
            return response_;
        }
        private bool OnResponse(AgentLLMResponseInfo response)
        {
            if (response.stop)
            {
                response_.think += response.think;
                response_.answer += response.answer;
                response_.content += response.content;
                response_.message = response.message;
                response_.success = response.success;
                response_.stop = response.stop;
                response_.toolCalls = response.toolCalls;
                return true;
            }
            if (onResponse == null) return false;
            return onResponse.Invoke(response);
        }
    }
}
