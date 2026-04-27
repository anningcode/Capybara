using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class PlanningPlugin : IToolPlugin
    {
        //[AgentFunction("task_planning")]
        //[Description("拆解规划执行进度,列出任务进度(已完成,执行中,待执行)三个状态,分别由(Completed:代表已完成,InProgress:代表执行中,Pending:代表待执行),任务必须列出编号.")]
        //public string Planning([Description("规划列表,例子['Completed: 1.xxx完成的任务'],['InProgress: 2.xxx执行中的任务'],['Pending: 3.xxx待执行的任务']")] List<string> list)
        //{
        //    return string.Empty;
        //}
        [AgentFunction("task_planning")]
        [Description("规划任务,初始所有任务是待执行状态.")]
        public string TaskPlanning([Description("规划列表,必须添加任务编号且必须从1开始 1-N,例子['1.xxx任务','2.xxx任务','3.xxx任务']")] List<string> tasks)
        {
            return string.Empty;
        }
        [AgentFunction("task_update")]
        [Description("更新任务状态,任务进度(已完成,执行中,待执行)三个状态,分别由(Completed:代表已完成,InProgress:代表执行中,Pending:代表待执行)")]
        public string TaskUpdate([Description("规划列表,必须添加任务编号['任务编号:状态',...],例子['2:Completed','3:InProgress']")] List<string> tasks)
        {
            return string.Empty;
        }
    }
}
