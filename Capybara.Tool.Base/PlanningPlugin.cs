using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class PlanningPlugin : IToolPlugin
    {
        [AgentFunction("task_planning")]
        [Description("规划任务,初始所有任务是待执行状态.")]
        public string TaskPlanning([Description("规划列表,必须添加任务编号且必须从1开始 1-N,例子['1.xxx任务','2.xxx任务','3.xxx任务']")] List<string> tasks)
        {
            return string.Empty;
        }
        [AgentFunction("task_update")]
        [Description("更新任务状态,任务进度(已完成,执行中,待执行,执行失败)三个状态,分别由(Completed:代表已完成,InProgress:代表执行中,Pending:代表待执行,Failure:代表失败)")]
        public string TaskUpdate([Description("规划列表,必须添加任务编号['任务编号:状态',...],例子['2:Completed','3:InProgress']")] List<string> tasks)
        {
            return string.Empty;
        }
    }
}
