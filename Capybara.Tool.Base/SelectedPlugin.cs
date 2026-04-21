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
        [AgentFunction("selected")]
        [Description("AI疑问,列出选项供用户选择解答疑问")]
        public string Selected([Description("标题")] string title, [Description("选项列表")] List<string> list, [Description("单选true,多选false")] bool single)
        {
            return string.Empty;
        }
    }
}
