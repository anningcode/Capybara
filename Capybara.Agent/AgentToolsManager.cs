using Capybara.Models;
using Capybara.Tool;
using LLMGateway.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public static class AgentToolsManager
    {
        private static List<IToolPlugin> loadPlugins_ = new List<IToolPlugin>();
        static AgentToolsManager()
        {
            Load();
        }
        private static void Load()
        {
            loadPlugins_ = loadPlugins();
        }
        public static string Invoke(LLMFunctionCallRequestInfo tool)
        {
            foreach (var item in loadPlugins_)
            {
                if (item.Contains(tool.Name))
                {
                    return item.Invoke(tool);
                }
            }
            return "未实现这个工具!";
        }
        public static List<LLMToolDefinitionInfo> GetTools(List<string> tools)
        {
            List<LLMToolDefinitionInfo> result = new List<LLMToolDefinitionInfo>();
            foreach (var item in loadPlugins_)
            {
                result.AddRange(item.GetTools(tools));
            }
            return result;
        }
        private static List<IToolPlugin> loadPlugins()
        {
            List<IToolPlugin> result = new List<IToolPlugin>();
            var dllFiles = Directory.EnumerateFiles(Path.Join(AppContext.BaseDirectory, "plugins"), "*.dll", SearchOption.AllDirectories);

            foreach (var path in dllFiles)
            {
                result.AddRange(loadPlugins(path));
            }

            return result;
        }
        private static List<IToolPlugin> loadPlugins(string assemblyPath)
        {
            try 
            {
                List<IToolPlugin> result = new List<IToolPlugin>();
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(IToolPlugin)))
                    {
                        object? instance = Activator.CreateInstance(type);
                        if (instance is IToolPlugin pluginInstance)
                        {
                            result.Add(pluginInstance);
                        }
                    }
                }
                return result;
            }
            catch { return new(); }
            
        }
    }
}
