using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class SkillsPlugin : IToolPlugin
    {
        // 加载技能
        [AgentFunction("load_skill")]
        [Description(@"技能加载,在技能列表找到对应的技能加载,例子:LoadSkill(""xlsxstyle"","""")加载技能描述,描述里边提到了`references/人员.md`LoadSkill(""excel"",""references/人员.md"")加载详情描述,不能用于非技能列表的技能.")]
        public string LoadSkill([Description("技能名称")] string skill, [Description("技能组件,默认空.")] string component)
        {
            string path = "";
            if (string.IsNullOrEmpty(component))
            {
                path = $"./capybara/skills/{skill}/SKILL.md";
            }
            else
            {
                path = $"./capybara/skills/{skill}/{component}";
            }
            if (!System.IO.File.Exists(path))
                return "技能不存在.";
            try
            {
                return System.IO.File.ReadAllText(path);
            }
            catch
            {
                return "加载技能错误!";
            }
        }
        // 执行技能脚本
        [AgentFunction("execute_skill_script")]
        [Description(@"执行技能脚本,只能用于执行技能相关的命令,execute_skill_script不是Bash工具不能用于执行命令行,用法:首先通过load_skill(""技能名称"","""")加载技能详情,在通过execute_skill_script(""技能名称"",""执行程序(python, node) 脚本路径 参数1 参数2...""),不同的脚本对应不同的执行命令.")]
        public string ExecuteSkillScript([Description("技能名称")] string skill, [Description("命令行")] string command)
        {
            string cmd_line = $"cd /d {Environment.CurrentDirectory}\\capybara\\skills\\{skill} && {command.Replace("/", "\\")}";
            CommandPlugin cmd = new CommandPlugin();
            return cmd.Bash(cmd_line);
        }
    }
}
