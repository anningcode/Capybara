using Capybara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class DownloadPlugin : IToolPlugin
    {
        [AgentFunction("add_download_file")]
        [Description("添加到下载文件列表")]
        public string AddDownloadFile([Description("文件相对路径")] string path)
        {
            return string.Empty;
        }
    }
}
