using Capybara.Agent;
using Capybara.IEntrance;
using Capybara.Models;
using Newtonsoft.Json;
using NexusNetNetwork;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Capybara.entrance
{
    public class ChatEntrance
    {
        // 日志
        private static Logger logger_ = LogManager.Setup().LoadConfigurationFromFile("config/nlog.config").GetCurrentClassLogger();
        private List<IChatEntrance> chatEntrances_ { get; set; } = new List<IChatEntrance>();
        private NexusNetInstance nexusNetEntrance_ { get; set; } = NexusNetInstance.Instance();
        private AgentRuntime agentRuntime_ { get; set; }
        public ChatEntrance()
        {
            agentRuntime_ = new AgentRuntime(OnResponse);
            nexusNetEntrance_.onConnect = OnConnect;
            nexusNetEntrance_.onDisconnect = OnDisconnect;
            nexusNetEntrance_.onAuthorize = OnAuthorize;
        }
        public void Init()
        {
            LoadModule();
            nexusNetEntrance_.Start();
        }
        private void OnRequest(AgentChatMessageInfo request)
        {
            if (request.type == 0)
            {
                agentRuntime_.Request(request);
            }
            Console.WriteLine(JsonConvert.SerializeObject(request));
        }
        private bool OnResponse(AgentChatMessageInfo response)
        {
            bool success = false;
            foreach (var item in chatEntrances_)
            {
                if (item.Response(response))
                {
                    success = true;
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return success;
        }
        // 加载模块
        private void LoadModule()
        {
            foreach (var item in Load())
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(item.file);
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.GetInterface(typeof(IChatEntrance).FullName ?? "") != null)
                        {
                            object? instance = Activator.CreateInstance(type, new object[1] { item.param });
                            if (instance == null) continue;
                            if (instance is IChatEntrance chatEntranceInstance)
                            {
                                chatEntranceInstance.onRequest = OnRequest;
                                chatEntrances_.Add(chatEntranceInstance);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger_.Error(ex);
                }
            }
        }
        // 加载配置文件
        private List<AgentChatEntranceInfo> Load()
        {
            try
            {
                XDocument xDoc = XDocument.Load("./config/modules.config");

                if (xDoc.Root == null) throw new Exception("Root异常");
                IEnumerable<XElement> moduleElements = xDoc.Root.Elements("module");

                List<AgentChatEntranceInfo> moduleFiles = new List<AgentChatEntranceInfo>();

                foreach (var entrance in moduleElements)
                {
                    string? fileValue = entrance.Attribute("file")?.Value;
                    if (string.IsNullOrWhiteSpace(fileValue)) continue;
                    string? paramValue = entrance.Attribute("param")?.Value;
                    if (paramValue == null) continue;
                    string? enableValue = entrance.Attribute("enable")?.Value;
                    if (string.IsNullOrWhiteSpace(enableValue) || enableValue != "true") continue;
                    string? remarksValue = entrance.Attribute("remarks")?.Value;
                    moduleFiles.Add(new AgentChatEntranceInfo
                    {
                        file = fileValue,
                        param = paramValue,
                        enable = true,
                        remarks = remarksValue ?? string.Empty
                    });
                }
                return moduleFiles;
            }
            catch (Exception ex)
            {
                logger_.Error(ex);
            }
            return new List<AgentChatEntranceInfo>();
        }
        private void OnConnect(bool status)
        {
            logger_.Info($"连接状态:{status}");
        }
        private void OnDisconnect()
        {
            logger_.Info("连接断开");
        }
        private void OnAuthorize(bool success, string parentCode, string parentName)
        {
            logger_.Info($"授权结果:{success}, 父级编码:{parentCode}, 父级名称:{parentName}");
        }
    }
}
