
using Capybara;
using Capybara.Agent;
using Capybara.entrance;
using Capybara.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;



public class Core
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    // 定义消息框类型常量
    const uint MB_YESNO = 0x00000004;
    const uint MB_ICONQUESTION = 0x00000020;
    const int IDYES = 6;      // 用户点击"是"的返回值
    const int IDNO = 7;       // 用户点击"否"的返回值



    private AgentRuntime? agentRuntime_ { get; set; }
    public Core()
    {
        agentRuntime_ = new AgentRuntime(OnResponse);
    }
    public void Request(AgentChatMessageInfo request)
    {
        agentRuntime_?.Request(request);
    }
    private bool OnResponse(AgentChatMessageInfo response)
    {
        if (response.Type == AgentChatToolCallResponseInfo.Type)
        {
            AgentChatToolCallResponseInfo? value = JsonConvert.DeserializeObject<AgentChatToolCallResponseInfo>(response.Data) ?? new();
            if (value.Confirmation)
            {
                AgentChatToolConfirmationRequestInfo param = new AgentChatToolConfirmationRequestInfo();
                param.ToolName = value.ToolName;
                int result = MessageBox(IntPtr.Zero,
                                $"工具确认:{param.ToolName}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);
                param.Allow = result == IDYES;

                AgentChatMessageInfo request = new AgentChatMessageInfo();

                request.Type = AgentChatToolConfirmationRequestInfo.Type;
                request.SessionId = response.SessionId;
                request.UserId = response.UserId;
                request.AgentId = response.AgentId;
                request.ParentAgentId = response.ParentAgentId;
                request.Data = JsonConvert.SerializeObject(param);

                agentRuntime_?.Request(request);
            }
        }
        else if (response.Type == AgentChatSkillCallResponseInfo.Type)
        {
            AgentChatSkillCallResponseInfo? value = JsonConvert.DeserializeObject<AgentChatSkillCallResponseInfo>(response.Data) ?? new();
            if (value.Confirmation)
            {
                AgentChatSkillConfirmationRequestInfo param = new AgentChatSkillConfirmationRequestInfo();
                param.SkillName = value.SkillName;
                int result = MessageBox(IntPtr.Zero,
                                $"技能确认:{param.SkillName}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);
                param.Allow = result == IDYES;

                AgentChatMessageInfo request = new AgentChatMessageInfo();

                request.Type = AgentChatSkillConfirmationRequestInfo.Type;
                request.SessionId = response.SessionId;
                request.UserId = response.UserId;
                request.AgentId = response.AgentId;
                request.ParentAgentId = response.ParentAgentId;
                request.Data = JsonConvert.SerializeObject(param);

                agentRuntime_?.Request(request);
            }
        }
        else if (response.Type == AgentChatSelectResponseInfo.Type)
        {
            AgentChatSelectResponseInfo? value = JsonConvert.DeserializeObject<AgentChatSelectResponseInfo>(response.Data) ?? new();
            AgentChatSelectRequestInfo param = new AgentChatSelectRequestInfo();
            if (value.Single)
            {
                param.Options = new List<string> { value.Options[0].Content };
            }
            else
            {
                param.Options = value.Options.Select(n => n.Content).ToList();
            }

            int result = MessageBox(IntPtr.Zero,
                                $"标题:{value.Title}\n单选:{value.Single}\n选择:{string.Join(',', param.Options)}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);

            AgentChatMessageInfo request = new AgentChatMessageInfo();

            request.Type = AgentChatSelectRequestInfo.Type;
            request.SessionId = response.SessionId;
            request.UserId = response.UserId;
            request.AgentId = response.AgentId;
            request.ParentAgentId = response.ParentAgentId;
            request.Data = JsonConvert.SerializeObject(param);

            agentRuntime_?.Request(request);
        }
        else if (response.Type == AgentChatPlanningResponseInfo.Type)
        {
            AgentChatPlanningResponseInfo? value = JsonConvert.DeserializeObject<AgentChatPlanningResponseInfo>(response.Data) ?? new();

            foreach (var item in value.Plannings)
            {
                if (item.Type == "COMPLETED")
                {
                    Console.WriteLine($"-------------------------已完成:{item.Content}---------------------");
                }
                else if (item.Type == "INPROGRESS")
                {
                    Console.WriteLine($"-------------------------执行中:{item.Content}---------------------");
                }
                else
                {
                    Console.WriteLine($"-------------------------待执行:{item.Content}---------------------");
                }
            }
        }
        return true;
    }
}
class Program
{
    static void Main(string[] args)
    {
        ChatEntrance chatEntrance = new ChatEntrance();
        chatEntrance.Init();
        Thread.Sleep(Timeout.Infinite);
        return;
        Core core = new Core();
        string sessionId = "ff102885-4439-4046-850c-09d3dab4d1ed";
        string userId = "c34c9eaf-e9f8-4da7-895e-179f09918391";
        AgentChatMessageInfo request = new AgentChatMessageInfo();
        request.Type = AgentChatQuestionRequestInfo.Type;
        request.SessionId = sessionId;
        request.UserId = userId;
        // request.Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = "帮我用html+css+js写一个坦克大战游戏,将所有文件添加到下载列表." });
        // request.Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = "帮我看一下几点了?" });
        request.Data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { Content = "帮我张啸武发一封邮件" });
        core.Request(request);
        Console.ReadKey();
    }
}