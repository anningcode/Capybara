using Capybara.Models;

namespace Capybara.IEntrance
{
    public interface IChatEntrance
    {
        Action<AgentChatMessageInfo>? onRequest { get; set; }
        bool Response(AgentChatMessageInfo response);
        void Stop(string userId);
    }
}
