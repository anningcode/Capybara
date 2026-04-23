using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace NexusNetNetwork
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum nexus_net_msg_type_enum
    {
        /// <summary>
        /// 身份认证
        /// </summary>
        AUTHENTICATE = 1,
        /// <summary>
        /// 身份认证响应
        /// </summary>
        REPAUTHENTICATE,
        /// <summary>
        /// 订阅
        /// </summary>
        SUBSCRIBE,
        /// <summary>
        /// 订阅响应
        /// </summary>
        REPSUBSCRIBE,
        /// <summary>
        /// 删除订阅
        /// </summary>
        REMOVESUBSCRIBE,
        /// <summary>
        /// 发布
        /// </summary>
        PUBLISHER,
        /// <summary>
        /// 发布响应
        /// </summary>
        REPPUBLISHER,
        /// <summary>
        /// 心跳
        /// </summary>
        HEARTBEAT,
        /// <summary>
        /// 详情
        /// </summary>
        DETAILS,
        /// <summary>
        /// 详情响应
        /// </summary>
        REPDETAILS
    }
    /// <summary>
    /// 消息
    /// </summary>
    public class nexus_net_message
    {
        /// <summary>
        /// 
        /// </summary>
        public nexus_net_message() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public nexus_net_message(List<byte> data)
        {
            set_data(data);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        public void set_data(List<byte> data)
        {
            //消息类型
            msg_type = data[5];
            //消息ID
            int size = data[6];
            int begin = 7;
            int end = begin + size;
            msg_id = Encoding.UTF8.GetString(data.GetRange(begin, size).ToArray());
            //路由
            size = data[end];
            begin = end + 1;
            end = begin + size;
            route = Encoding.UTF8.GetString(data.GetRange(begin, size).ToArray());
            //路由列表
            size = data[end] << 8;
            size += data[++end];
            begin = end + 1;
            end = begin + size;
            if (size > 0)
            {
                string str_routes = Encoding.UTF8.GetString(data.GetRange(begin, size).ToArray());
                routes = str_routes.Split((char)0x0d).ToList();
            }
            //参数
            size = data[end] << 24;
            size += data[++end] << 16;
            size += data[++end] << 8;
            size += data[++end];
            begin = end + 1;
            end = begin + size;
            args.add(data.GetRange(begin, size));
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<byte> get_data()
        {
            List<byte> result = new List<byte>();
            //协议头
            result.Add(0x02);
            //字节数量
            result.Add(0x00);
            result.Add(0x00);
            result.Add(0x00);
            result.Add(0x00);
            //消息类型
            result.Add(msg_type);
            //消息ID
            result.Add((byte)msg_id.Length);
            result.AddRange(Encoding.UTF8.GetBytes(msg_id));
            //路由
            result.Add((byte)route.Length);
            result.AddRange(Encoding.UTF8.GetBytes(route));
            //路由列表
            //路由列表
            List<byte> vec_routes = new List<byte>();
            for (int i = 0; i < routes.Count; ++i)
            {
                if (i + 1 == routes.Count)
                {
                    vec_routes.AddRange(Encoding.UTF8.GetBytes(routes[i]));
                }
                else
                {
                    vec_routes.AddRange(Encoding.UTF8.GetBytes(routes[i]));
                    vec_routes.Add(0x0d);
                }
            }
            result.Add((byte)(vec_routes.Count >> 8));
            result.Add((byte)(vec_routes.Count >> 0));
            result.AddRange(vec_routes);
            //参数
            List<byte> vec_args = args.data();
            result.Add((byte)(vec_args.Count >> 24));
            result.Add((byte)(vec_args.Count >> 16));
            result.Add((byte)(vec_args.Count >> 8));
            result.Add((byte)(vec_args.Count >> 0));
            result.AddRange(vec_args);
            //协议尾
            //协议尾
            result.Add(0x03);
            result[1] = (byte)(result.Count >> 24);
            result[2] = (byte)(result.Count >> 16);
            result[3] = (byte)(result.Count >> 8);
            result[4] = (byte)(result.Count >> 0);
            return result;
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public byte msg_type = 0;
        /// <summary>
        /// 消息ID
        /// </summary>
        public string msg_id = "";
        /// <summary>
        /// 路由
        /// </summary>
        public string route = "";
        /// <summary>
        /// 路由列表
        /// </summary>
        public List<string> routes = new List<string>();
        /// <summary>
        /// 参数
        /// </summary>
        public archive_stream args = new archive_stream();
    }
    /// <summary>
    /// 客户端
    /// </summary>
    public class nexus_net_tcp_client
    {
        public delegate void ConnectEvent(bool success, string msg);
        public delegate void DisconnectEvent();
        public delegate void RecvEvent(List<byte> data);
        public nexus_net_tcp_client()
        {
            is_connect_ = false;
            buffer_ = new byte[1024];
            buffer_all_ = new List<byte>();
        }
        public void start(string ip, int port)
        {
            ip_ = ip;
            port_ = port;
            is_stop_ = false;
            begin_connect(false);
        }
        public void stop()
        {
            buffer_all_.Clear();
            is_stop_ = true;
            client_?.Close();
        }
        public Task<int> send_async(List<byte> data)
        {
            return client_.SendAsync(data.ToArray());
        }
        public int send(List<byte> data)
        {
            try
            {
                return client_.Send(data.ToArray());
            }
            catch
            {
                return 0;
            }
        }
        private void begin_connect(bool reconnect = true)
        {
            if (is_stop_)
            {
                if (is_connect_)
                {
                    if (disconnect_event != null) disconnect_event();
                }
                return;
            }
            stop();
            is_stop_ = false;
            IPEndPoint remote_ep = new IPEndPoint(IPAddress.Parse(ip_), port_);
            client_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (reconnect)
            {
                if (is_connect_)
                {
                    if (disconnect_event != null) disconnect_event();
                }
                Thread.Sleep(2000);
            }
            client_.BeginConnect(remote_ep, connect_callback, client_);
        }
        private void end_connect(IAsyncResult ar)
        {
            client_.EndConnect(ar);
        }
        private void connect_callback(IAsyncResult ar)
        {
            try
            {
                end_connect(ar);
                connect(true, "success");
            }
            catch (Exception ec)
            {
                connect(false, ec.Message);
            }
        }
        private void connect(bool status, string msg)
        {
            is_connect_ = status;
            if (connect_event != null) connect_event(status, msg);
            if (status)
            {
                client_.BeginReceive(buffer_, 0, 1024, 0, receive_callback, client_);
            }
            else
            {
                begin_connect();
            }
        }

        private object receiveCallbackLocker = new object();
        private void receive_callback(IAsyncResult ar)
        {
            try
            {
                lock (receiveCallbackLocker)
                {
                    int read_len = client_.EndReceive(ar);
                    if (read_len == 0)
                    {
                        begin_connect();
                    }
                    else
                    {
                        buffer_all_.AddRange(buffer_.ToList().GetRange(0, read_len));
                        while (true)
                        {
                            var tuple = match_role(buffer_all_);
                            if (!tuple.Item2)
                            {
                                begin_connect();
                            }
                            else if (tuple.Item1 > 0)
                            {
                                if (recv_event != null)
                                {
                                    recv_event(buffer_all_.GetRange(0, tuple.Item1));
                                    buffer_all_.RemoveRange(0, tuple.Item1);
                                    if (buffer_all_.Any())
                                        continue;
                                }
                                else
                                {
                                    buffer_all_.Clear();
                                }
                            }
                            break;
                        }
                        client_.BeginReceive(buffer_, 0, 1024, 0, receive_callback, client_);
                    }
                }
            }
            catch (Exception ex)
            {
                begin_connect();
            }
        }
        private Tuple<int, bool> match_role(List<byte> buffer)
        {
            int index = 0;
            if (buffer.Any())
            {
                if (buffer[index] != 0x02)
                    return Tuple.Create(0, false);
                if (buffer.Count < 5)
                    return Tuple.Create(0, true);
                int size = 0;
                for (int i = 0; i < 4; ++i)
                {
                    size += buffer[i + 1] << 8 * (3 - i);
                }
                if (buffer.Count >= size)
                {
                    if (buffer[size - 1] == 0x03)
                    {
                        return Tuple.Create(size, true);
                    }
                    else
                    {
                        return Tuple.Create(0, false);
                    }
                }
            }
            return Tuple.Create(0, true);
        }
        private Socket client_ { get; set; }
        private byte[] buffer_ { get; set; }
        private string ip_ { get; set; }
        private int port_ { get; set; }
        private bool is_stop_ { get; set; }
        private bool is_connect_ { get; set; }
        private List<byte> buffer_all_ { get; set; }
        public ConnectEvent connect_event { get; set; }
        public DisconnectEvent disconnect_event { get; set; }
        public RecvEvent recv_event { get; set; }
    }
    /// <summary>
    /// 日志类型
    /// </summary>
    public class nexus_net_logger_config_info
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public int type = 0;
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks = "";
    };
    /// <summary>
    /// 父节点配置
    /// </summary>
    public class nexus_net_client_config_info
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string ip = "";
        /// <summary>
        /// 端口
        /// </summary>
        public int port = 0;
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks = "";
    };
    /// <summary>
    /// 订阅配置
    /// </summary>
    public class nexus_net_subscribe_config_info
    {
        /// <summary>
        /// KEY
        /// </summary>
        public string key = "";
        /// <summary>
        /// 路由
        /// </summary>
        public string route = "";
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks = "";
    };
    /// <summary>
    /// 发布配置
    /// </summary>
    public class nexus_net_publisher_config_info
    {
        /// <summary>
        /// KEY
        /// </summary>
        public string key = "";
        /// <summary>
        /// 节点
        /// </summary>
        public string route = "";
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks = "";
    };
    /// <summary>
    /// 用户信息
    /// </summary>
    public class nexus_net_user_config_info
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name = "";
        /// <summary>
        /// 代码
        /// </summary>
        public string code = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string password = "";
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks = "";
    };
    /// <summary>
    /// 配置
    /// </summary>
    public class nexus_net_config_info
    {
        /// <summary>
        /// 身份
        /// </summary>
        public nexus_net_user_config_info user = new nexus_net_user_config_info();
        /// <summary>
        /// 日志
        /// </summary>
        public List<nexus_net_logger_config_info> loggers = new List<nexus_net_logger_config_info>();
        /// <summary>
        /// 客户端
        /// </summary>
        public nexus_net_client_config_info client = new nexus_net_client_config_info();
        /// <summary>
        /// 订阅
        /// </summary>
        public Dictionary<string, nexus_net_subscribe_config_info> subscribes = new Dictionary<string, nexus_net_subscribe_config_info>();
        /// <summary>
        /// 发布
        /// </summary>
        public Dictionary<string, nexus_net_publisher_config_info> publishers = new Dictionary<string, nexus_net_publisher_config_info>();
    };
    /// <summary>
    /// nexusnet配置
    /// </summary>
    public class nexus_net_config
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static Logger logger_ = LogManager.Setup().LoadConfigurationFromFile("config/nlog.config").GetCurrentClassLogger();
        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public static nexus_net_config_info read()
        {
            lock (locker_)
            {
                if (value_ != null)
                {
                    return value_;
                }
                value_ = new nexus_net_config_info();
                XmlDocument doc = new XmlDocument();
                doc.Load("./config/nexus_net_config.xml");
                // 读取 user 节点
                XmlNodeList user_nodes = doc.SelectNodes("/root/user/item");
                foreach (XmlNode node in user_nodes)
                {
                    if (node.Attributes["enable"] != null && bool.Parse(node.Attributes["enable"].Value))
                    {
                        if (node.Attributes["name"] != null) value_.user.name = node.Attributes["name"].Value;
                        if (node.Attributes["code"] != null) value_.user.code = node.Attributes["code"].Value;
                        if (node.Attributes["password"] != null) value_.user.password = node.Attributes["password"].Value;
                        if (node.Attributes["enable"] != null) value_.user.enable = true;
                        if (node.Attributes["remarks"] != null) value_.user.remarks = node.Attributes["remarks"].Value;
                    }
                }
                // 读取 logger 节点
                XmlNodeList loggerNodes = doc.SelectNodes("/root/logger/item");
                foreach (XmlNode node in loggerNodes)
                {
                    nexus_net_logger_config_info item = new nexus_net_logger_config_info();
                    if (node.Attributes["type"] != null) item.type = int.Parse(node.Attributes["type"].Value);
                    if (node.Attributes["enable"] != null) item.enable = bool.Parse(node.Attributes["enable"].Value);
                    if (node.Attributes["remarks"] != null) item.remarks = node.Attributes["remarks"].Value;
                    value_.loggers.Add(item);
                }

                // 读取 client 节点
                XmlNodeList clientNodes = doc.SelectNodes("/root/client/item");
                foreach (XmlNode node in clientNodes)
                {
                    if (node.Attributes["enable"] != null && bool.Parse(node.Attributes["enable"].Value))
                    {
                        if (node.Attributes["ip"] != null) value_.client.ip = node.Attributes["ip"].Value;
                        if (node.Attributes["port"] != null) value_.client.port = int.Parse(node.Attributes["port"].Value);
                        if (node.Attributes["enable"] != null) value_.client.enable = bool.Parse(node.Attributes["enable"].Value);
                        if (node.Attributes["remarks"] != null) value_.client.remarks = node.Attributes["remarks"].Value;
                    }
                }

                // 读取 subscribe 节点
                XmlNodeList subscribeNodes = doc.SelectNodes("/root/subscribe/item");
                foreach (XmlNode node in subscribeNodes)
                {
                    nexus_net_subscribe_config_info item = new nexus_net_subscribe_config_info();
                    if (node.Attributes["key"] != null) item.key = node.Attributes["key"].Value;
                    if (node.Attributes["route"] != null) item.route = node.Attributes["route"].Value;
                    if (node.Attributes["enable"] != null) item.enable = bool.Parse(node.Attributes["enable"].Value);
                    if (node.Attributes["remarks"] != null) item.remarks = node.Attributes["remarks"].Value;
                    value_.subscribes.Add(item.key, item);
                }

                // 读取 publisher 节点
                XmlNodeList publisherNodes = doc.SelectNodes("/root/publisher/item");
                foreach (XmlNode node in publisherNodes)
                {
                    nexus_net_publisher_config_info item = new nexus_net_publisher_config_info();
                    if (node.Attributes["key"] != null) item.key = node.Attributes["key"].Value;
                    if (node.Attributes["route"] != null) item.route = node.Attributes["route"].Value;
                    if (node.Attributes["enable"] != null) item.enable = bool.Parse(node.Attributes["enable"].Value);
                    if (node.Attributes["remarks"] != null) item.remarks = node.Attributes["remarks"].Value;
                    value_.publishers.Add(item.key, item);
                }
            }
            return value_;
        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="success"></param>
        /// <param name="is_out"></param>
        /// <param name="value"></param>
        public static void logger(bool success, bool is_out, nexus_net_message value)
        {
            Dictionary<byte, string> g_msg_type = new Dictionary<byte, string>{
                {(byte)nexus_net_msg_type_enum.AUTHENTICATE,     "身份认证"},
                {(byte)nexus_net_msg_type_enum.REPAUTHENTICATE,  "身份认证响应"},
                {(byte)nexus_net_msg_type_enum.SUBSCRIBE,        "订阅"},
                {(byte)nexus_net_msg_type_enum.REPSUBSCRIBE,     "订阅响应"},
                {(byte)nexus_net_msg_type_enum.REMOVESUBSCRIBE,  "删除订阅"},
                {(byte)nexus_net_msg_type_enum.PUBLISHER,        "发布"},
                {(byte)nexus_net_msg_type_enum.REPPUBLISHER,     "发布响应"},
                {(byte)nexus_net_msg_type_enum.HEARTBEAT,        "心跳"},
                {(byte)nexus_net_msg_type_enum.DETAILS,          "详情"},
                {(byte)nexus_net_msg_type_enum.REPDETAILS,       "详情响应"}
            };
            var config = read();
            if (config.loggers.Find(x => x.type == value.msg_type && x.enable) != null)
            {
                if (success)
                {
                    logger_.Info("{0},消息类型:{1} 消息ID:{2} 事件:{3} 路由:{4}", is_out ? "接收成功" : "发送成功", g_msg_type[value.msg_type], value.msg_id, value.route, string.Join('/', value.routes));
                }
                else
                {
                    logger_.Error("{0},消息类型:{1} 消息ID:{2} 事件:{3} 路由:{4}", is_out ? "接收失败" : "发送失败", g_msg_type[value.msg_type], value.msg_id, value.route, string.Join('/', value.routes));
                }
            }
        }
        private static object locker_ = new object();
        private static nexus_net_config_info value_ = null;
    };
    /// <summary>
    /// 特性
    /// </summary>
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public SubscribeAttribute(string value)
        {
            key = value;
        }
        /// <summary>
        /// key
        /// </summary>
        public string key = "";
    }
    /// <summary>
    /// 客户端
    /// </summary>
    public class nexus_net_client
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static Logger logger_ = LogManager.Setup().LoadConfigurationFromFile("config/nlog.config").GetCurrentClassLogger();
        /// <summary>
        /// 消息管理
        /// </summary>
        public class message_manage
        {
            private object locker_ = new object();
            private Dictionary<string, KeyValuePair<SemaphoreSlim, nexus_net_message>> msg_list_ = new Dictionary<string, KeyValuePair<SemaphoreSlim, nexus_net_message>>();
            private static message_manage message_manage_ = new message_manage();
            private message_manage()
            {
            }
            /// <summary>
            /// 单例
            /// </summary>
            /// <returns></returns>
            public static message_manage instance()
            {
                return message_manage_;
            }
            /// <summary>
            /// 添加
            /// </summary>
            /// <param name="msg_id"></param>
            /// <returns></returns>
            public SemaphoreSlim add(string msg_id)
            {
                lock (locker_)
                {
                    msg_list_[msg_id] = new KeyValuePair<SemaphoreSlim, nexus_net_message>(new SemaphoreSlim(0), new nexus_net_message());
                    return msg_list_[msg_id].Key;
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="msg_id"></param>
            /// <returns></returns>
            public nexus_net_message get_data(string msg_id)
            {
                lock (locker_)
                {
                    return msg_list_[msg_id].Value;
                }
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="msg_id"></param>
            /// <param name="data"></param>
            public void set_data(string msg_id, nexus_net_message data)
            {
                lock (locker_)
                {
                    msg_list_[msg_id] = new KeyValuePair<SemaphoreSlim, nexus_net_message>(msg_list_[msg_id].Key, data);
                    msg_list_[msg_id].Key.Release();
                }

            }
            /// <summary>
            /// 查找数据
            /// </summary>
            /// <param name="msg_id"></param>
            /// <returns></returns>
            public bool find(string msg_id)
            {
                lock (locker_)
                {
                    return msg_list_.ContainsKey(msg_id);
                }
            }
            /// <summary>
            /// 移出
            /// </summary>
            /// <param name="msg_id"></param>
            public void remove(string msg_id)
            {
                lock (locker_)
                {
                    msg_list_.Remove(msg_id);
                }
            }
        }
        /// <summary>
        /// 消息管理GC
        /// </summary>
        public class message_manage_gc
        {
            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="data"></param>
            /// <param name="msg_id"></param>
            public message_manage_gc(string msg_id)
            {
                msg_id_ = msg_id;
                wait_ = message_manage.instance().add(msg_id_);
            }
            /// <summary>
            /// 析构
            /// </summary>
            ~message_manage_gc()
            {
                message_manage.instance().remove(msg_id_);
            }
            /// <summary>
            /// 添加
            /// </summary>
            /// <param name="millisecond"></param>
            /// <returns></returns>
            public bool loop(int millisecond)
            {
                return wait_.Wait(millisecond);
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <returns></returns>
            public nexus_net_message get_data()
            {
                return message_manage.instance().get_data(msg_id_);
            }
            private string msg_id_ = "";
            private SemaphoreSlim wait_ = null;
        }
        public delegate void ConnectEvent(bool success);
        public delegate void DisconnectEvent();
        public delegate void AuthorizeEvent(bool success, string parent_code, string parent_name);
        private nexus_net_client()
        {
            client_ = new nexus_net_tcp_client();
            timer_ = new System.Timers.Timer(10000);
            client_.connect_event = on_connect;
            client_.disconnect_event = on_disconnect;
            client_.recv_event = on_recv;
            timer_.Elapsed += on_time_out;
        }
        public static nexus_net_client instance()
        {
            return nexus_net_client_;
        }
        public static nexus_net_message caller_message()
        {
            lock (nexus_net_client_.locker_)
            {
                int thread_id = Thread.CurrentThread.ManagedThreadId;
                if (nexus_net_client_.caller_message_list_.ContainsKey(thread_id))
                {
                    return nexus_net_client_.caller_message_list_[thread_id];
                }
                return null;
            }
        }
        public void start()
        {
            var config = nexus_net_config.read();
            if (config.client.enable)
            {
                timer_.Start();
                client_.start(config.client.ip, config.client.port);
            }
        }
        public void stop()
        {
            client_.stop();
        }
        public void subscribe(params object[] param)
        {
            var config = nexus_net_config.read();
            foreach (object obj in param)
            {
                //反序列化
                Type type = obj.GetType();
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    // 检查方法是否有MyCustomAttribute特性
                    if (method.IsDefined(typeof(SubscribeAttribute), inherit: false))
                    {
                        SubscribeAttribute attribute = method.GetCustomAttribute<SubscribeAttribute>();
                        if (!config.subscribes.ContainsKey(attribute.key))
                        {
                            continue;
                        }
                        if (config.subscribes[attribute.key].enable && !subscribes_.ContainsKey(config.subscribes[attribute.key].route))
                        {
                            List<Tuple<MethodInfo, object>> add = new List<Tuple<MethodInfo, object>>();
                            add.Add(new Tuple<MethodInfo, object>(method, obj));
                            subscribes_[config.subscribes[attribute.key].route] = add;
                            function_manage_.bind(config.subscribes[attribute.key].route, method, obj);
                        }
                    }
                }
            }
        }
        public T publisher<T>(string key, params object[] args) where T : new()
        {
            return publisher<T>(2000, key, args);
        }
        public T publisher<T>(int timeout, string key, params object[] args) where T : new()
        {
            var config = nexus_net_config.read();
            if (!config.publishers.ContainsKey(key))
                throw new Exception("key不存在!");
            nexus_net_message msg = new nexus_net_message();
            msg.route = config.publishers[key].route;
            msg.msg_type = (byte)nexus_net_msg_type_enum.PUBLISHER;
            for (int i = args.Length; i > 0; --i)
            {
                int index = i - 1;
                if (args[index].GetType() == typeof(int))
                {
                    msg.args.serialize((int)args[index]);
                }
                else if (args[index].GetType() == typeof(long))
                {
                    msg.args.serialize((long)args[index]);
                }
                else if (args[index].GetType() == typeof(bool))
                {
                    msg.args.serialize((bool)args[index]);
                }
                else if (args[index].GetType() == typeof(float))
                {
                    msg.args.serialize((float)args[index]);
                }
                else if (args[index].GetType() == typeof(double))
                {
                    msg.args.serialize((double)args[index]);
                }
                else if (args[index].GetType() == typeof(byte))
                {
                    msg.args.serialize((byte)args[index]);
                }
                else if (args[index].GetType() == typeof(DateTime))
                {
                    msg.args.serialize((DateTime)args[index]);
                }
                else if (args[index].GetType() == typeof(string))
                {
                    msg.args.serialize((string)args[index]);
                }
                else if (args[index].GetType() == typeof(List<byte>))
                {
                    msg.args.serialize((List<byte>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<int>))
                {
                    msg.args.serialize((List<int>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<string>))
                {
                    msg.args.serialize((List<string>)args[index]);
                }
                else
                {
                    msg.args.serialize(args[index]);
                }
            }
            if (typeof(T) != typeof(void))
            {
                msg.msg_id = create_message_id();
                message_manage_gc gc = new message_manage_gc(msg.msg_id);
                send(msg);
                if (!gc.loop(timeout))
                    throw new Exception("请求超时");
                object result;
                if (typeof(T) == typeof(int))
                {
                    int tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(long))
                {
                    long tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool tmp = false;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(float))
                {
                    float tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(double))
                {
                    double tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(byte))
                {
                    byte tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    DateTime tmp = new DateTime();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(string))
                {
                    string tmp = "";
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<byte>))
                {
                    List<byte> tmp = new List<byte>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<int>))
                {
                    List<int> tmp = new List<int>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<string>))
                {
                    List<string> tmp = new List<string>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else
                {
                    object tmp = new T();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                return (T)result;
            }
            else
            {
                send(msg);
                return default;
            }
        }
        public void publisher(string key, params object[] args)
        {
            publisher(2000, key, args);
        }
        public void publisher(int timeout, string key, params object[] args)
        {
            var config = nexus_net_config.read();
            if (!config.publishers.ContainsKey(key))
                throw new Exception("key不存在!");
            nexus_net_message msg = new nexus_net_message();
            msg.route = config.publishers[key].route;
            msg.msg_type = (byte)nexus_net_msg_type_enum.PUBLISHER;
            for (int i = args.Length; i > 0; --i)
            {
                int index = i - 1;
                if (args[index].GetType() == typeof(int))
                {
                    msg.args.serialize((int)args[index]);
                }
                else if (args[index].GetType() == typeof(long))
                {
                    msg.args.serialize((long)args[index]);
                }
                else if (args[index].GetType() == typeof(bool))
                {
                    msg.args.serialize((bool)args[index]);
                }
                else if (args[index].GetType() == typeof(float))
                {
                    msg.args.serialize((float)args[index]);
                }
                else if (args[index].GetType() == typeof(double))
                {
                    msg.args.serialize((double)args[index]);
                }
                else if (args[index].GetType() == typeof(byte))
                {
                    msg.args.serialize((byte)args[index]);
                }
                else if (args[index].GetType() == typeof(DateTime))
                {
                    msg.args.serialize((DateTime)args[index]);
                }
                else if (args[index].GetType() == typeof(string))
                {
                    msg.args.serialize((string)args[index]);
                }
                else if (args[index].GetType() == typeof(List<byte>))
                {
                    msg.args.serialize((List<byte>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<int>))
                {
                    msg.args.serialize((List<int>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<string>))
                {
                    msg.args.serialize((List<string>)args[index]);
                }
                else
                {
                    msg.args.serialize(args[index]);
                }
            }

            send(msg);
        }
        public T custom_publisher<T>(string key, string format, params object[] args) where T : new()
        {
            return custom_publisher<T>(2000, key, format, args);
        }
        public T custom_publisher<T>(int timeout, string key, string format, params object[] args) where T : new()
        {
            var config = nexus_net_config.read();
            if (!config.publishers.ContainsKey(key))
                throw new Exception("key不存在!");
            nexus_net_message msg = new nexus_net_message();
            msg.route = string.Format(config.publishers[key].route, format);
            msg.msg_type = (byte)nexus_net_msg_type_enum.PUBLISHER;
            for (int i = args.Length; i > 0; --i)
            {
                int index = i - 1;
                if (args[index].GetType() == typeof(int))
                {
                    msg.args.serialize((int)args[index]);
                }
                else if (args[index].GetType() == typeof(long))
                {
                    msg.args.serialize((long)args[index]);
                }
                else if (args[index].GetType() == typeof(bool))
                {
                    msg.args.serialize((bool)args[index]);
                }
                else if (args[index].GetType() == typeof(float))
                {
                    msg.args.serialize((float)args[index]);
                }
                else if (args[index].GetType() == typeof(double))
                {
                    msg.args.serialize((double)args[index]);
                }
                else if (args[index].GetType() == typeof(byte))
                {
                    msg.args.serialize((byte)args[index]);
                }
                else if (args[index].GetType() == typeof(DateTime))
                {
                    msg.args.serialize((DateTime)args[index]);
                }
                else if (args[index].GetType() == typeof(string))
                {
                    msg.args.serialize((string)args[index]);
                }
                else if (args[index].GetType() == typeof(List<byte>))
                {
                    msg.args.serialize((List<byte>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<int>))
                {
                    msg.args.serialize((List<int>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<string>))
                {
                    msg.args.serialize((List<string>)args[index]);
                }
                else
                {
                    msg.args.serialize(args[index]);
                }
            }
            if (typeof(T) != typeof(void))
            {
                msg.msg_id = create_message_id();
                message_manage_gc gc = new message_manage_gc(msg.msg_id);
                send(msg);
                if (!gc.loop(timeout))
                    throw new Exception("请求超时");
                object result;
                if (typeof(T) == typeof(int))
                {
                    int tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(long))
                {
                    long tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool tmp = false;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(float))
                {
                    float tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(double))
                {
                    double tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(byte))
                {
                    byte tmp = 0;
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    DateTime tmp = new DateTime();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(string))
                {
                    string tmp = "";
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<byte>))
                {
                    List<byte> tmp = new List<byte>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<int>))
                {
                    List<int> tmp = new List<int>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else if (typeof(T) == typeof(List<string>))
                {
                    List<string> tmp = new List<string>();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                else
                {
                    object tmp = new T();
                    gc.get_data().args.deserialize(ref tmp);
                    result = tmp;
                }
                return (T)result;
            }
            else
            {
                send(msg);
                return default;
            }
        }
        public void app_publisher(string key, string nodeName, string appName, params object[] args)
        {
            custom_publisher(2000, key, nodeName, appName, args);
        }
        public void node_publisher(string key, string nodeName, params object[] args)
        {
            custom_publisher(2000, key, nodeName, "*", args);
        }
        public void custom_publisher(int timeout, string key, string nodeName, string appName, params object[] args)
        {
            var config = nexus_net_config.read();
            if (!config.publishers.ContainsKey(key))
                throw new Exception("key不存在!");
            nexus_net_message msg = new nexus_net_message();
            msg.route = string.Format(config.publishers[key].route, nodeName, appName);
            msg.msg_type = (byte)nexus_net_msg_type_enum.PUBLISHER;
            for (int i = args.Length; i > 0; --i)
            {
                int index = i - 1;
                if (args[index].GetType() == typeof(int))
                {
                    msg.args.serialize((int)args[index]);
                }
                else if (args[index].GetType() == typeof(long))
                {
                    msg.args.serialize((long)args[index]);
                }
                else if (args[index].GetType() == typeof(bool))
                {
                    msg.args.serialize((bool)args[index]);
                }
                else if (args[index].GetType() == typeof(float))
                {
                    msg.args.serialize((float)args[index]);
                }
                else if (args[index].GetType() == typeof(double))
                {
                    msg.args.serialize((double)args[index]);
                }
                else if (args[index].GetType() == typeof(byte))
                {
                    msg.args.serialize((byte)args[index]);
                }
                else if (args[index].GetType() == typeof(DateTime))
                {
                    msg.args.serialize((DateTime)args[index]);
                }
                else if (args[index].GetType() == typeof(string))
                {
                    msg.args.serialize((string)args[index]);
                }
                else if (args[index].GetType() == typeof(List<byte>))
                {
                    msg.args.serialize((List<byte>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<int>))
                {
                    msg.args.serialize((List<int>)args[index]);
                }
                else if (args[index].GetType() == typeof(List<string>))
                {
                    msg.args.serialize((List<string>)args[index]);
                }
                else
                {
                    msg.args.serialize(args[index]);
                }
            }

            send(msg);
        }
        private string create_message_id()
        {
            return Guid.NewGuid().ToString();
        }
        private void send(nexus_net_message data)
        {
            if (data.msg_type != (byte)nexus_net_msg_type_enum.REPPUBLISHER)
            {
                var config = nexus_net_config.read();
                data.routes.Add(config.user.code);
                if (data.msg_type == (byte)nexus_net_msg_type_enum.SUBSCRIBE)
                {
                    data.route = string.Format("{0}/{1}/{2}", parent_code_, config.user.code, data.route);
                }
            }
            else if (data.msg_type == (byte)nexus_net_msg_type_enum.REPPUBLISHER && data.routes.Any())
            {
                data.routes.RemoveAt(data.routes.Count - 1);
            }

            if (client_.send(data.get_data()) > 0)
                nexus_net_config.logger(true, false, data);
            else
                nexus_net_config.logger(false, false, data);
        }
        private void on_connect(bool success, string msg)
        {
            if (connect_event != null)
            {
                connect_event(success);
                if (success)
                    authenticate();
            }
        }
        private void on_disconnect()
        {
            if (disconnect_event != null) disconnect_event();
        }
        private void on_recv(List<byte> data)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(do_work_nexus_net_message), new nexus_net_message(data));
        }
        private void heartbeat()
        {
            if (!is_heartbeat_)
                return;
            nexus_net_message msg = new nexus_net_message();
            msg.msg_type = (byte)nexus_net_msg_type_enum.HEARTBEAT;
            send(msg);
        }
        private void on_time_out(object source, ElapsedEventArgs e)
        {
            heartbeat();
        }
        private void do_work_nexus_net_message(object obj)
        {
            nexus_net_message? data = obj as nexus_net_message;
            if (data == null) return;
            nexus_net_config.logger(true, true, data);
            try
            {
                switch ((nexus_net_msg_type_enum)data.msg_type)
                {
                    case nexus_net_msg_type_enum.REPAUTHENTICATE: on_authenticate(data); break;
                    case nexus_net_msg_type_enum.PUBLISHER: on_publisher(data); break;
                    case nexus_net_msg_type_enum.REPPUBLISHER: on_reppublisher(data); break;
                    case nexus_net_msg_type_enum.REPSUBSCRIBE: on_repsubscribe(data); break;
                }
            }
            catch (Exception ec)
            {
                logger_.Error(ec, "处理消息失败");
            }
        }
        private void on_authenticate(nexus_net_message data)
        {
            string node_name = "";
            string message = "";
            data.args.deserialize(ref is_heartbeat_);
            data.args.deserialize(ref parent_code_);
            data.args.deserialize(ref parent_name_);
            data.args.deserialize(ref node_name);
            data.args.deserialize(ref message);
            if (authorize_event != null)
            {
                authorize_event(is_heartbeat_, parent_code_, parent_name_);
                subscribes();
            }
        }
        private void on_publisher(nexus_net_message data)
        {
            if (3 != data.route.Count(i => "/".Contains(i)))
            {
                return;
            }
            string key = data.route.Substring(data.route.IndexOf('/', data.route.IndexOf('/') + 1) + 1);

            lock (locker_)
            {
                caller_message_list_.Add(Thread.CurrentThread.ManagedThreadId, data);
            }
            data.args = function_manage_.invoke(key, data.args);
            lock (locker_)
            {
                caller_message_list_.Remove(Thread.CurrentThread.ManagedThreadId);
            }
            if (!data.args.empty())
            {
                data.msg_type = (byte)nexus_net_msg_type_enum.REPPUBLISHER;
                send(data);
            }
        }
        private void on_reppublisher(nexus_net_message data)
        {
            if (message_manage_.find(data.msg_id))
            {
                message_manage_.set_data(data.msg_id, data);
            }
        }
        private void on_repsubscribe(nexus_net_message data)
        {
            bool success = false;
            data.args.deserialize(ref success);
        }
        private void subscribes()
        {
            var config = nexus_net_config.read();
            foreach (var it in subscribes_)
            {
                nexus_net_message msg = new nexus_net_message();
                msg.msg_type = (byte)nexus_net_msg_type_enum.SUBSCRIBE;
                msg.route = it.Key;
                send(msg);
            }
        }
        private void authenticate()
        {
            var config = nexus_net_config.read();
            if (config.user.enable)
            {
                nexus_net_message data = new nexus_net_message();
                data.msg_type = (byte)nexus_net_msg_type_enum.AUTHENTICATE;
                data.args = data.args << config.user.code << config.user.password << config.user.name << 1;
                send(data);
            }
        }
        private nexus_net_tcp_client client_ { get; set; }
        public ConnectEvent? connect_event { get; set; }
        public DisconnectEvent? disconnect_event { get; set; }
        public AuthorizeEvent? authorize_event { get; set; }
        private System.Timers.Timer timer_ { get; set; }
        private string parent_code_ = "";
        private string parent_name_ = "";
        private bool is_heartbeat_ = false;
        private Dictionary<int, nexus_net_message> caller_message_list_ = new Dictionary<int, nexus_net_message>();
        private object locker_ = new object();
        private message_manage message_manage_ = message_manage.instance();
        private static nexus_net_client nexus_net_client_ = new nexus_net_client();
        private archive_stream.function_manage function_manage_ = new archive_stream.function_manage();
        private Dictionary<string, List<Tuple<MethodInfo, object>>> subscribes_ = new Dictionary<string, List<Tuple<MethodInfo, object>>>();
    }
}
