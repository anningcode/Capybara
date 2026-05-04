using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Utils
{
    public static class Logger
    {
        private static readonly NLog.Logger _logger;

        static Logger()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "nlog.config");
            if (File.Exists(configPath))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(configPath);
            }
            else
            {
                var config = new LoggingConfiguration();
                var consoleTarget = new ConsoleTarget("console")
                {
                    Layout = "${longdate} [${level:uppercase=true}] ${message} ${exception:format=ToString}"
                };
                var fileTarget = new FileTarget("file")
                {
                    FileName = "${basedir}/logs/${shortdate}.log",
                    Layout = "${longdate} [${level:uppercase=true}] ${logger} - ${message} ${exception:format=ToString}"
                };
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
                LogManager.Configuration = config;
            }

            _logger = LogManager.GetCurrentClassLogger();
        }

        public static void Trace(string message, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Trace(message);
            else
                _logger.Trace($"[{memberName}] {message}");
        }

        public static void Debug(string message, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Debug(message);
            else
                _logger.Debug($"[{memberName}] {message}");
        }

        public static void Info(string message, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Info(message);
            else
                _logger.Info($"[{memberName}] {message}");
        }

        public static void Warn(string message, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Warn(message);
            else
                _logger.Warn($"[{memberName}] {message}");
        }

        public static void Error(string message, Exception? ex = null, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Error(ex, message);
            else
                _logger.Error(ex, $"[{memberName}] {message}");
        }

        public static void Fatal(string message, Exception? ex = null, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
                _logger.Fatal(ex, message);
            else
                _logger.Fatal(ex, $"[{memberName}] {message}");
        }
    }
}
