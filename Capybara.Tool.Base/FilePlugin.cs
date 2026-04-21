using System.Text;

namespace Capybara.Tool.Base
{
    public class FilePlugin : IToolPlugin
    {
        // 写文件内容
        [AgentFunction("write_file")]
        [Description("写文件内容")]
        public string WriteFile([Description("文件完整路径")] string path, [Description("文件内容")] string content)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件路径不能为空或空白";
            content ??= string.Empty;
            try
            {
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
            return "写入完成";
        }
        // 追加文件内容
        [AgentFunction("append_write_file")]
        [Description("追加写入文件")]
        public string AppendWriteFile([Description("文件完整路径")] string path, [Description("要追加的文件内容")] string content)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件路径不能为空或空白";
            content ??= string.Empty;
            try
            {
                string? directoryPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                System.IO.File.AppendAllText(path, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return $"追加写入文件失败（路径：{path}）：{ex.Message}";
            }
            return "写入完成";
        }
        // 读取文件内容
        [AgentFunction("read_file")]
        [Description("读取文件")]
        public string ReadFile([Description("文件完整路径")] string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "文件路径不能为空或空白";
            try
            {
                if (!System.IO.File.Exists(path))
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
