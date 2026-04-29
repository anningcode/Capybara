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
        public List<string> options { get; set; } = new List<string>();
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
        private string toolName_ = string.Empty;
        private string toolParam_ = string.Empty;
        // 工具名称
        public string toolName
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
        public string toolParam
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
        public bool confirmation { get; set; } = false;
    }
    // 响应,工具响应
    public class AgentChatToolResponseInfo
    {
        public const int type = 4;
        private string response_ = string.Empty;
        // 工具名称
        public string toolName { get; set; } = string.Empty;
        // 工具响应
        public string response
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
        public const int type = 5;
        private string skillName_ = string.Empty;
        private string skillParam_ = string.Empty;
        // 技能名称
        public string skillName
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
        public string skillParam
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
        public bool confirmation { get; set; } = false;
    }
    // 响应,技能响应
    public class AgentChatSkillResponseInfo
    {
        public const int type = 6;
        private string response_ = string.Empty;
        // 技能名称
        public string skillName { get; set; } = string.Empty;
        // 技能响应
        public string response
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
    public class AgentChatSelectItemInfo
    {
        // true:选择, false:输入
        public bool type { get; set; }
        // 选项内容
        public string content { get; set; } = string.Empty;
    }
    // 响应,疑问
    public class AgentChatSelectResponseInfo
    {
        public const int type = 7;
        // 标题
        public string title { get; set; } = string.Empty;
        // 选项
        public List<AgentChatSelectItemInfo> options { get; set; } = new List<AgentChatSelectItemInfo>();
        // 选择类型:true:单选, false:多选
        public bool single { get; set; } = true;
    }
    // 响应,上传文件
    public class AgentChatUploadResponseInfo
    {
        public const int type = 8;
        // 文件名称
        public string name { get; set; } = string.Empty;
        // 数据base64
        public string data { get; set; } = string.Empty;
    }
    // 规划内容
    public class AgentChatPlanningItemInfo 
    {
        // 任务编号
        public int id { get; set; } = 0;
        // 类型,COMPLETED:已完成, INPROGRESS:执行中, PENDING:待执行
        public string type { get; set; } = "PENDING";
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
    public class AgentChatErrorResponseInfo
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
        // 响应地址
        public string address { get; set; } = string.Empty;
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
