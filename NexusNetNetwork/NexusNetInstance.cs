using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetNetwork
{
    public class NexusNetInstance
    {
        // 客户端实例
        private nexus_net_client client_ = nexus_net_client.instance();
        private bool isConnected_ { get; set; } = false;
        private static NexusNetInstance instance_ { get; set; } = new NexusNetInstance();
        public static NexusNetInstance Instance()
        {
            return instance_;
        }
        public Action<bool>? onConnect { get; set; }
        public Action? onDisconnect { get; set; }
        public Action<bool, string, string>? onAuthorize { get; set; }
        private NexusNetInstance()
        {
            client_.connect_event = OnConnect;
            client_.disconnect_event = OnDisconnect;
            client_.authorize_event = OnAuthorize;
        }
        public void Start()
        {
            if (!isConnected_)
            {
                isConnected_ = true;
                client_.start();
            }
        }
        public void Stop()
        {
            client_.stop();
        }
        public void Subscribe(params object[] values)
        {
            client_.subscribe(values);
        }
        public nexus_net_message CallerMessage()
        { 
            return nexus_net_client.caller_message();
        }
        public T Publisher<T>(string key, params object[] args) where T : new()
        {
            return client_.publisher<T>(key, args);
        }
        public T Publisher<T>(int timeout, string key, params object[] args) where T : new()
        {
            return client_.publisher<T>(timeout, key, args);
        }
        public void Publisher(string key, params object[] args)
        {
            client_.publisher(key, args);
        }
        public void Publisher(int timeout, string key, params object[] args)
        {
            client_.publisher(timeout, key, args);
        }
        public T CustomPublisher<T>(string key, string format, params object[] args) where T : new()
        { 
            return client_.custom_publisher<T>(key, format, args);
        }
        public T CustomPublisher<T>(int timeout, string key, string format, params object[] args) where T : new()
        {
            return client_.custom_publisher<T>(timeout, key, format, args);
        }
        public void AppPublisher(string key, string nodeName, string appName, params object[] args)
        {
            client_.app_publisher(key, nodeName, appName, args);
        }
        public void NodePublisher(string key, string nodeName, params object[] args)
        { 
            client_.node_publisher(key, nodeName, args);
        }
        public void CustomPublisher(int timeout, string key, string nodeName, string appName, params object[] args)
        { 
            client_.custom_publisher(timeout, key, nodeName, appName, args);
        }
        private void OnConnect(bool success)
        {
            onConnect?.Invoke(success);
        }
        private void OnDisconnect()
        {
            isConnected_ = false;
            onDisconnect?.Invoke();
        }
        private void OnAuthorize(bool success, string parentCode, string parentName)
        {
            onAuthorize?.Invoke(success, parentCode, parentName);
        }
    }
}
