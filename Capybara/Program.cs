
using Capybara;
using Capybara.Agent;
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
    const int IDYES = 6;      // 用户点击“是”的返回值
    const int IDNO = 7;       // 用户点击“否”的返回值



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

        Console.WriteLine(JsonConvert.SerializeObject(response));

        if (response.type == AgentChatToolCallResponseInfo.type)
        {
            AgentChatToolCallResponseInfo? value = JsonConvert.DeserializeObject<AgentChatToolCallResponseInfo>(response.data) ?? new();
            if (value.confirmation)
            {
                AgentChatToolConfirmationRequestInfo param = new AgentChatToolConfirmationRequestInfo();
                param.toolName = value.toolName;
                int result = MessageBox(IntPtr.Zero,
                                $"工具确认:{param.toolName}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);
                param.allow = result == IDYES;

                AgentChatMessageInfo request = new AgentChatMessageInfo();

                request.type = AgentChatToolConfirmationRequestInfo.type;
                request.sessionId = response.sessionId;
                request.userId = response.userId;
                request.agentId = response.agentId;
                request.parentAgentId = response.parentAgentId;
                request.data = JsonConvert.SerializeObject(param);

                agentRuntime_?.Request(request);
            }
        }
        else if (response.type == AgentChatSkillCallResponseInfo.type)
        {
            AgentChatSkillCallResponseInfo? value = JsonConvert.DeserializeObject<AgentChatSkillCallResponseInfo>(response.data) ?? new();
            if (value.confirmation)
            {
                AgentChatSkillConfirmationRequestInfo param = new AgentChatSkillConfirmationRequestInfo();
                param.skillName = value.skillName;
                int result = MessageBox(IntPtr.Zero,
                                $"技能确认:{param.skillName}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);
                param.allow = result == IDYES;

                AgentChatMessageInfo request = new AgentChatMessageInfo();

                request.type = AgentChatSkillConfirmationRequestInfo.type;
                request.sessionId = response.sessionId;
                request.userId = response.userId;
                request.agentId = response.agentId;
                request.parentAgentId = response.parentAgentId;
                request.data = JsonConvert.SerializeObject(param);

                agentRuntime_?.Request(request);
            }
        }
        else if (response.type == AgentChatSelectResponseInfo.type)
        {
            AgentChatSelectResponseInfo? value = JsonConvert.DeserializeObject<AgentChatSelectResponseInfo>(response.data) ?? new();
            AgentChatSelectRequestInfo param = new AgentChatSelectRequestInfo();
            param.selects = value.selects;
            int result = MessageBox(IntPtr.Zero,
                                $"标题:{value.title}\n单选:{value.single}\n选择:{string.Join(',', value.selects)}",
                                "询问",
                                MB_YESNO | MB_ICONQUESTION);
            AgentChatMessageInfo request = new AgentChatMessageInfo();

            request.type = AgentChatSelectRequestInfo.type;
            request.sessionId = response.sessionId;
            request.userId = response.userId;
            request.agentId = response.agentId;
            request.parentAgentId = response.parentAgentId;
            request.data = JsonConvert.SerializeObject(param);

            agentRuntime_?.Request(request);
        }
        return true;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Core core = new Core();
        string sessionId = "ff102885-4439-4046-850c-09d3dab4d1ed";
        string userId = "c34c9eaf-e9f8-4da7-895e-179f09918391";

        AgentChatMessageInfo request = new AgentChatMessageInfo();

        request.type = AgentChatQuestionRequestInfo.type;
        request.sessionId = sessionId;
        request.userId = userId;
        request.data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = "帮我创建几个子智能体分别查看几个硬盘都有哪些文件夹,查询那个几个盘请询问用户?" });
        // request.data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = "帮我看一下几点了?" });
        // request.data = JsonConvert.SerializeObject(new AgentChatQuestionRequestInfo { content = "帮我创建1个子智能体查看D盘都有哪些文件夹?" });
        core.Request(request);

        Console.ReadKey();
    }
}