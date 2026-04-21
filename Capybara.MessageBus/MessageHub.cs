using Easy.MessageHub;

namespace Capybara.MessageBus
{
    public class MessageHub
    {
        private static MessageHub instance_ { get; set; } = new MessageHub();
        private IMessageHub hub_ { get; set; } = new Easy.MessageHub.MessageHub();
        public static MessageHub Instance { get { return instance_; } }
        private MessageHub() { }
        public Guid Subscribe<T>(Action<T> action)
        { 
            return hub_.Subscribe<T>(action);
        }
        public void Publish<T>(T data)
        {
            hub_.Publish<T>(data);
        }
    }
}
