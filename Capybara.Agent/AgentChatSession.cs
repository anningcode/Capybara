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
            session_.Message = request;
            session_.AgentId = request.AgentId;
            session_.ParentAgentId = request.ParentAgentId;
        }
        public AgentChatSession(AgentChatSessionInfo session,bool refactor = true)
        {
            session_ = session;
            if (refactor)
            {
                // 添加工具
                session_.Request.Tools = agentTools_.GetTools(session_.Config.Tools);
                // 添加提示词
                AddPropmtMessage(session_.Config.Prompts.Select(n => n.PromptValue).ToList());
                // 添加技能
                AddPropmtMessage(agentSkills_.GetSkills(session_.Config.Skills));
                // 创建智能体
                if (session_.Config.Tools.Where(n => n.ToolName == "create_sub_agent").Count() > 0)
                {
                    var models = AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("IsSubAgent", true));
                    string modelPrompts = string.Empty;
                    if (models.Count > 0)
                        modelPrompts = "## 可用模型列表:";
                    foreach (var modelItem in models)
                    {
                        modelPrompts += $"\n - ID:{modelItem.Id},模型名称:{modelItem.ModelName},备注:{modelItem.Remarks}.";
                    }
                    AddPropmtMessage(modelPrompts);
                }
                // 加载智能体
                if (session_.Config.Tools.Where(n => n.ToolName == "load_sub_agent").Count() > 0)
                {
                    var roles = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("Id", session_.Config.Roles[0].SubRoleIds));
                    string rolePrompts = string.Empty;
                    if (roles.Count > 0)
                        rolePrompts = "## 可用子智能体:";
                    foreach (var roleItem in roles)
                    {
                        rolePrompts += $"\n - 角色Id:{roleItem.Id},智能体名称:{roleItem.Name},备注:{roleItem.Remarks}.";
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
            session_.Config.Users.Add(user);
            return CreateSession(user.RoleId, Guid.NewGuid().ToString());
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
                    session_.Config.Roles.Add(role);
                    // 查询模型
                    var model = agentModel_.GetModel(role.ModelId);
                    if (model == null) throw new Exception("模型不存在");
                    session_.Config.Models.Add(model);
                    // 加载提示词
                    session_.Config.Prompts.AddRange(agentPrompt_.GetPrompts(role.Prompts));
                    // 加载技能
                    session_.Config.Skills.AddRange(agentSkills_.GetSkills(role.Skills));
                    // 加载工具
                    session_.Config.Tools.AddRange(agentTools_.GetTools(role.Tools));
                }
                // 基础信息
                {
                    // 智能体ID
                    session_.AgentId = agentId;
                    // 智能体名称
                    session_.AgentName = session_.Config.Roles[0].Name;
                    // 父智能体
                    session_.ParentAgentId = session_.Message.ParentAgentId;
                }
                // 构建上下文
                {
                    // LLM地址
                    session_.Request.Address = session_.Config.Roles[0].LlmAddress;
                    // 模型名称
                    session_.Request.Model = session_.Config.Models[0].ModelName;
                    // 最大token数量
                    session_.Request.MaxTokens = session_.Config.Roles[0].MaxTokens;
                    // 温度
                    session_.Request.Temperature = session_.Config.Roles[0].Temperature;
                    // 开启思考
                    session_.Request.Thinking = true;
                    // 添加工具
                    session_.Request.Tools = agentTools_.GetTools(session_.Config.Tools);
                    // 添加提示词
                    AddPropmtMessage(session_.Config.Prompts.Select(n => n.PromptValue).ToList());
                    // 添加技能
                    AddPropmtMessage(agentSkills_.GetSkills(session_.Config.Skills));
                    // 创建智能体
                    if (session_.Config.Tools.Where(n => n.ToolName == "create_sub_agent").Count() > 0)
                    {
                        var models = AgentConfigManager.GetConfig<AgentChatModelInfo>("models", ("IsSubAgent", true));
                        string modelPrompts = string.Empty;
                        if (models.Count > 0)
                            modelPrompts = "## 可用模型列表:";
                        foreach (var modelItem in models)
                        {
                            modelPrompts += $"\n - ID:{modelItem.Id},模型名称:{modelItem.ModelName},备注:{modelItem.Remarks}.";
                        }
                        AddPropmtMessage(modelPrompts);
                    }
                    // 加载智能体
                    if (session_.Config.Tools.Where(n => n.ToolName == "load_sub_agent").Count() > 0)
                    {
                        var roles = AgentConfigManager.GetConfig<AgentChatRoleInfo>("roles", ("Id", session_.Config.Roles[0].SubRoleIds));
                        string rolePrompts = string.Empty;
                        if (roles.Count > 0)
                            rolePrompts = "## 可用子智能体:";
                        foreach (var roleItem in roles)
                        {
                            rolePrompts += $"\n - 角色Id:{roleItem.Id},智能体名称:{roleItem.Name},备注:{roleItem.Remarks}.";
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
            if (session_.Request.Context.Count > 0)
            {
                foreach (var item in session_.Request.Context[session_.Request.Context.Count - 1].ToolCalls)
                {
                    if (item.Response == null)
                    {
                        item.Response = response;
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
                for (int i = 0; i < session_.Request.Context.Count; ++i)
                {
                    if (session_.Request.Context.Count > i + 1 && session_.Request.Context[i].Role == "system" && session_.Request.Context[i + 1].Role != "system")
                    {
                        session_.Request.Context.Insert(i + 1, new AgentLLMItemRequestInfo { Role = role, Content = content, Think = think, Answer = answer, ToolCalls = toolCalls });
                        break;
                    }
                    else if (session_.Request.Context.Count == i + 1)
                    {
                        session_.Request.Context.Add(new AgentLLMItemRequestInfo { Role = role, Content = content, Think = think, Answer = answer, ToolCalls = toolCalls });
                        break;
                    }
                }
                if (session_.Request.Context.Count == 0)
                {
                    session_.Request.Context.Add(new AgentLLMItemRequestInfo { Role = role, Content = content, Think = think, Answer = answer, ToolCalls = toolCalls });
                }
            }
            else
            {
                session_.Request.Context.Add(new AgentLLMItemRequestInfo { Role = role, Content = content, Think = think, Answer = answer, ToolCalls = toolCalls });
            }
        }
        // 加载session
        public bool LoadSession()
        {
            return LoadSession(session_.AgentId, session_.ParentAgentId);
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
                    session.Message = session_.Message;
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
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        if (!Directory.Exists(path)) throw new Exception();
                    }
                    File.WriteAllText($"{path}main.json", JsonConvert.SerializeObject(session_));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        // 加载子智能体信息
        public bool LoadSubAgentAnswers()
        {
            try
            {
                lock (locker_)
                {
                    List<string> result = new List<string>();
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/subagent.json";
                    if (!File.Exists(path)) throw new Exception();
                    string json = File.ReadAllText(path);
                    var subAgent = JsonConvert.DeserializeObject<AgentChatSubAgentInfo>(json);
                    if (subAgent == null) throw new Exception();

                    if (subAgent.Ids.Count == 0 && subAgent.Count == 0)
                    {
                        result.Add("没有发现智能体.");
                    }
                    else if (subAgent.Ids.Count > 0 && subAgent.Count == 0)
                    {

                        foreach (var item in subAgent.Ids)
                        {
                            string value = LoadSubAgentAnswer(item);
                            result.Add($"智能体ID:{item}, 结论:{value}");
                        }
                    }
                    if (result.Count > 0)
                    {
                        var context = session_.Request.Context;
                        if (context.Count == 0) return false;
                        int index = context.Count - 1;

                        foreach (var item in context[index].ToolCalls)
                        {
                            if (item.Response == null && item.Name == "wait_for_agents")
                            {
                                item.Response = string.Join('\n', result);
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        // 加载子智能体结论
        private string LoadSubAgentAnswer(string subAgentId)
        {
            try
            {
                string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/{subAgentId}/main.json";
                if (!File.Exists(path)) throw new Exception();
                string json = File.ReadAllText(path);
                var session = JsonConvert.DeserializeObject<AgentChatSessionInfo>(json);
                if (session == null) throw new Exception();
                if (session.Request.Context.Count == 0) throw new Exception();
                if (session.Request.Context[session.Request.Context.Count - 1].Role != "assistant") throw new Exception();
                return session.Request.Context[session.Request.Context.Count - 1].Answer;
            }
            catch {  }
            return string.Empty;
        }
        // 添加子智能体
        public void AddSubAgent(string agentId)
        {
            try
            {
                lock (locker_)
                {
                    AgentChatSubAgentInfo? subAgent = new();
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/subagent.json";
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        subAgent = JsonConvert.DeserializeObject<AgentChatSubAgentInfo>(json);
                        if (subAgent == null) throw new Exception();
                    }
                    subAgent.Count++;
                    subAgent.Ids.Add(agentId);
                    File.WriteAllText(path, JsonConvert.SerializeObject(subAgent));
                }
            }
            catch
            {
            }
        }
        // 子智能体完成
        public AgentChatSession? Complete()
        {
            if (string.IsNullOrEmpty(session_.ParentAgentId)) return null;
            try
            {
                AgentChatSubAgentInfo? subAgent = new AgentChatSubAgentInfo();

                lock (locker_)
                {
                    string path = $"capybara/memory/context/{session_.ParentAgentId}/subagent.json";
                    if (!File.Exists(path)) throw new Exception();
                    string json = File.ReadAllText(path);
                    subAgent = JsonConvert.DeserializeObject<AgentChatSubAgentInfo>(json);
                    if (subAgent == null) throw new Exception();
                    subAgent.Count--;
                    File.WriteAllText(path, JsonConvert.SerializeObject(subAgent));
                }

                if (subAgent.Count == 0)
                {
                    List<string> paths = session_.ParentAgentId.Split('/').ToList();
                    string agentId = paths[paths.Count - 1];
                    paths.RemoveAt(paths.Count - 1);
                    AgentChatSession session = new AgentChatSession(new AgentChatMessageInfo { AgentId = agentId, ParentAgentId = string.Join('/', paths), SessionId = session_.Message.SessionId });
                    session.LoadSession();
                    return session;
                }
            }
            catch { }
            return null;
        }
        // 保存规划结果
        public bool SavePlanning(AgentChatPlanningResponseInfo planning)
        {
            try
            {
                lock (locker_)
                {
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        if (!Directory.Exists(path)) throw new Exception();
                    }
                    File.WriteAllText($"{path}planning.json", JsonConvert.SerializeObject(planning));
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        // 更新规划结果
        public AgentChatPlanningResponseInfo? UpdatePlanning(List<(int,string)> status)
        {
            try
            {
                AgentChatPlanningResponseInfo? result = null;
                lock (locker_)
                {
                    string path = $"capybara/memory/context/{(string.IsNullOrEmpty(session_.ParentAgentId) ? session_.AgentId : session_.ParentAgentId + "/" + session_.AgentId)}/planning.json";
                    if (!File.Exists(path)) throw new Exception();
                    string json = File.ReadAllText(path);
                    result = JsonConvert.DeserializeObject<AgentChatPlanningResponseInfo>(json);
                    if (result == null) throw new Exception();

                    foreach (var item in result.Plannings)
                    {
                        foreach (var state in status)
                        {
                            if (item.Id == state.Item1)
                            {
                                item.Type = state.Item2.ToUpper();
                            }
                        }
                    }
                    File.WriteAllText(path, JsonConvert.SerializeObject(result));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
