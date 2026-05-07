using Capybara.Agent;
using Capybara.IEntrance;
using Capybara.Models;
using Capybara.Utils;
using Newtonsoft.Json;
using NexusNetNetwork;
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
            if (request.Type == 0 || request.Type == 1 || request.Type == 2 || request.Type == 3)
            {
                agentRuntime_.Request(request);
            }
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
            return success;
        }
        // 加载模块
        private void LoadModule()
        {
            foreach (var item in Load())
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(item.File);
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.GetInterface(typeof(IChatEntrance).FullName ?? "") != null)
                        {
                            object? instance = Activator.CreateInstance(type, new object[1] { item.Param });
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
                    Logger.Error(ex.Message);
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
                        File = fileValue,
                        Param = paramValue,
                        Enable = true,
                        Remarks = remarksValue ?? string.Empty
                    });
                }
                return moduleFiles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return new List<AgentChatEntranceInfo>();
        }
        private void OnConnect(bool status)
        {
            Logger.Info($"连接状态:{status}");
        }
        private void OnDisconnect()
        {
            Logger.Info("连接断开");
        }
        private void OnAuthorize(bool success, string parentCode, string parentName)
        {
            Logger.Info($"授权结果:{success}, 父级编码:{parentCode}, 父级名称:{parentName}");
        }
    }
}
