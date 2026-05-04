using Capybara.LLM;
using Capybara.Models;
using LLMGateway.Models;

namespace Capybara.Agent
{
    public class AgentExecutor
    {
        private AgentChatSession session_ { get; set; }
        public Func<LLMChatResponseInfo, AgentChatSession, bool> onResponse { get; set; }
        public AgentExecutor(AgentChatSession session, Func<LLMChatResponseInfo, AgentChatSession, bool> callback)
        {
            session_ = session;
            onResponse = callback;
        }
        // 请求
        public LLMChatResponseInfo Request()
        {
            LLMChatResponseInfo response = LLMRequest.Request(session_.GetSession().Request, OnResponse);
            OnResponse(response);
            return response;
        }
        // llm响应
        private bool OnResponse(LLMChatResponseInfo response)
        {
            if (response.Stop) return true;
            return onResponse.Invoke(response, session_);
        }
    }
}
