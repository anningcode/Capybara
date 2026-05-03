using Capybara.Models;
using Capybara.Tool;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Agent
{
    public class AgentToolsManager
    {
        private static AgentToolsManager instance_ = new AgentToolsManager();
        private List<IToolPlugin> loadPlugins_ = new List<IToolPlugin>();
        public static AgentToolsManager Instance { get { return instance_; } }
        private AgentToolsManager()
        {
            Load();
        }
        private void Load()
        {
            loadPlugins_ = loadPlugins();
        }
        public string Invoke(AgentLLMItemFuncRequestInfo tool)
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
        public List<AgentLLMToolCallsRequestInfo> GetTools(List<string> tools)
        {
            List<AgentLLMToolCallsRequestInfo> result = new List<AgentLLMToolCallsRequestInfo>();
            foreach (var item in loadPlugins_)
            {
                result.AddRange(item.GetTools(tools));
            }
            return result;
        }
        private List<IToolPlugin> loadPlugins()
        {
            List<IToolPlugin> result = new List<IToolPlugin>();
            string folderPath = "./plugins";
            var dllFiles = Directory.EnumerateFiles(folderPath, "*.dll", SearchOption.AllDirectories);

            foreach (var path in dllFiles)
            {
                result.AddRange(loadPlugins(path));
            }

            return result;
        }
        private List<IToolPlugin> loadPlugins(string assemblyPath)
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
    }
}
