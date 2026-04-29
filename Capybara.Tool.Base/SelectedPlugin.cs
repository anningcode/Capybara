using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class SelectedPlugin : IToolPlugin
    {
        [AgentFunction("ask_user")]
        [Description("当你的推理过程中发现缺少必要信息（例如：缺少用户具体参数、有多个歧义需用户选择、需要用户确认高风险操作、数据不足以决策），必须优先使用本工具向用户询问，而不是猜测或跳过。本工具每次只能询问一个明确的问题。")]
        public string Selected([Description("标题")] string title, [Description("选项列表，['SELECT:(AI输入内容,供用户选择.)','INPUT:(用户输入内容,需要用户输入提供给AI)'],例子:['SELECT:A方案','SELECT:B方案','SELECT:C方案','INPUT:自定义']")] List<string> options, [Description("单选true,多选false")] bool single)
        {
            return string.Empty;
        }
    }
}
