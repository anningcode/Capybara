using System.Net;
using System.Net.Sockets;

namespace TcpNetwork
{
    public class TcpServerNetwork : IDisposable
    {
        private IDataProtocol? dataProtocol_ { get; set; } = null;
        private TcpListener? tcpListener_ { get; set; }
        private CancellationTokenSource cts_ { get; set; } = new();
        public Action<TcpClientNetwork>? onConnect { get; set; }
        public Action? onClose { get; set; }
        public Action<string>? onError { get; set; }
        public TcpServerNetwork() { }
        public async Task Start(int port, IDataProtocol? dataProtocol)
        {
            dataProtocol_ = dataProtocol;
            await Start(port);
        }
        public async Task Start(int port)
        {
            tcpListener_ = new TcpListener(IPAddress.Any, port);
            try
            {
                tcpListener_.Start();
                while (!cts_.IsCancellationRequested)
                {
                    TcpClient client = await tcpListener_.AcceptTcpClientAsync(cts_.Token);
                    _ = NewHandleClient(client);
                }
            }
            catch (OperationCanceledException) 
            {
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
            finally
            {
                tcpListener_.Stop();
                onClose?.Invoke();
            }
        }
        public void Stop()
        {
            cts_.Cancel();
        }
        private async Task NewHandleClient(TcpClient client)
        {
            await Task.Run(() =>
            {
                onConnect?.Invoke(new TcpClientNetwork(client, dataProtocol_));
            });
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
