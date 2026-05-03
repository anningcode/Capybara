using Capybara.Models;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace Capybara.Tool
{
    public abstract class IToolPlugin
    {
        public virtual bool Contains(string funcName)
        {
            var methods = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AgentFunctionAttribute>() != null);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<AgentFunctionAttribute>();
                if (attr == null) continue;
                if (attr.funcName == funcName) return true;
            }
            return false;
        }
        public virtual string Invoke(AgentLLMItemFuncRequestInfo tool)
        {
            try
            {
                MethodInfo? method = null;
                var methods = this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<AgentFunctionAttribute>() != null);
                foreach (var item in methods)
                {
                    var attr = item.GetCustomAttribute<AgentFunctionAttribute>();
                    if (attr == null) continue;
                    if (attr.funcName == tool.Name)
                    {
                        method = item;
                        break;
                    }
                }
                if (method == null) return $"方法 {tool.Name} 未找到!";

                var parameters = method.GetParameters();

                var json = JObject.Parse(tool.Arguments);

                if (json == null) throw new Exception();

                List<object> paramValues = new List<object>();

                foreach (var param in parameters)
                {
                    string name = param.Name ?? "";

                    if (!json.ContainsKey(name) && param.GetCustomAttribute<NoRequiredAttribute>() != null)
                    {
                        return $"调用失败,未提供 {name} 参数!";
                    }

                    if (param.ParameterType == typeof(int))
                    {
                        if (json.ContainsKey(name))
                        {
                            int? value = json[name]?.ToObject<int>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add((int)0);
                        }
                    }
                    else if (param.ParameterType == typeof(double))
                    {
                        if (json.ContainsKey(name))
                        {
                            double? value = json[name]?.ToObject<double>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add((double)0.0);
                        }
                    }
                    else if (param.ParameterType == typeof(long))
                    {
                        if (json.ContainsKey(name))
                        {
                            long? value = json[name]?.ToObject<long>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add((long)0);
                        }
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        if (json.ContainsKey(name))
                        {
                            string? value = json[name]?.ToObject<string>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add(string.Empty);
                        }
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        if (json.ContainsKey(name))
                        {
                            bool? value = json[name]?.ToObject<bool>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add(true);
                        }
                    }
                    else if (param.ParameterType == typeof(List<string>))
                    {
                        if (json.ContainsKey(name))
                        {
                            List<string>? value = json[name]?.ToObject<List<string>>();
                            if (value == null) return "调用失败,参数错误!";
                            paramValues.Add(value);
                        }
                        else
                        {
                            paramValues.Add(new List<string>());
                        }
                    }
                    else
                    {
                        return $"参数 {param.Name} 类型 {param.ParameterType.Name} 不支持!";
                    }
                }

                var result = method.Invoke(this, paramValues.ToArray());
                return result?.ToString() ?? "方法执行完成, 无返回值.";
            }
            catch
            {
                return $"方法 {tool.Name} 调用失败!";
            }
        }
        public virtual List<AgentLLMToolCallsRequestInfo> GetTools(List<string> tools)
        {
            Dictionary<Type, string> typeMapping = new Dictionary<Type, string>
            {
                { typeof(string),       "string" },
                { typeof(int),          "integer" },
                { typeof(long),         "integer" },
                { typeof(double),       "number" },
                { typeof(bool),         "boolean" },
                { typeof(List<string>), "array" }
            };

            List<AgentLLMToolCallsRequestInfo> result = new List<AgentLLMToolCallsRequestInfo>();

            var name = this.GetType().Name;
            var methods = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AgentFunctionAttribute>() != null && m.GetCustomAttribute<DescriptionAttribute>() != null);
            foreach (var method in methods)
            {
                try
                {
                    var attrFunction = method.GetCustomAttribute<AgentFunctionAttribute>();
                    var attrDescription = method.GetCustomAttribute<DescriptionAttribute>();
                    if (attrFunction == null || attrDescription == null) continue;

                    if (!tools.Contains(attrFunction.funcName)) continue;

                    // 构建方法
                    var item = new AgentLLMToolCallsRequestInfo();
                    item.Name = attrFunction.funcName;
                    item.Description = attrDescription.description;

                    // 构建参数
                    List<AgentLLMToolCallsArgumentRequestInfo> arguments = new List<AgentLLMToolCallsArgumentRequestInfo>();
                    foreach (var param in method.GetParameters())
                    {
                        var attr = param.GetCustomAttribute<DescriptionAttribute>();
                        if (attr == null || param == null || param.Name == null) continue;

                        var argument = new AgentLLMToolCallsArgumentRequestInfo();
                        argument.Name = param.Name;
                        argument.Type = typeMapping.ContainsKey(param.ParameterType) ? typeMapping[param.ParameterType] : throw new Exception("参数类型错误!");
                        argument.Description = attr.description;
                        arguments.Add(argument);
                    }
                    item.Arguments = arguments;
                    result.Add(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}
