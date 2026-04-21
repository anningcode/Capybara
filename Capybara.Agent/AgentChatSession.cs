using Capybara.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Capybara.Agent
{
    public class AgentChatSession
    {
        private static object locker_ { get; set; } = new object();
        private AgentUser agentUser_ { get; set; } = new();
        private AgentRole agentRole_ { get; set; } = new();
        private AgentModel agentModel_ { get; set; } = new();
        private AgentTools agentTools_ { get; set; } = new();
        private AgentSkills agentSkills_ { get; set; } = new();
        private AgentPrompt agentPrompt_ { get; set; } = new();
        private AgentChatSessionInfo session_ { get; set; } = new();
        public AgentChatSession(AgentChatMessageInfo request)
        {
            session_.message = request;
        }
        public AgentChatSession(AgentChatSessionInfo session,bool refactor = true)
        {
            session_ = session;
            if (refactor)
            {
                // 添加工具
                session_.request.tools = agentTools_.GetTools(session_.config.tools);
                // 添加提示词
                AddPropmtMessage(session_.config.prompts.Select(n => n.promptValue).ToList());
                // 添加技能
                AddPropmtMessage(agentSkills_.GetSkills(session_.config.skills));
                // 创建智能体
                if (session_.config.tools.Where(n => n.toolName == "create_sub_agent").Count() > 0)
                {
                    var models = AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("isSubAgent", true));
                    string modelPrompts = string.Empty;
                    if (models.Count > 0)
                        modelPrompts = "## 可用模型列表:";
                    foreach (var modelItem in models)
                    {
                        modelPrompts += $"\n - ID:{modelItem.id},模型名称:{modelItem.modelName},备注:{modelItem.remarks}.";
                    }
                    AddPropmtMessage(modelPrompts);
                }
                // 加载智能体
                if (session_.config.tools.Where(n => n.toolName == "load_sub_agent").Count() > 0)
                {
                    var roles = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("id", session_.config.roles[0].subRoleIds));
                    string rolePrompts = string.Empty;
                    if (roles.Count > 0)
                        rolePrompts = "## 可用子智能体:";
                    foreach (var roleItem in roles)
                    {
                        rolePrompts += $"\n - 角色Id:{roleItem.id},智能体名称:{roleItem.name},备注:{roleItem.remarks}.";
                    }
                    AddPropmtMessage(rolePrompts);
                }
            }
        }
        public AgentChatSessionInfo GetSession()
        {
            return session_;
        }
        // 通过用户ID创建智能体
        public bool CreateSession(string userId)
        {
            var user = agentUser_.GetUser(userId);
            if (user == null) return false;
            session_.config.users.Add(user);
            return CreateSession(user.roleId, Guid.NewGuid().ToString());
        }
        // 通过角色ID创建智能体
        public bool CreateSession(int roleId,string agentId)
        {
            try
            {
                // 加载配置
                {
                    // 查询角色
                    var role = agentRole_.GetRole(roleId);
                    if (role == null) throw new Exception("角色不存在");
                    session_.config.roles.Add(role);
                    // 查询模型
                    var model = agentModel_.GetModel(role.modelId);
                    if (model == null) throw new Exception("模型不存在");
                    session_.config.models.Add(model);
                    // 加载提示词
                    session_.config.prompts.AddRange(agentPrompt_.GetPrompts(role.prompts));
                    // 加载技能
                    session_.config.skills.AddRange(agentSkills_.GetSkills(role.skills));
                    // 加载工具
                    session_.config.tools.AddRange(agentTools_.GetTools(role.tools));
                }
                // 基础信息
                {
                    // 智能体ID
                    session_.agentId = agentId;
                    // 智能体名称
                    session_.agentName = session_.config.roles[0].name;
                    // 父智能体
                    session_.parentAgentId = session_.message.parentAgentId;
                }
                // 构建上下文
                {
                    // LLM地址
                    session_.request.address = session_.config.roles[0].llmAddress;
                    // 模型名称
                    session_.request.model = session_.config.models[0].modelName;
                    // 最大token数量
                    session_.request.maxTokens = session_.config.roles[0].maxTokens;
                    // 温度
                    session_.request.temperature = session_.config.roles[0].temperature;
                    // 开启思考
                    session_.request.thinking = true;
                    // 添加工具
                    session_.request.tools = agentTools_.GetTools(session_.config.tools);
                    // 添加提示词
                    AddPropmtMessage(session_.config.prompts.Select(n => n.promptValue).ToList());
                    // 添加技能
                    AddPropmtMessage(agentSkills_.GetSkills(session_.config.skills));
                    // 创建智能体
                    if (session_.config.tools.Where(n => n.toolName == "create_sub_agent").Count() > 0)
                    {
                        var models = AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("isSubAgent", true));
                        string modelPrompts = string.Empty;
                        if (models.Count > 0)
                            modelPrompts = "## 可用模型列表:";
                        foreach (var modelItem in models)
                        {
                            modelPrompts += $"\n - ID:{modelItem.id},模型名称:{modelItem.modelName},备注:{modelItem.remarks}.";
                        }
                        AddPropmtMessage(modelPrompts);
                    }
                    // 加载智能体
                    if (session_.config.tools.Where(n => n.toolName == "load_sub_agent").Count() > 0)
                    {
                        var roles = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("id", session_.config.roles[0].subRoleIds));
                        string rolePrompts = string.Empty;
                        if (roles.Count > 0)
                            rolePrompts = "## 可用子智能体:";
                        foreach (var roleItem in roles)
                        {
                            rolePrompts += $"\n - 角色Id:{roleItem.id},智能体名称:{roleItem.name},备注:{roleItem.remarks}.";
                        }
                        AddPropmtMessage(rolePrompts);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        // 创建session
        public bool CreateSession(AgentChatSessionInfo session)
        {
            try
            {

                return true;
            }
            catch
            {
                return false;
            }
        }
        // 添加用户信息
        public void AddUserMessage(string content)
        {
            if (string.IsNullOrEmpty(content)) return;
            AddMessage("user", content, "", "", new List<AgentLLMItemFuncRequestInfo>());
        }
        // 添加提示词
        public void AddPropmtMessage(List<string> contents)
        {
            foreach (var content in contents)
            {
                AddMessage("system", content, "", "", new List<AgentLLMItemFuncRequestInfo>());
            }
        }
        // 添加提示词列表
        public void AddPropmtMessage(string content)
        {
            if (string.IsNullOrEmpty(content)) return;
            AddMessage("system", content, "", "", new List<AgentLLMItemFuncRequestInfo>());
        }
        // 添加工具信息
        public void AddToolMessage(string content)
        {
            AddMessage("tool", content, "", "", new List<AgentLLMItemFuncRequestInfo>());
        }
        // 添加AI信息
        public void AddAssistantMessage(string think, string answer, List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            AddMessage("assistant", "", think, answer, toolCalls);
        }
        // 设置工具响应
        public bool SetToolResponse(string response)
        {
            bool result = false;
            if (session_.request.context.Count > 0)
            {
                foreach (var item in session_.request.context[session_.request.context.Count - 1].toolCalls)
                {
                    if (item.response == null)
                    {
                        item.response = response;
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        // 添加信息
        private void AddMessage(string role, string content, string think, string answer, List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            if (role == "system")
            {
                for (int i = 0; i < session_.request.context.Count; ++i)
                {
                    if (session_.request.context.Count > i + 1 && session_.request.context[i].role == "system" && session_.request.context[i + 1].role != "system")
                    {
                        session_.request.context.Insert(i + 1, new AgentLLMItemRequestInfo { role = role, content = content, think = think, answer = answer, toolCalls = toolCalls });
                        break;
                    }
                    else if (session_.request.context.Count == i + 1)
                    {
                        session_.request.context.Add(new AgentLLMItemRequestInfo { role = role, content = content, think = think, answer = answer, toolCalls = toolCalls });
                        break;
                    }
                }
                if (session_.request.context.Count == 0)
                {
                    session_.request.context.Add(new AgentLLMItemRequestInfo { role = role, content = content, think = think, answer = answer, toolCalls = toolCalls });
                }
            }
            else
            {
                session_.request.context.Add(new AgentLLMItemRequestInfo { role = role, content = content, think = think, answer = answer, toolCalls = toolCalls });
            }
        }
        // 加载session
        public bool LoadSession()
        {
            return LoadSession(session_.message.agentId, session_.message.parentAgentId);
        }
        // 递减
        public List<string> SubAgentComplete()
        {
            List<string> result = new List<string>();
            lock (locker_)
            {
                try
                {
                    // 读取
                    {
                        string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.message.parentAgentId) ? session_.message.agentId : session_.message.parentAgentId + "/" + session_.message.agentId)}/main.json";
                        if (!File.Exists(path)) throw new Exception();
                        string json = File.ReadAllText(path);
                        var session = JsonConvert.DeserializeObject<AgentChatSessionInfo>(json);
                        if (session == null) throw new Exception();
                        session.message = session_.message;
                        session_ = session;
                    }
                    session_.subAgentCount--;
                    if (session_.subAgentCount == 0)
                    {
                        result = session_.subAgentIds;
                    }
                    // 保存
                    {
                        string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.parentAgentId) ? session_.agentId : session_.parentAgentId + "/" + session_.agentId)}/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                            if (!Directory.Exists(path)) throw new Exception();
                        }
                        File.WriteAllText($"{path}/main.json", JsonConvert.SerializeObject(session_));
                    }
                }
                catch { }
            }
            return result;
        }
        // 加载session
        public bool LoadSession(string agentId,string parentAgentId = "")
        {
            try
            {
                lock (locker_)
                {
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(parentAgentId) ? agentId : parentAgentId + "/" + agentId)}/main.json";
                    if (!File.Exists(path)) throw new Exception();
                    string json = File.ReadAllText(path);
                    var session = JsonConvert.DeserializeObject<AgentChatSessionInfo>(json);
                    if (session == null) throw new Exception();
                    session.message = session_.message;
                    session_ = session;
                    return true;
                }
            }
            catch { }
            return false;
        }
        // 保存session
        public void SaveSession()
        {
            try
            {
                lock (locker_)
                {
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.parentAgentId) ? session_.agentId : session_.parentAgentId + "/" + session_.agentId)}/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        if (!Directory.Exists(path)) throw new Exception();
                    }
                    File.WriteAllText($"{path}/main.json", JsonConvert.SerializeObject(session_));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
