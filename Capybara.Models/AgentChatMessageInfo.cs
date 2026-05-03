using Newtonsoft.Json;
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
        [JsonProperty("type")]
        public const int Type = 0;
        // 问题内容
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    // 请求,用户工具确认或取消
    public class AgentChatToolConfirmationRequestInfo
    {
        [JsonProperty("type")]
        public const int Type = 1;
        [JsonProperty("allow")]
        public bool Allow { get; set; }
        [JsonProperty("toolName")]
        public string ToolName { get; set; } = string.Empty;
    }
    // 请求,用户技能确认或取消
    public class AgentChatSkillConfirmationRequestInfo
    {
        [JsonProperty("type")]
        public const int Type = 2;
        [JsonProperty("allow")]
        public bool Allow { get; set; }
        [JsonProperty("skillName")]
        public string SkillName { get; set; } = string.Empty;
    }
    // 请求,疑问
    public class AgentChatSelectRequestInfo
    {
        [JsonProperty("type")]
        public const int Type = 3;
        // 选择列表
        [JsonProperty("options")]
        public List<string> Options { get; set; } = new List<string>();
    }
    // 响应,收到任务
    public class AgentChatTaskResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 0;
        // 问题内容
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    // 响应,思考
    public class AgentChatThinkResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 1;
        // 思考
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    // 响应,结论
    public class AgentChatAnswerResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 2;
        // 结论
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    // 响应,工具
    public class AgentChatToolCallResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 3;
        private string toolName_ = string.Empty;
        private string toolParam_ = string.Empty;
        // 工具名称
        [JsonProperty("toolName")]
        public string ToolName
        {
            get
            {
                return toolName_;
            }
            set
            {
                if (value.Length > 100)
                {
                    toolName_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    toolName_ = value;
                }
            }
        }
        // 参数
        [JsonProperty("toolParam")]
        public string ToolParam
        {
            get
            {
                return toolParam_;
            }
            set
            {
                if (value.Length > 100)
                {
                    toolParam_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    toolParam_ = value;
                }
            }
        }
        // 要求用户确认
        [JsonProperty("confirmation")]
        public bool Confirmation { get; set; } = false;
    }
    // 响应,工具响应
    public class AgentChatToolResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 4;
        private string response_ = string.Empty;
        // 工具名称
        [JsonProperty("toolName")]
        public string ToolName { get; set; } = string.Empty;
        // 工具响应
        [JsonProperty("response")]
        public string Response
        {
            get
            {
                return response_;
            }
            set
            {
                if (value.Length > 100)
                {
                    response_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    response_ = value;
                }
            }
        }
    }
    // 响应,技能
    public class AgentChatSkillCallResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 5;
        private string skillName_ = string.Empty;
        private string skillParam_ = string.Empty;
        // 技能名称
        [JsonProperty("skillName")]
        public string SkillName
        {
            get
            {
                return skillName_;
            }
            set
            {
                if (value.Length > 100)
                {
                    skillName_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    skillName_ = value;
                }
            }
        }
        // 参数
        [JsonProperty("skillParam")]
        public string SkillParam
        {
            get
            {
                return skillParam_;
            }
            set
            {
                if (value.Length > 100)
                {
                    skillParam_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    skillParam_ = value;
                }
            }
        }
        // 要求用户确认
        [JsonProperty("confirmation")]
        public bool Confirmation { get; set; } = false;
    }
    // 响应,技能响应
    public class AgentChatSkillResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 6;
        private string response_ = string.Empty;
        // 技能名称
        [JsonProperty("skillName")]
        public string SkillName { get; set; } = string.Empty;
        // 技能响应
        [JsonProperty("response")]
        public string Response
        {
            get
            {
                return response_;
            }
            set
            {
                if (value.Length > 100)
                {
                    response_ = value.Substring(0, 100) + " ......";
                }
                else
                {
                    response_ = value;
                }
            }
        }
    }
    // 响应,疑问
    public class AgentChatSelectItemInfo
    {
        [JsonProperty("type")]
        public bool Type { get; set; }
        // 选项内容
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    public class AgentChatSelectResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 7;
        // 标题
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        // 选项
        [JsonProperty("options")]
        public List<AgentChatSelectItemInfo> Options { get; set; } = new List<AgentChatSelectItemInfo>();
        // 选择类型:true:单选, false:多选
        [JsonProperty("single")]
        public bool Single { get; set; } = true;
    }
    // 响应,上传文件
    public class AgentChatUploadResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 8;
        // 文件名称
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        // 数据base64
        [JsonProperty("data")]
        public string Data { get; set; } = string.Empty;
    }
    // 规划内容
    public class AgentChatPlanningItemInfo
    {
        // 任务编号
        [JsonProperty("id")]
        public int Id { get; set; } = 0;
        // 类型,COMPLETED:已完成, INPROGRESS:执行中, PENDING:待执行
        [JsonProperty("type")]
        public string Type { get; set; } = "PENDING";
        // 规划正文
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
    // 响应,规划
    public class AgentChatPlanningResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 9;
        // 规划列表
        [JsonProperty("plannings")]
        public List<AgentChatPlanningItemInfo> Plannings { get; set; } = new List<AgentChatPlanningItemInfo>();
    }
    // 响应,错误
    public class AgentChatErrorResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 10;
        // 错误消息
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
    // 响应,结束
    public class AgentChatEndResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 11;
        // 结束内容
        [JsonProperty("content")]
        public string Content { get; set; } = "结束";
    }
    // 响应,全部结束
    public class AgentChatEndAllResponseInfo
    {
        [JsonProperty("type")]
        public const int Type = 12;
        // 全部结束内容
        [JsonProperty("content")]
        public string Content { get; set; } = "全部结束";
    }
    // 交互消息
    public class AgentChatMessageInfo
    {
        // 响应地址
        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;
        // 用户id
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;
        // 会话ID,区分客户端
        [JsonProperty("sessionId")]
        public string SessionId { get; set; } = string.Empty;
        // 智能体ID
        [JsonProperty("agentId")]
        public string AgentId { get; set; } = string.Empty;
        // 父智能体ID
        [JsonProperty("parentAgentId")]
        public string ParentAgentId { get; set; } = string.Empty;
        // 智能体名称
        [JsonProperty("agentName")]
        public string AgentName { get; set; } = string.Empty;
        // 消息ID
        [JsonProperty("msgId")]
        public string MsgId { get; set; } = string.Empty;
        // 消息类型
        [JsonProperty("type")]
        public int Type { get; set; }
        // 数据
        [JsonProperty("data")]
        public string Data { get; set; } = string.Empty;
    }
}
