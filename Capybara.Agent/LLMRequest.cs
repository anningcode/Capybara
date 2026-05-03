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
            LLMNetworkRequest tcpRequest = new LLMNetworkRequest(request.Address, OnResponse);
            if (!tcpRequest.Request(request))
            {
                response_.Stop = true;
                response_.Success = false;
                response_.Message = "未知异常!";
            }
            tcpRequest.Dispose();
            return response_;
        }
        private bool OnResponse(AgentLLMResponseInfo response)
        {
            if (response.Stop)
            {
                response_.Think += response.Think;
                response_.Answer += response.Answer;
                response_.Content += response.Content;
                response_.Message = response.Message;
                response_.Success = response.Success;
                response_.Stop = response.Stop;
                response_.ToolCalls = response.ToolCalls;
                return true;
            }
            if (onResponse == null) return false;
            return onResponse.Invoke(response);
        }
    }
}
