using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Capybara.Models;

namespace Capybara.Agent
{
    public class AgentConfigManager
    {
        private const string fileName_ = "config/settings.json";
        private AgentConfigManager() { }
        public static List<T> GetConfig<T>(string sectionName, params (string, object)[] conditions) where T : class, new()
        {
            JObject json = LoadConfig();
            List<T> result = new List<T>();
            if (json.ContainsKey(sectionName))
            {
                List<T>? values = json[sectionName]?.ToObject<List<T>>();
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        bool success = false;
                        foreach (var condition in conditions)
                        {
                            PropertyInfo? property = typeof(T).GetProperty(condition.Item1);
                            if (property == null) return new List<T>();
                            object? propertyValue = property.GetValue(value);
                            if (propertyValue == null) return new List<T>();
                            success = IsSame(propertyValue, condition.Item2);
                            if (!success)
                            {
                                success = false;
                                break;
                            }
                        }
                        if (success || conditions.Count() == 0)
                            result.Add(value);
                    }
                }
            }
            return result;
        }
        private static bool IsSame(object obj1, object obj2)
        {
            if (obj1.GetType() == obj2.GetType() && obj1.GetType() == typeof(int))
            {
                return (int)obj1 == (int)obj2;
            }
            else if (obj1.GetType() == obj2.GetType() && obj1.GetType() == typeof(string))
            {
                return (string)obj1 == (string)obj2;
            }
            else if (obj1.GetType() == obj2.GetType() && obj1.GetType() == typeof(double))
            {
                return (double)obj1 == (double)obj2;
            }
            else if (obj1.GetType() == obj2.GetType() && obj1.GetType() == typeof(long))
            {
                return (long)obj1 == (long)obj2;
            }
            else if (obj1.GetType() == obj2.GetType() && obj1.GetType() == typeof(bool))
            {
                return (bool)obj1 == (bool)obj2;
            }
            else if (obj1.GetType() == typeof(int) && obj2.GetType() == typeof(List<int>))
            {
                return ((List<int>)obj2).Contains((int)obj1);
            }
            else if (obj1.GetType() == typeof(string) && obj2.GetType() == typeof(List<string>))
            {
                return ((List<string>)obj2).Contains((string)obj1);
            }
            else
            {
                return false;
            }
        }
        private static JObject LoadConfig()
        {
            try
            {
                if (!File.Exists(fileName_))
                    return JObject.Parse("{}");
                string text = File.ReadAllText(fileName_);
                return JObject.Parse(text);
            }
            catch 
            {
                return JObject.Parse("{}");
            }
        }
    }
}
