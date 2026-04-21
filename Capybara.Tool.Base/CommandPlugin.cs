using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class CommandPlugin : IToolPlugin
    {
        // 终端命令
        [AgentFunction("bash")]
        [Description("执行终端命令,", true)]
        public string Bash([Description("命令行")] string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return "命令行不能为空";
            // 在Windows上，使用cmd.exe来执行系统命令
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // 注意：/c 参数表示执行后关闭cmd
                return ExecuteCommand("cmd.exe", $"/c {command}", null);
            }
            else // 在Linux/macOS上，使用bash
            {
                // 注意：-c 参数表示从字符串中读取命令
                return ExecuteCommand("/bin/bash", $"-c \"{command}\"", null);
            }
        }
        // 终端命令
        private string ExecuteCommand(string command, string? arguments = null, string? workingDirectory = null)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
                };

                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    var outputBuilder = new StringBuilder();
                    var errorBuilder = new StringBuilder();

                    // 异步读取输出和错误流，避免死锁
                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                            outputBuilder.AppendLine(args.Data);
                    };
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                            errorBuilder.AppendLine(args.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // 如果错误输出不为空，将其附加到输出中（或者可以抛出异常，根据需求）
                    if (errorBuilder.Length > 0)
                    {
                        outputBuilder.AppendLine("Error:");
                        outputBuilder.AppendLine(errorBuilder.ToString());
                    }

                    return outputBuilder.ToString().TrimEnd();
                }
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}";
            }
        }
    }
}
