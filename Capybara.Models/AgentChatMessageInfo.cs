using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Models
{
    // 请求,用户问题
    public class AgentChatQuestionRequestInfo
    {
        public const int type = 0;
        // 问题内容
        public string content { get; set; } = string.Empty;
    }
    // 请求,用户工具确认或取消
    public class AgentChatToolConfirmationRequestInfo
    {
        public const int type = 1;
        public bool allow { get; set; }
        public string toolName { get; set; } = string.Empty;
    }
    // 请求,用户技能确认或取消
    public class AgentChatSkillConfirmationRequestInfo
    {
        public const int type = 2;
        public bool allow { get; set; }
        public string skillName { get; set; } = string.Empty;
    }
    // 请求,疑问
    public class AgentChatSelectRequestInfo
    {
        public const int type = 3;
        // 选择列表
        public List<string> selects { get; set; } = new List<string>();
    }
    // 响应,收到任务
    public class AgentChatTaskResponseInfo
    {
        public const int type = 0;
        // 问题内容
        public string content { get; set; } = string.Empty;
    }
    // 响应,思考
    public class AgentChatThinkResponseInfo
    {
        public const int type = 1;
        // 思考
        public string content { get; set; } = string.Empty;
    }
    // 响应,结论
    public class AgentChatAnswerResponseInfo
    {
        public const int type = 2;
        // 结论
        public string content { get; set; } = string.Empty;
    }
    // 响应,工具
    public class AgentChatToolCallResponseInfo
    {
        public const int type = 3;
        // 工具名称
        public string toolName { get; set; } = string.Empty;
        // 参数
        public string toolParam { get; set; } = string.Empty;
        // 要求用户确认
        public bool confirmation { get; set; } = false;
    }
    // 响应,工具响应
    public class AgentChatToolResponseInfo
    {
        public const int type = 4;
        // 工具名称
        public string toolName { get; set; } = string.Empty;
        // 工具响应
        public string response { get; set; } = string.Empty;
    }
    // 响应,技能
    public class AgentChatSkillCallResponseInfo
    {
        public const int type = 5;
        // 技能名称
        public string skillName { get; set; } = string.Empty;
        // 参数
        public string skillParam { get; set; } = string.Empty;
        // 要求用户确认
        public bool confirmation { get; set; } = false;
    }
    // 响应,技能响应
    public class AgentChatSkillResponseInfo
    {
        public const int type = 6;
        // 技能名称
        public string skillName { get; set; } = string.Empty;
        // 技能响应
        public string response { get; set; } = string.Empty;
    }
    // 响应,疑问
    public class AgentChatSelectResponseInfo
    {
        public const int type = 7;
        // 标题
        public string title { get; set; } = string.Empty;
        // 选项
        public List<string> selects { get; set; } = new List<string>();
        // 选择类型:true:单选, false:多选
        public bool single { get; set; } = true;
    }
    // 响应,上传文件
    public class AgentChatUploadResponseInfo
    {
        public const int type = 8;
        // 数据base64
        public string data { get; set; } = string.Empty;
    }
    // 规划内容
    public class AgentChatPlanningItemInfo 
    {
        // 类型,0:未处理, 1:处理中, 2:处理完成
        public int type { get; set; } = 0;
        // 规划正文
        public string content { get; set; } = string.Empty;
    }
    // 响应,规划
    public class AgentChatPlanningResponseInfo
    {
        public const int type = 9;
        // 规划列表
        public List<AgentChatPlanningItemInfo> plannings { get; set; } = new List<AgentChatPlanningItemInfo>();
    }
    // 响应,错误
    public class AgentChatErrorRespResponseInfo
    {
        public const int type = 10;
        // 错误消息
        public string message { get; set; } = string.Empty;
    }
    // 响应,结束
    public class AgentChatEndResponseInfo
    {
        public const int type = 11;
        // 结束内容
        public string content { get; set; } = "结束";
    }
    // 响应,全部结束
    public class AgentChatEndAllResponseInfo
    {
        public const int type = 12;
        // 全部结束内容
        public string content { get; set; } = "全部结束";
    }
    // 交互消息
    public class AgentChatMessageInfo
    {
        // 用户id
        public string userId { get; set; } = string.Empty;
        // 会话ID,区分客户端
        public string sessionId { get; set; } = string.Empty;
        // 智能体ID
        public string agentId { get; set; } = string.Empty;
        // 父智能体ID
        public string parentAgentId { get; set; } = string.Empty;
        // 智能体名称
        public string agentName { get; set; } = string.Empty;
        // 消息ID
        public string msgId { get; set; } = string.Empty;
        // 消息类型
        public int type { get; set; }
        // 数据
        public string data { get; set; } = string.Empty;
    }
}
