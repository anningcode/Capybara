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

            if (request.Type == AgentChatQuestionRequestInfo.Type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatQuestionRequestInfo>(request.Data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.Type, new AgentChatErrorResponseInfo { Message = "收到用户问题,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.Type == AgentChatToolConfirmationRequestInfo.Type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatToolConfirmationRequestInfo>(request.Data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.Type, new AgentChatErrorResponseInfo { Message = "收到工具确认,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.Type == AgentChatSkillConfirmationRequestInfo.Type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatSkillConfirmationRequestInfo>(request.Data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.Type, new AgentChatErrorResponseInfo { Message = "收到技能确认,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            else if (request.Type == AgentChatSelectRequestInfo.Type)
            {
                var value = JsonConvert.DeserializeObject<AgentChatSelectRequestInfo>(request.Data);
                if (value == null)
                {
                    Response(request, AgentChatErrorResponseInfo.Type, new AgentChatErrorResponseInfo { Message = "收到用户选择,失败反序列化为空!" });
                    return;
                }
                Request(request, value);
            }
            Response(request, AgentChatTaskResponseInfo.Type, new AgentChatTaskResponseInfo { Content = "收到请求" });
        }
        // 问题
        private void Request(AgentChatMessageInfo request, AgentChatQuestionRequestInfo question)
        {
            AgentChatSession session = new AgentChatSession(request);
            if (string.IsNullOrEmpty(request.AgentId) || !session.LoadSession())
            {
                if (!session.CreateSession(request.UserId))
                {
                    return;
                }
            }
            session.GetSession().MsgId = Guid.NewGuid().ToString();
            session.AddUserMessage(question.Content);
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
            var context = session.GetSession().Request.Context;
            var type = session.GetSession().Message.Type;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            // 用户询问
            if (context[index].Role == "user")
            {
                ExecuteQuestion(session);
            }
            // 上传文件
            else if (context[index].Role == "assistant" && IsDownloadFile(context[index].ToolCalls))
            {
                OnDownloadFile(session, context[index].ToolCalls.Where(n => n.Name == "add_download_file" && n.Response == null).ToList()[0]);
            }
            // 规划任务
            else if (context[index].Role == "assistant" && IsTaskPlanning(context[index].ToolCalls))
            {
                OnTaskPlanning(session, context[index].ToolCalls.Where(n => n.Name == "task_planning" && n.Response == null).ToList()[0]);
            }
            // 更新任务
            else if (context[index].Role == "assistant" && IsTaskUpdate(context[index].ToolCalls))
            {
                OnTaskUpdate(session, context[index].ToolCalls.Where(n => n.Name == "task_update" && n.Response == null).ToList()[0]);
            }
            // 用户选择
            else if (context[index].Role == "assistant" && IsAskUser(context[index].ToolCalls))
            {
                OnAskUser(session, context[index].ToolCalls.Where(n => n.Name == "ask_user" && n.Response == null).ToList()[0]);
            }
            // 创建子智能体
            else if (context[index].Role == "assistant" && IsCreateSubAgent(context[index].ToolCalls))
            {
                OnCreateSubAgent(session, context[index].ToolCalls.Where(n => n.Name == "create_sub_agent" && n.Response == null).ToList()[0]);
            }
            // 加载子智能体
            else if (context[index].Role == "assistant" && IsLoadSubAgent(context[index].ToolCalls))
            {
                OnLoadSubAgent(session, context[index].ToolCalls.Where(n => n.Name == "load_sub_agent" && n.Response == null).ToList()[0]);
            }
            // 复用子智能体
            else if (context[index].Role == "assistant" && IsReuseSubAgent(context[index].ToolCalls))
            {
                OnReuseSubAgent(session, context[index].ToolCalls.Where(n => n.Name == "reuse_sub_agent" && n.Response == null).ToList()[0]);
            }
            // 等待子智能体
            else if (context[index].Role == "assistant" && IsWaitForAgent(context[index].ToolCalls))
            {
                OnWaitForAgent(session, context[index].ToolCalls.Where(n => n.Name == "wait_for_agents" && n.Response == null).ToList()[0]);
            }
            // 工具调用
            else if (context[index].Role == "assistant" && IsTools(context[index].ToolCalls))
            {
                ExecuteTools(session);
            }
            // 技能调用
            else if (context[index].Role == "assistant" && IsSkills(context[index].ToolCalls))
            {
                ExecuteSkills(session);
            }
            // 工具执行完成提交给大模型继续执行
            else if (context[index].Role == "assistant" && context[index].ToolCalls.Count > 0)
            {
                ExecuteQuestion(session);
            }
            else
            {
                Response(session, AgentChatEndResponseInfo.Type, new AgentChatEndResponseInfo { Content = "结束" });
                if (!string.IsNullOrEmpty(session.GetSession().ParentAgentId))
                {
                    OnActivateParentAgent(session);
                }
                else
                {
                    Response(session, AgentChatEndAllResponseInfo.Type, new AgentChatEndAllResponseInfo { Content = "全部结束" });
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
            if (response.Success)
            {
                session.AddAssistantMessage(response.Think, response.Answer, response.ToolCalls);
                session.GetSession().Message.Type = -1;
                Request(session);
            }
            else
            {
                Response(session, AgentChatErrorResponseInfo.Type, new AgentChatErrorResponseInfo { Message = response.Message });
            }
        }
        // 执行工具
        private void ExecuteTools(AgentChatSession session)
        {
            var context = session.GetSession().Request.Context;
            if (context.Count == 0) return;
            int index = context.Count - 1;
            foreach (var call in context[index].ToolCalls)
            {
                if (call.Response == null)
                {
                    var tools = session.GetSession().Config.Tools.Where(n => n.ToolName == call.Name).ToList();
                    // 没有这个工具,不允许执行
                    if (tools.Count <= 0)
                    {
                        Response(session, AgentChatToolCallResponseInfo.Type, new AgentChatToolCallResponseInfo { Confirmation = false, ToolName = call.Name, ToolParam = call.Arguments });
                        call.Response = "本次用户不允许调用这个工具";
                        Response(session, AgentChatToolResponseInfo.Type, new AgentChatToolResponseInfo { ToolName = call.Name, Response = "执行失败没有找到这个工具!" });
                    }
                    // 这个工具需要授权
                    else if (tools[0].Confirm)
                    {
                        if (session.GetSession().Message.Type == AgentChatToolConfirmationRequestInfo.Type)
                        {
                            var value = JsonConvert.DeserializeObject<AgentChatToolConfirmationRequestInfo>(session.GetSession().Message.Data) ?? new();
                            call.Response = value.Allow ? toolsManager_.Invoke(call) : "本次用户不允许调用这个工具";
                            Response(session, AgentChatToolResponseInfo.Type, new AgentChatToolResponseInfo { ToolName = call.Name, Response = string.IsNullOrWhiteSpace(call.Response) ? "没有内容." : call.Response });
                        }
                        else
                        {
                            Response(session, AgentChatToolCallResponseInfo.Type, new AgentChatToolCallResponseInfo { Confirmation = true, ToolName = call.Name, ToolParam = call.Arguments });
                            return;
                        }
                    }
                    // 这个工具不需要授权直接调用
                    else
                    {
                        Response(session, AgentChatToolCallResponseInfo.Type, new AgentChatToolCallResponseInfo { Confirmation = false, ToolName = call.Name, ToolParam = call.Arguments });
                        call.Response = toolsManager_.Invoke(call);
                        Response(session, AgentChatToolResponseInfo.Type, new AgentChatToolResponseInfo { ToolName = call.Name, Response = string.IsNullOrWhiteSpace(call.Response) ? "没有内容." : call.Response });
                    }
                    // 执行完成丢给规划方法
                    session.GetSession().Message.Type = -1;
                    Request(session);
                    return;
                }
            }
        }
        // 执行技能
        private void ExecuteSkills(AgentChatSession session)
        {
            var context = session.GetSession().Request.Context;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            foreach (var call in context[index].ToolCalls)
            {
                if (call.Response == null)
                {
                    var tools = session.GetSession().Config.Tools.Where(n => n.ToolName == call.Name).ToList();
                    // 没有这个技能,不允许执行
                    if (tools.Count <= 0)
                    {
                        Response(session, AgentChatToolCallResponseInfo.Type, new AgentChatToolCallResponseInfo { Confirmation = false, ToolName = call.Name, ToolParam = call.Arguments });
                        call.Response = "本次用户不允许调用这个技能";
                        Response(session, AgentChatToolResponseInfo.Type, new AgentChatToolResponseInfo { ToolName = call.Name, Response = "执行失败没有找到这个技能!" });
                    }
                    else
                    {
                        try
                        {
                            var json = JObject.Parse(call.Arguments);
                            if (!json.ContainsKey("skill")) throw new Exception();
                            string? skillName = json["skill"]?.ToObject<string>();
                            if(skillName == null) throw new Exception();
                            var skills = session.GetSession().Config.Skills.Where(n => n.SkillName == skillName).ToList();


                            // 没有这个技能,不允许执行
                            if (skills.Count <= 0)
                            {
                                Response(session, AgentChatSkillCallResponseInfo.Type, new AgentChatSkillCallResponseInfo { Confirmation = false, SkillName = call.Name, SkillParam = call.Arguments });
                                call.Response = "本次用户不允许调用这个技能";
                                Response(session, AgentChatSkillCallResponseInfo.Type, new AgentChatSkillResponseInfo { SkillName = call.Name, Response = "执行失败没有找到这个技能!" });
                            }
                            // 这个技能需要授权
                            else if (skills[0].Confirm)
                            {
                                if (session.GetSession().Message.Type == AgentChatSkillConfirmationRequestInfo.Type)
                                {
                                    var value = JsonConvert.DeserializeObject<AgentChatSkillConfirmationRequestInfo>(session.GetSession().Message.Data) ?? new();
                                    call.Response = value.Allow ? toolsManager_.Invoke(call) : "本次用户不允许调用这个技能";
                                    Response(session, AgentChatSkillResponseInfo.Type, new AgentChatSkillResponseInfo { SkillName = call.Name, Response = string.IsNullOrWhiteSpace(call.Response) ? "没有内容." : call.Response });
                                }
                                else
                                {
                                    Response(session, AgentChatSkillCallResponseInfo.Type, new AgentChatSkillCallResponseInfo { Confirmation = true, SkillName = call.Name, SkillParam = call.Arguments });
                                    return;
                                }
                            }
                            // 这个技能不需要授权直接调用
                            else
                            {
                                Response(session, AgentChatSkillCallResponseInfo.Type, new AgentChatSkillCallResponseInfo { Confirmation = false, SkillName = call.Name, SkillParam = call.Arguments });
                                call.Response = toolsManager_.Invoke(call);
                                Response(session, AgentChatSkillResponseInfo.Type, new AgentChatSkillResponseInfo { SkillName = call.Name, Response = string.IsNullOrWhiteSpace(call.Response) ? "没有内容." : call.Response });
                            }
                        }
                        catch
                        {

                        }
                    }
                    // 执行完成丢给规划方法
                    session.GetSession().Message.Type = -1;
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
                if (item.Response == null)
                {
                    return item.Name == "add_download_file";
                }
            }
            return false;
        }
        // 选择
        private bool IsAskUser(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "ask_user";
                }
            }
            return false;
        }
        // 创建智能体
        private bool IsCreateSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "create_sub_agent";
                }
            }
            return false;
        }
        // 加载智能体
        private bool IsLoadSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "load_sub_agent";
                }
            }
            return false;
        }
        // 复用智能体
        private bool IsReuseSubAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "reuse_sub_agent";
                }
            }
            return false;
        }
        // 等待子智能体
        private bool IsWaitForAgent(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "wait_for_agents";
                }
            }
            return false;
        }
        // 判断是不是调用工具
        private bool IsTools(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name != "load_skill" &&
                        item.Name != "execute_skill_script" &&
                        item.Name != "wait_for_agents" &&
                        item.Name != "ask_user" &&
                        item.Name != "create_sub_agent" &&
                        item.Name != "load_sub_agent" &&
                        item.Name != "reuse_sub_agent" &&
                        item.Name != "task_planning" &&
                        item.Name != "task_update" &&
                        item.Name != "add_download_file";
                }
            }
            return false;
        }
        // 判断是不是调用技能
        private bool IsSkills(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "load_skill" || item.Name == "execute_skill_script";
                }
            }
            return false;
        }
        // 判断是不是规划
        private bool IsTaskPlanning(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "task_planning";
                }
            }
            return false;
        }
        // 判断是不是更新任务
        private bool IsTaskUpdate(List<AgentLLMItemFuncRequestInfo> toolCalls)
        {
            foreach (var item in toolCalls)
            {
                if (item.Response == null)
                {
                    return item.Name == "task_update";
                }
            }
            return false;
        }
        // 激活父智能体
        private void OnActivateParentAgent(AgentChatSession session)
        {
            var sessionParent = session.Complete();
            if (sessionParent == null) return;

            var context = sessionParent.GetSession().Request.Context;
            var type = sessionParent.GetSession().Message.Type;
            if (context.Count == 0) return;
            int index = context.Count - 1;

            if (!IsWaitForAgent(context[index].ToolCalls)) return;

            try
            {
                OnWaitForAgent(sessionParent, context[index].ToolCalls.Where(n => n.Name == "wait_for_agents" && n.Response == null).ToList()[0]);
            }
            catch { }
        }
        // 添加到下载列表
        private void OnDownloadFile(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("path")) throw new Exception();
                string? path = json["path"]?.ToObject<string>();
                if (path == null) throw new Exception();

                if (File.Exists(path))
                {
                    byte[] fileBytes = File.ReadAllBytes(path);
                    string base64String = Convert.ToBase64String(fileBytes);
                    Response(session, AgentChatUploadResponseInfo.Type, new AgentChatUploadResponseInfo { Name = (new FileInfo(path)).Name, Data = base64String });

                    toolCall.Response = "添加成功!";
                    session.GetSession().Message.Type = -1;
                    Request(session);
                }
                else
                {
                    toolCall.Response = "添加失败,文件不存在!";
                    session.GetSession().Message.Type = -1;
                    Request(session);
                }
            }
            catch
            {
                toolCall.Response = "调用失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 等待子智能体
        private void OnWaitForAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            if (!session.LoadSubAgentAnswers()) return;

            session.GetSession().Message.Type = -1;
            Request(session);
        }
        // AI疑问,列出选项供用户解答疑问
        private void OnAskUser(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                if (session.GetSession().Message.Type != AgentChatSelectRequestInfo.Type)
                {
                    var json = JObject.Parse(toolCall.Arguments);
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
                        addItem.Type = key.ToUpper() == "SELECT" ? true : false;
                        addItem.Content = value;
                        param.Options.Add(addItem);
                    }
                    param.Single = (bool)single;
                    param.Title = title;

                    Response(session, AgentChatSelectResponseInfo.Type, param);
                }
                else
                {
                    var value = JsonConvert.DeserializeObject<AgentChatSelectRequestInfo>(session.GetSession().Message.Data);
                    if (value == null) throw new Exception();
                    toolCall.Response = string.Join('\n', value.Options);
                    if (value.Options.Count == 0) toolCall.Response = "用户没有提供选择!";
                    session.GetSession().Message.Type = -1;
                    Request(session);
                }
            }
            catch
            {
                toolCall.Response = "调用失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 智能体规划任务
        private void OnTaskPlanning(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
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

                    addItem.Id = param.Plannings.Count + 1;
                    addItem.Type = "PENDING";
                    addItem.Content = content;
                    param.Plannings.Add(addItem);
                }

                // 保存
                if(!session.SavePlanning(param)) throw new Exception();

                Response(session, AgentChatPlanningResponseInfo.Type, param);

                toolCall.Response = "执行成功";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
            catch
            {
                toolCall.Response = "调用失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 智能体更新任务
        private void OnTaskUpdate(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
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

                Response(session, AgentChatPlanningResponseInfo.Type, planning);

                toolCall.Response = "执行成功";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
            catch
            {
                toolCall.Response = "调用失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 创建子智能体
        private void OnCreateSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
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
                    UserId = "",
                    SessionId = session.GetSession().Message.SessionId,
                    AgentId = agentId,
                    AgentName = agentName,
                    MsgId = "",
                    Type = AgentChatQuestionRequestInfo.Type,
                    Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = content })
                };
                // 配置
                List<AgentChatSkillInfo> skillsValue = new List<AgentChatSkillInfo>();
                foreach (var item1 in skills)
                {
                    foreach (var item2 in session.GetSession().Config.Skills)
                    {
                        if (item1 == item2.SkillName)
                        {
                            skillsValue.Add(item2);
                        }
                    }
                }
                List<AgentChatToolInfo> toolsValue = new List<AgentChatToolInfo>();
                foreach (var item1 in tools)
                {
                    foreach (var item2 in session.GetSession().Config.Tools)
                    {
                        if (item1 == item2.ToolName)
                        {
                            toolsValue.Add(item2);
                        }
                    }
                }
                AgentChatConfigInfo config = new AgentChatConfigInfo
                {
                    Users = new List<AgentChatUserInfo> { },
                    Roles = new List<AgentChatRoleInfo> { { new AgentChatRoleInfo { Name = agentName, LlmAddress = session.GetSession().Config.Roles[0].LlmAddress, Temperature = (double)temperature, MaxTokens = (int)maxTokens } } },
                    Models = new List<AgentChatModelInfo> { { new AgentChatModelInfo { ModelName = model } } },
                    Prompts = new List<AgentChatPromptInfo> { { new AgentChatPromptInfo { PromptValue = prompt } } },
                    Skills = skillsValue,
                    Tools = toolsValue
                };
                // 上下文
                AgentLLMRequestInfo request = new AgentLLMRequestInfo();
                // LLM地址
                request.Address = session.GetSession().Config.Roles[0].LlmAddress;
                // 模型名称
                request.Model = session.GetSession().Config.Models[0].ModelName;
                // 最大token数量
                request.MaxTokens = session.GetSession().Config.Roles[0].MaxTokens;
                // 温度
                request.Temperature = session.GetSession().Config.Roles[0].Temperature;
                // 开启思考
                request.Thinking = true;
                // 添加提示词
                if (!string.IsNullOrEmpty(prompt))
                {
                    request.Context.Add(new AgentLLMItemRequestInfo
                    {
                        Content = prompt,
                        Role = "system"
                    });
                }
                // 添加问题
                request.Context.Add(new AgentLLMItemRequestInfo
                {
                    Content = content,
                    Role = "user"
                });
                // 参数
                AgentChatSessionInfo param = new AgentChatSessionInfo
                {
                    AgentId = agentId,
                    AgentName = agentName,
                    ParentAgentId = string.IsNullOrEmpty(session.GetSession().ParentAgentId) ? session.GetSession().AgentId : session.GetSession().ParentAgentId + "/" + session.GetSession().AgentId,
                    Config = config,
                    Message = message,
                    Request = request
                };

                AgentChatSession newSession = new(param);
                newSession.GetSession().MsgId = Guid.NewGuid().ToString();
                Request(newSession);

                toolCall.Response = $"创建成功,智能体ID: {agentId}";

                Console.WriteLine("====================================================================");

                session.GetSession().Message.Type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch
            {
                toolCall.Response = "创建失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 加载子智能体
        private void OnLoadSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("id") || !json.ContainsKey("content")) throw new Exception();
                int? id = json["id"]?.ToObject<int>();
                string? content = json["content"]?.ToObject<string>();
                if (id == null || content == null) throw new Exception();

                string agentId = Guid.NewGuid().ToString();

                AgentChatSession newSession = new(new AgentChatMessageInfo
                {
                    AgentId = agentId,
                    ParentAgentId = session.GetSession().AgentId,
                    SessionId = session.GetSession().Message.SessionId,
                    Type = AgentChatQuestionRequestInfo.Type,
                    Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = content })
                });

                if (newSession.CreateSession((int)id, agentId))
                {
                    newSession.GetSession().MsgId = Guid.NewGuid().ToString();
                    Request(newSession);
                    toolCall.Response = $"加载成功,智能体ID: {agentId}";
                }
                else
                {
                    toolCall.Response = "加载失败,角色不存在!";
                }

                session.GetSession().Message.Type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch
            {
                toolCall.Response = "加载失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 复用子智能体
        private void OnReuseSubAgent(AgentChatSession session, AgentLLMItemFuncRequestInfo toolCall)
        {
            try
            {
                var json = JObject.Parse(toolCall.Arguments);
                if (json == null) throw new Exception();
                if (!json.ContainsKey("agentId") || !json.ContainsKey("content")) throw new Exception();
                string? agentId = json["agentId"]?.ToObject<string>();
                string? content = json["content"]?.ToObject<string>();
                if (agentId == null || content == null) throw new Exception();

                AgentChatSession newSession = new(new AgentChatMessageInfo
                {
                    AgentId = agentId,
                    ParentAgentId = session.GetSession().AgentId,
                    SessionId = session.GetSession().Message.SessionId,
                    Type = AgentChatQuestionRequestInfo.Type,
                    Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = content })
                });

                if (newSession.LoadSession())
                {
                    newSession.GetSession().MsgId = Guid.NewGuid().ToString();
                    Request(newSession);
                    toolCall.Response = "复用成功!";
                }
                else
                {
                    toolCall.Response = "复用失败!";
                }

                session.GetSession().Message.Type = -1;
                session.AddSubAgent(agentId);
                Request(session);
            }
            catch
            {
                toolCall.Response = "加载失败,参数错误!";
                session.GetSession().Message.Type = -1;
                Request(session);
            }
        }
        // 响应
        private bool OnResponse(AgentLLMResponseInfo response, AgentChatSession session)
        {
            if (!string.IsNullOrEmpty(response.Think))
            {
                return Response(session, AgentChatThinkResponseInfo.Type, new AgentChatThinkResponseInfo { Content = response.Think });
            }
            else
            {
                return Response(session, AgentChatAnswerResponseInfo.Type, new AgentChatAnswerResponseInfo { Content = response.Answer });
            }
        }
        // 响应
        private bool Response(AgentChatMessageInfo request, int type, object t)
        {
            AgentChatMessageInfo response = new AgentChatMessageInfo();
            response.SessionId = request.SessionId;
            response.AgentId = request.AgentId;
            response.AgentName = request.AgentName;
            response.MsgId = request.MsgId;
            response.ParentAgentId = request.ParentAgentId;
            response.Type = type;
            response.Data = JsonConvert.SerializeObject(t);
            return onResponse.Invoke(response);
        }
        // 响应
        private bool Response(AgentChatSession session,int type, object t)
        {
            AgentChatMessageInfo response = new AgentChatMessageInfo();
            response.SessionId = session.GetSession().Message.SessionId;
            response.AgentId = session.GetSession().AgentId;
            response.AgentName = session.GetSession().AgentName;
            response.MsgId = session.GetSession().MsgId;
            response.ParentAgentId = session.GetSession().ParentAgentId;
            response.Type = type;
            // 回复收到消息
            if (response.Type == AgentChatTaskResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 思考
            else if (response.Type == AgentChatThinkResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 结论
            else if (response.Type == AgentChatAnswerResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 工具调用
            else if (response.Type == AgentChatToolCallResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 工具响应
            else if (response.Type == AgentChatToolResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 技能调用
            else if (response.Type == AgentChatSkillCallResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 技能响应
            else if (response.Type == AgentChatSkillResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 选择
            else if (response.Type == AgentChatSelectResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 上传文件
            else if (response.Type == AgentChatUploadResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 规划响应
            else if (response.Type == AgentChatPlanningResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 错误
            else if (response.Type == AgentChatErrorResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 智能体结束
            else if (response.Type == AgentChatEndResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            // 全部结束
            else if (response.Type == AgentChatEndAllResponseInfo.Type)
            {
                response.Data = JsonConvert.SerializeObject(t);
            }
            return onResponse.Invoke(response);
        }
    }
}
