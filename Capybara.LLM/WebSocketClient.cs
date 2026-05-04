using System.Net.WebSockets;
using System.Text;

namespace Capybara.LLM
{
    internal class WebSocketClient
    {
        private ClientWebSocket? _ws { get; set; }
        public WebSocketClient()
        {
        }
        public async Task<bool> Start(string url)
        {
            try
            {
                if (_ws == null)
                    _ws = new ClientWebSocket();
                await _ws.ConnectAsync(new Uri(url), CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task Stop()
        {
            try
            {
                if (_ws == null) throw new Exception("使用未创建的对象!");
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
                _ws.Dispose();
                _ws = null;
            }
            catch { _ws = null; }
        }
        public async Task Send(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            if (_ws == null) throw new Exception("使用未创建的对象!");
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task<string?> Receive()
        {
            using var ms = new MemoryStream();
            var buffer = new byte[4096];
            WebSocketReceiveResult result;

            do
            {
                if (_ws == null) throw new Exception("使用未创建的对象!");
                result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                ms.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            // 根据消息类型决定如何解码
            if (result.MessageType == WebSocketMessageType.Text)
            {
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                return null;
            }
            else
            {
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
