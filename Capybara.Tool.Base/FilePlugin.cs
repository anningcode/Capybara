using Capybara.Models;
using Capybara.Utils;
using System.Text;

namespace Capybara.Tool.Base
{
    public class FilePlugin : IToolPlugin
    {
        private string rootPath_ { get; set; } = "D:/file/";
        public FilePlugin()
        {
            var values = AppConfig.Get<List<WebGeneralConfigInfo>>("generals");
            if (values != null)
                rootPath_ = values.FirstOrDefault(n => n.Key == "rootpath" && n.Enable)?.Value ?? rootPath_;
        }
        // 写文件内容
        [AgentFunction("write_file")]
        [Description("写文件内容")]
        public string WriteFile([Description("文件相对路径")] string path, [Description("文件内容")] string content)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件相对路径不能为空或空白";
            content ??= string.Empty;
            try
            {
                path = Path.Join(rootPath_, path);
                string? directoryPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                System.IO.File.WriteAllText(path, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return ($"写入文件失败（路径：{path}）：{ex.Message}");
            }
            return $"写入完成,文件:{path}";
        }
        // 追加文件内容
        [AgentFunction("append_write_file")]
        [Description("追加写入文件")]
        public string AppendWriteFile([Description("文件相对路径")] string path, [Description("要追加的文件内容")] string content)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件相对路径不能为空或空白";
            content ??= string.Empty;
            try
            {
                path = Path.Join(rootPath_, path);
                string? directoryPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.AppendAllText(path, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return $"追加写入文件失败（路径：{path}）：{ex.Message}";
            }
            return $"写入完成,文件:{path}";
        }
        // 读取文件内容
        [AgentFunction("read_file")]
        [Description("读取文件")]
        public string ReadFile([Description("文件相对路径")] string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件相对路径不能为空或空白";
            try
            {
                path = Path.Join(rootPath_, path);
                if (!File.Exists(path))
                {
                    return "读取文件失败：文件不存在";
                }
                return System.IO.File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return ($"读取文件失败（路径：{path}）：{ex.Message}");
            }
        }
    }
}
