using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class PlanningPlugin : IToolPlugin
    {
        [AgentFunction("planning")]
        [Description("拆解规划执行进度,列出任务进度(已完成,执行中,待执行)三个状态,分别由(Completed:代表已完成,InProgress:代表执行中,Pending:代表待执行)")]
        public string Planning([Description("规划列表,例子['Completed: xxx完成的任务'],['InProgress: xxx执行中的任务'],['Pending: xxx待执行的任务']")] List<string> list)
        {
            return string.Empty;
        }
    }
}
