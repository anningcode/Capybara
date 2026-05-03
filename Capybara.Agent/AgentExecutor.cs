using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentExecutor
    {
        private AgentChatSession session_ { get; set; }
        public Func<AgentLLMResponseInfo, AgentChatSession, bool> onResponse { get; set; }
        public AgentExecutor(AgentChatSession session, Func<AgentLLMResponseInfo, AgentChatSession, bool> callback)
        {
            session_ = session;
            onResponse = callback;
        }
        // 请求
        public AgentLLMResponseInfo Request()
        {
            AgentLLMResponseInfo response = new LLMRequest().Request(session_.GetSession().Request, OnResponse);
            OnResponse(response);
            return response;
        }
        // llm响应
        private bool OnResponse(AgentLLMResponseInfo response)
        {
            if (response.Stop) return true;
            return onResponse.Invoke(response, session_);
        }
    }
}
