using Capybara.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Capybara.Agent
{
    public class AgentRuntime
    {
        // 工具
        private AgentToolsManager toolsManager_ { get; set; } = AgentToolsManager.Instance;
        // 响应回调
        public Func<AgentChatMessageInfo, bool> onResponse { get; set; }
        // 构造
        public AgentRuntime(Func<AgentChatMessageInfo, bool> callback)
        {
            onResponse = callback;
        }
        // 请求
        // 请求
        public void Request(AgentChatMessageInfo request)
        {

            if (request.type == AgentChatQuestionRequestInfo.type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatQuestionRequestInfo>(request.data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.type, new AgentChatErrorResponseInfo { message = "收到用户问题,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.type == AgentChatToolConfirmationRequestInfo.type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatToolConfirmationRequestInfo>(request.data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.type, new AgentChatErrorResponseInfo { message = "收到工具确认,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.type == AgentChatSkillConfirmationRequestInfo.type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatSkillConfirmationRequestInfo>(request.data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.type, new AgentChatErrorResponseInfo { message = "收到技能确认,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.type == AgentChatSelectRequestInfo.type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatSelectRequestInfo>(request.data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.type, new AgentChatErrorResponseInfo { message = "收到用户选择,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            Response(request, AgentChatTaskResponseInfo.type, new AgentChatTaskResponseInfo { content = "收到请求" });
        }
        // 问题
        private void Request(AgentChatMessageInfo request, AgentChatQuestionRequestInfo question)
        {
            AgentChatSession session = new AgentChatSession(request);
            if (string.IsNullOrEmpty(request.agentId) || !session.LoadSession())
            {
                if (!session.CreateSession(request.userId))
                {
                    return;
                }
            }
            session.GetSession().msgId = Guid.NewGuid().ToString();
            session.AddUserMessage(question.content);
            Request(session);
        }
        // 工具
        private void Request(AgentChatMessageInfo request, AgentChatToolConfirmationRequestInfo content)
        {
            AgentChatSession session = new AgentChatSession(request);
            if (!session.LoadSession())
            {
                // 异常session不存在
                return;
            }
            Request(session);
        }
        // 技能
        private void Request(AgentChatMessageInfo request, AgentChatSkillConfirmationRequestInfo content)
        {
            AgentChatSession session = new AgentChatSession(request);
            if (!session.LoadSession())
            {
                // 异常session不存在
                return;
            }
            Request(session);
        }
        // 疑问
        private void Request(AgentChatMessageInfo request, AgentChatSelectRequestInfo content)
        {
            AgentChatSession session = new AgentChatSession(request);
            if (!session.LoadSession())
            {
                // 异常 session不存在
                return;
            }
            Request(session);
        }
        // 请求
        private void Request(AgentChatSession session)
        {
            Task.Run(() => Planning(session));
        }
        // 规划任务
        private void Planning(AgentChatSession session)
        {
            var context = session.GetSession().request.context;
            var type = session.GetSession().message.type;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            // 用户询问
            if (context[index].role == "user")
            {
                ExecuteQuestion(session);
            }
            // 上传文件
            else if (context[index].role == "assistant" && IsDownloadFile(context[index].toolCalls))
            {
                OnDownloadFile(session, context[index].toolCalls.Where(n => n.name == "add_download_file" && n.response == null).ToList()[0]);
            }
            // 规划任务
            else if (context[index].role == "assistant" && IsTaskPlanning(context[index].toolCalls))
            {
                OnTaskPlanning(session, context[index].toolCalls.Where(n => n.name == "task_planning" && n.response == null).ToList()[0]);
            }
            // 更新任务
            else if (context[index].role == "assistant" && IsTaskUpdate(context[index].toolCalls))
            {
                OnTaskUpdate(session, context[index].toolCalls.Where(n => n.name == "task_update" && n.response == null).ToList()[0]);
            }
            // 用户选择
            else if (context[index].role == "assistant" && IsAskUser(context[index].toolCalls))
            {
                OnAskUser(session, context[index].toolCalls.Where(n => n.name == "ask_user" && n.response == null).ToList()[0]);
            }
            // 创建子智能体
            else if (context[index].role == "assistant" && IsCreateSubAgent(context[index].toolCalls))
            {
                OnCreateSubAgent(session, context[index].toolCalls.Where(n => n.name == "create_sub_agent" && n.response == null).ToList()[0]);
            }
            // 加载子智能体
            else if (context[index].role == "assistant" && IsLoadSubAgent(context[index].toolCalls))
            {
                OnLoadSubAgent(session, context[index].toolCalls.Where(n => n.name == "load_sub_agent" && n.response == null).ToList()[0]);
            }
            // 复用子智能体
            else if (context[index].role == "assistant" && IsReuseSubAgent(context[index].toolCalls))
            {
                OnReuseSubAgent(session, context[index].toolCalls.Where(n => n.name == "reuse_sub_agent" && n.response == null).ToList()[0]);
            }
            // 等待子智能体
            else if (context[index].role == "assistant" && IsWaitForAgent(context[index].toolCalls))
            {
                OnWaitForAgent(session, context[index].toolCalls.Where(n => n.name == "wait_for_agents" && n.response == null).ToList()[0]);
            }
            // 工具调用
            else if (context[index].role == "assistant" && IsTools(context[index].toolCalls))
            {
                ExecuteTools(session);
            }
            // 技能调用
            else if (context[index].role == "assistant" && IsSkills(context[index].toolCalls))
            {
                ExecuteSkills(session);
            }
            // 工具执行完成提交给大模型继续执行
            else if (context[index].role == "assistant" && context[index].toolCalls.Count > 0)
            {
                ExecuteQuestion(session);
            }
            else
            {
                Response(session, AgentChatEndResponseInfo.type, new AgentChatEndResponseInfo { content = "结束" });
                if (!string.IsNullOrEmpty(session.GetSession().parentAgentId))
                {
                    OnActivateParentAgent(session);
                }
                else
                {
                    Response(session, AgentChatEndAllResponseInfo.type, new AgentChatEndAllResponseInfo { content = "全部结束" });
                }
            }
            // 保存
            session.SaveSession();
        }
        // 执行请求
        private void ExecuteQuestion(AgentChatSession session)
        {
            AgentExecutor agentExecutor = new AgentExecutor(session, OnResponse);
            var response = agentExecutor.Request();
            if (response.success)
            {
                session.AddAssistantMessage(response.think, response.answer, response.toolCalls);
                session.GetSession().message.type = -1;
                Request(session);
            }
            else 
            {
                Response(session, AgentChatErrorResponseInfo.type, new AgentChatErrorResponseInfo { message = response.message });
            }
        }
        // 执行工具
        private void ExecuteTools(AgentChatSession session)
        {
            var context = session.GetSession().request.context;
            if (context.Count == 0) return;
            int index = context.Count - 1;
            foreach (var call in context[index].toolCalls)
            {
                if (call.response == null)
                {
                    var tools = session.GetSession().config.tools.Where(n => n.toolName == call.name).ToList();
                    // 没有这个工具,不允许执行
                    if (tools.Count <= 0)
                    {
                        Response(session, AgentChatToolCallResponseInfo.type, new AgentChatToolCallResponseInfo { confirmation = false, toolName = call.name, toolParam = call.arguments });
                        call.response = "本次用户不允许调用这个工具";
                        Response(session, AgentChatToolResponseInfo.type, new AgentChatToolResponseInfo { toolName = call.name, response = "执行失败没有找到这个工具!" });
                    }
                    // 这个工具需要授权
                    else if (tools[0].confirm)
                    {
                        if (session.GetSession().message.type == AgentChatToolConfirmationRequestInfo.type)
                        {
                            var value = JsonConvert.DeserializeObject<AgentChatToolConfirmationRequestInfo>(session.GetSession().message.data) ?? new();
                            call.response = value.allow ? toolsManager_.Invoke(call) : "本次用户不允许调用这个工具";
                            Response(session, AgentChatToolResponseInfo.type, new AgentChatToolResponseInfo { toolName = call.name, response = string.IsNullOrWhiteSpace(call.response) ? "没有内容." : call.response });
                        }
                        else
                        {
                            Response(session, AgentChatToolCallResponseInfo.type, new AgentChatToolCallResponseInfo { confirmation = true, toolName = call.name, toolParam = call.arguments });
                            return;
                        }
                    }
                    // 这个工具不需要授权直接调用
                    else
                    {
                        Response(session, AgentChatToolCallResponseInfo.type, new AgentChatToolCallResponseInfo { confirmation = false, toolName = call.name, toolParam = call.arguments });
                        call.response = toolsManager_.Invoke(call);
                        Response(session, AgentChatToolResponseInfo.type, new AgentChatToolResponseInfo { toolName = call.name, response = string.IsNullOrWhiteSpace(call.response) ? "没有内容." : call.response });
                    }
                    // 执行完成丢给规划方法
                    session.GetSession().message.type = -1;
                    Request(session);
                    return;
                }
            }
        }
        // 执行技能
        private void ExecuteSkills(AgentChatSession session)
        {
            var context = session.GetSession().request.context;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            foreach (var call in context[index].toolCalls)
            {
                if (call.response == null)
                {
                    var tools = session.GetSession().config.tools.Where(n => n.toolName == call.name).ToList();
                    // 没有这个技能,不允许执行
                    if (tools.Count <= 0)
                    {
                        Response(session, AgentChatToolCallResponseInfo.type, new AgentChatToolCallResponseInfo { confirmation = false, toolName = call.name, toolParam = call.arguments });
                        call.response = "本次用户不允许调用这个技能";
                        Response(session, AgentChatToolResponseInfo.type, new AgentChatToolResponseInfo { toolName = call.name, response = "执行失败没有找到这个技能!" });
                    }
                    else
                    {
                        try 
                        {
                            var json = JObject.Parse(call.arguments);
                            if (!json.ContainsKey("skill")) throw new Exception();
                            string? skillName = json["skill"]?.ToObject<string>();
                            if(skillName == null) throw new Exception();
                            var skills = session.GetSession().config.skills.Where(n => n.skillName == skillName).ToList();


                            // 没有这个技能,不允许执行
                            if (skills.Count <= 0)
                            {
                                Response(session, AgentChatSkillCallResponseInfo.type, new AgentChatSkillCallResponseInfo { confirmation = false, skillName = call.name, skillParam = call.arguments });
                                call.response = "本次用户不允许调用这个技能";
                                Response(session, AgentChatSkillCallResponseInfo.type, new AgentChatSkillResponseInfo { skillName = call.name, response = "执行失败没有找到这个技能!" });
                            }
                            // 这个技能需要授权
                            else if (skills[0].confirm)
                            {
                                if (session.GetSession().message.type == AgentChatSkillConfirmationRequestInfo.type)
                                {
                                    var value = JsonConvert.DeserializeObject<AgentChatSkillConfirmationRequestInfo>(session.GetSession().message.data) ?? new();
                                    call.response = value.allow ? toolsManager_.Invoke(call) : "本次用户不允许调用这个技能";
                                    Response(session, AgentChatSkillResponseInfo.type, new AgentChatSkillResponseInfo { skillName = call.name, response = string.IsNullOrWhiteSpace(call.response) ? "没有内容." : call.response });
                                }
                                else
                                {
                                    Response(session, AgentChatSkillCallResponseInfo.type, new AgentChatSkillCallResponseInfo { confirmation = true, skillName = call.name, skillParam = call.arguments });
                                    return;
                                }
                            }
                            // 这个技能不需要授权直接调用
                            else
                            {
                                Response(session, AgentChatSkillCallResponseInfo.type, new AgentChatSkillCallResponseInfo { confirmation = false, skillName = call.name, skillParam = call.arguments });
                                call.response = toolsManager_.Invoke(call);
                                Response(session, AgentChatSkillResponseInfo.type, new AgentChatSkillResponseInfo { skillName = call.name, response = string.IsNullOrWhiteSpace(call.response) ? "没有内容." : call.response });
                            }
                        } 
                        catch 
                        {

                        }
                    }
                    // 执行完成丢给规划方法
                    session.GetSession().message.type = -1;
                    Request(session);
                    return;
                }
            }
        }
        // 文件上传
        private bool IsDownloadFile(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "add_download_file";
                }
            }
            return false;
        }
        // 选择
        private bool IsAskUser(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "ask_user";
                }
            }
            return false;
        }
        // 创建智能体
        private bool IsCreateSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "create_sub_agent";
                }
            }
            return false;
        }
        // 加载智能体
        private bool IsLoadSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "load_sub_agent";
                }
            }
            return false;
        }
        // 复用智能体
        private bool IsReuseSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "reuse_sub_agent";
                }
            }
            return false;
        }
        // 等待子智能体
        private bool IsWaitForAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "wait_for_agents";
                }
            }
            return false;
        }
        // 判断是不是调用工具
        private bool IsTools(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name != "load_skill" &&
                        item.name != "execute_skill_script" &&
                        item.name != "wait_for_agents" &&
                        item.name != "ask_user" &&
                        item.name != "create_sub_agent" &&
                        item.name != "load_sub_agent" &&
                        item.name != "reuse_sub_agent" &&
                        item.name != "task_planning" &&
                        item.name != "task_update" &&
                        item.name != "add_download_file";
                }
            }
            return false;
        }
        // 判断是不是调用技能
        private bool IsSkills(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "load_skill" || item.name == "execute_skill_script";
                }
            }
            return false;
        }
        // 判断是不是规划
        private bool IsTaskPlanning(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "task_planning";
                }
            }
            return false;
        }
        // 判断是不是更新任务
        private bool IsTaskUpdate(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.response == null)
                {
                    return item.name == "task_update";
                }
            }
            return false;
        }
        // 激活父智能体
        private void OnActivateParentAgent(AgentChatSession session)
        {
            var sessionParent = session.Complete();
            if (sessionParent == null) return;

            var context = sessionParent.GetSession().request.context;
            var type = sessionParent.GetSession().message.type;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            if (!IsWaitForAgent(context[index].toolCalls)) return;

            try
            {
                OnWaitForAgent(sessionParent, context[index].toolCalls.Where(n => n.name == "wait_for_agents" && n.response == null).ToList()[0]);
            }
            catch { }
        }
        // 添加到下载列表
        private void OnDownloadFile(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("path")) throw new Exception();
                string? path = json["path"]?.ToObject<string>();
                if (path == null) throw new Exception();

                if (File.Exists(path))
                {
                    byte[] fileBytes = File.ReadAllBytes(path);
                    string base64String = Convert.ToBase64String(fileBytes);
                    Response(session, AgentChatUploadResponseInfo.type, new AgentChatUploadResponseInfo { name = (new FileInfo(path)).Name, data = base64String });

                    toolCall.response = "添加成功!";
                    session.GetSession().message.type = -1;
                    Request(session);
                }
                else
                {
                    toolCall.response = "添加失败,文件不存在!";
                    session.GetSession().message.type = -1;
                    Request(session);
                }
            }
            catch
            {
                toolCall.response = "调用失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 等待子智能体
        private void OnWaitForAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            if (!session.LoadSubAgentAnswers()) return;

            session.GetSession().message.type = -1;
            Request(session);
        }
        // AI疑问,列出选项供用户选择解答疑问
        private void OnAskUser(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                if (session.GetSession().message.type != AgentChatSelectRequestInfo.type)
                {
                    var json = JObject.Parse(toolCall.arguments);
                    if (json == null) throw new Exception();
                    if (!json.ContainsKey("options") || !json.ContainsKey("single") || !json.ContainsKey("title")) throw new Exception();
                    string? title = json["title"]?.ToObject<string>();
                    List<string>? options = json["options"]?.ToObject<List<string>>();
                    bool? single = json["single"]?.ToObject<bool>();
                    if (title == null || options == null || single == null) throw new Exception();

                    AgentChatSelectResponseInfo param = new AgentChatSelectResponseInfo();
                    foreach (var item in options)
                    {
                        int index = item.IndexOf(':');
                        if(index == -1) throw new Exception();

                        var key = item.Substring(0, index);
                        var value = item.Substring(index + 1);

                        AgentChatSelectItemInfo addItem = new AgentChatSelectItemInfo();
                        addItem.type = key.ToUpper() == "SELECT" ? true : false;
                        addItem.content = value;
                        param.options.Add(addItem);
                    }
                    param.single = (bool)single;
                    param.title = title;

                    Response(session, AgentChatSelectResponseInfo.type, param);
                }
                else
                {
                    var value = JsonConvert.DeserializeObject<AgentChatSelectRequestInfo>(session.GetSession().message.data);
                    if (value == null) throw new Exception();
                    toolCall.response = string.Join('\n', value.options);
                    if (value.options.Count == 0) toolCall.response = "用户没有提供选择!";
                    session.GetSession().message.type = -1;
                    Request(session);
                }
            }
            catch
            {
                toolCall.response = "调用失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 智能体规划任务
        private void OnTaskPlanning(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("tasks")) throw new Exception();
                List<string>? list = json["tasks"]?.ToObject<List<string>>();
                if (list == null) throw new Exception();

                AgentChatPlanningResponseInfo param = new();
                foreach (var item in list)
                {
                    AgentChatPlanningItemInfo addItem = new();
                    int index = item.IndexOf('.');
                    if (index == -1) throw new Exception();
                    string content = item;

                    addItem.id = param.plannings.Count + 1;
                    addItem.type = "PENDING";
                    addItem.content = content;
                    param.plannings.Add(addItem);
                }

                // 保存
                if(!session.SavePlanning(param)) throw new Exception();

                Response(session, AgentChatPlanningResponseInfo.type, param);

                toolCall.response = "执行成功";
                session.GetSession().message.type = -1;
                Request(session);
            }
            catch
            {
                toolCall.response = "调用失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 智能体更新任务
        private void OnTaskUpdate(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("tasks")) throw new Exception();
                List<string>? list = json["tasks"]?.ToObject<List<string>>();
                if (list == null) throw new Exception();

                List<(int,string)> param = new();
                foreach (var item in list)
                {
                    var values = item.Split(':');
                    if (values.Length != 2) throw new Exception();

                    param.Add((int.Parse(values[0]), values[1]));
                }

                // 加载
                var planning = session.UpdatePlanning(param);
                if (planning == null) throw new Exception();

                Response(session, AgentChatPlanningResponseInfo.type, planning);

                toolCall.response = "执行成功";
                session.GetSession().message.type = -1;
                Request(session);
            }
            catch
            {
                toolCall.response = "调用失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 创建子智能体
        private void OnCreateSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("agentName") ||
                    !json.ContainsKey("content") ||
                    !json.ContainsKey("temperature") ||
                    !json.ContainsKey("maxTokens") ||
                    !json.ContainsKey("model")) throw new Exception();

                string? agentName = json["agentName"]?.ToObject<string>();
                string? content = json["content"]?.ToObject<string>();
                double? temperature = json["temperature"]?.ToObject<double>();
                int? maxTokens = json["maxTokens"]?.ToObject<int>();
                string? model = json["model"]?.ToObject<string>();
                string? prompt = json.ContainsKey("prompt") ? json["prompt"]?.ToObject<string>() : "";
                List<string>? skills = json.ContainsKey("skills") ? json["skills"]?.ToObject<List<string>>() : new List<string>();
                List<string>? tools = json.ContainsKey("tools") ? json["tools"]?.ToObject<List<string>>() : new List<string>();

                if (agentName == null ||
                    content == null ||
                    temperature == null ||
                    maxTokens == null ||
                    model == null ||
                    prompt == null ||
                    skills == null ||
                    tools == null) throw new Exception();

                // 智能体ID
                string agentId = Guid.NewGuid().ToString();
                // 消息
                AgentChatMessageInfo message = new AgentChatMessageInfo
                {
                    userId = "",
                    sessionId = session.GetSession().message.sessionId,
                    agentId = agentId,
                    agentName = agentName,
                    msgId = "",
                    type = AgentChatQuestionRequestInfo.type,
                    data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = content })
                };
                // 配置
                List<AgentChatSkillInfo> skillsValue = new List<AgentChatSkillInfo>();
                foreach (var item1 in skills)
                {
                    foreach (var item2 in session.GetSession().config.skills)
                    {
                        if (item1 == item2.skillName)
                        {
                            skillsValue.Add(item2);
                        }
                    }
                }
                List<AgentChatToolInfo> toolsValue = new List<AgentChatToolInfo>();
                foreach (var item1 in tools)
                {
                    foreach (var item2 in session.GetSession().config.tools)
                    {
                        if (item1 == item2.toolName)
                        {
                            toolsValue.Add(item2);
                        }
                    }
                }
                AgentChatConfigInfo config = new AgentChatConfigInfo
                {
                    users = new List<AgentChatUserInfo> { },
                    roles = new List<AgentChatRoleInfo> { { new AgentChatRoleInfo { name = agentName, llmAddress = session.GetSession().config.roles[0].llmAddress, temperature = (double)temperature, maxTokens = (int)maxTokens } } },
                    models = new List<AgentChatModelInfo> { { new AgentChatModelInfo { modelName = model } } },
                    prompts = new List<AgentChatPromptInfo> { { new AgentChatPromptInfo { promptValue = prompt } } },
                    skills = skillsValue,
                    tools = toolsValue
                };
                // 上下文
                AgentLLMRequestInfo request = new AgentLLMRequestInfo();
                // LLM地址
                request.address = session.GetSession().config.roles[0].llmAddress;
                // 模型名称
                request.model = session.GetSession().config.models[0].modelName;
                // 最大token数量
                request.maxTokens = session.GetSession().config.roles[0].maxTokens;
                // 温度
                request.temperature = session.GetSession().config.roles[0].temperature;
                // 开启思考
                request.thinking = true;
                // 添加提示词
                if (!string.IsNullOrEmpty(prompt))
                {
                    request.context.Add(new AgentLLMItemRequestInfo
                    {
                        content = prompt,
                        role = "system"
                    });
                }
                // 添加问题
                request.context.Add(new AgentLLMItemRequestInfo
                {
                    content = content,
                    role = "user"
                });
                // 参数
                AgentChatSessionInfo param = new AgentChatSessionInfo
                {
                    agentId = agentId,
                    agentName = agentName,
                    parentAgentId = string.IsNullOrEmpty(session.GetSession().parentAgentId) ? session.GetSession().agentId : session.GetSession().parentAgentId + "/" + session.GetSession().agentId,
                    config = config,
                    message = message,
                    request = request
                };

                AgentChatSession newSession = new(param);
                newSession.GetSession().msgId = Guid.NewGuid().ToString();
                Request(newSession);

                toolCall.response = $"创建成功,智能体ID: {agentId}";
               
                Console.WriteLine("====================================================================");

                session.GetSession().message.type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch 
            {
                toolCall.response = "创建失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 加载子智能体
        private void OnLoadSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("id") || !json.ContainsKey("content")) throw new Exception();
                int? id = json["id"]?.ToObject<int>();
                string? content = json["content"]?.ToObject<string>();
                if (id == null || content == null) throw new Exception();

                string agentId = Guid.NewGuid().ToString();

                AgentChatSession newSession = new(new AgentChatMessageInfo
                {
                    agentId = agentId,
                    parentAgentId = session.GetSession().agentId,
                    sessionId = session.GetSession().message.sessionId,
                    type = AgentChatQuestionRequestInfo.type,
                    data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = content })
                });

                if (newSession.CreateSession((int)id, agentId))
                {
                    newSession.GetSession().msgId = Guid.NewGuid().ToString();
                    Request(newSession);
                    toolCall.response = $"加载成功,智能体ID: {agentId}";
                }
                else
                {
                    toolCall.response = "加载失败,角色不存在!";
                }

                session.GetSession().message.type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch 
            {
                toolCall.response = "加载失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 复用子智能体
        private void OnReuseSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("agentId") || !json.ContainsKey("content")) throw new Exception();
                string? agentId = json["agentId"]?.ToObject<string>();
                string? content = json["content"]?.ToObject<string>();
                if (agentId == null || content == null) throw new Exception();

                AgentChatSession newSession = new(new AgentChatMessageInfo
                {
                    agentId = agentId,
                    parentAgentId = session.GetSession().agentId,
                    sessionId = session.GetSession().message.sessionId,
                    type = AgentChatQuestionRequestInfo.type,
                    data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = content })
                });

                if (newSession.LoadSession())
                {
                    newSession.GetSession().msgId = Guid.NewGuid().ToString();
                    Request(newSession);
                    toolCall.response = "复用成功!";
                }
                else
                {
                    toolCall.response = "复用失败!";
                }

                session.GetSession().message.type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch
            {
                toolCall.response = "加载失败,参数错误!";
                session.GetSession().message.type = -1;
                Request(session);
            }
        }
        // 响应
        private bool OnResponse(AgentLLMResponseInfo response, AgentChatSession session)
        {
            if (!string.IsNullOrEmpty(response.think))
            {
                return Response(session, AgentChatThinkResponseInfo.type, new AgentChatThinkResponseInfo { content = response.think });
            }
            else
            {
                return Response(session, AgentChatAnswerResponseInfo.type, new AgentChatAnswerResponseInfo { content = response.answer });
            }
        }
        // 响应
        private bool Response(AgentChatMessageInfo request, int type, object t)
        {
            AgentChatMessageInfo response = new AgentChatMessageInfo();
            response.sessionId = request.sessionId;
            response.agentId = request.agentId;
            response.agentName = request.agentName;
            response.msgId = request.msgId;
            response.parentAgentId = request.parentAgentId;
            response.type = type;
            response.data = JsonConvert.SerializeObject(t);
            return onResponse.Invoke(response);
        }
        // 响应
        private bool Response(AgentChatSession session,int type, object t)
        {
            AgentChatMessageInfo response = new AgentChatMessageInfo();
            response.sessionId = session.GetSession().message.sessionId;
            response.agentId = session.GetSession().agentId;
            response.agentName = session.GetSession().agentName;
            response.msgId = session.GetSession().msgId;
            response.parentAgentId = session.GetSession().parentAgentId;
            response.type = type;
            // 回复收到消息
            if (response.type == AgentChatTaskResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 思考
            else if (response.type == AgentChatThinkResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 结论
            else if (response.type == AgentChatAnswerResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 工具调用
            else if (response.type == AgentChatToolCallResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 工具响应
            else if (response.type == AgentChatToolResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 技能调用
            else if (response.type == AgentChatSkillCallResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 技能响应
            else if (response.type == AgentChatSkillResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 选择
            else if (response.type == AgentChatSelectResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 上传文件
            else if (response.type == AgentChatUploadResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 规划响应
            else if (response.type == AgentChatPlanningResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 错误
            else if (response.type == AgentChatErrorResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 智能体结束
            else if (response.type == AgentChatEndResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            // 全部结束
            else if (response.type == AgentChatEndAllResponseInfo.type)
            {
                response.data = JsonConvert.SerializeObject(t);
            }
            return onResponse.Invoke(response);
        }
    }
}
