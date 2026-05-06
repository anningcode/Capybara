using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Capybara.Utils
{
    public static class AppConfig
    {
        private const string FileName = "config/appsettings.json";

        private static string GetFilePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, FileName);
        }

        private static JObject? GetRoot()
        {
            var path = GetFilePath();
            if (!File.Exists(path))
                return null;

            return JObject.Parse(File.ReadAllText(path));
        }

        public static T? Get<T>(string key)
        {
            var root = GetRoot();
            if (root == null)
                return default;

            var token = root[key];
            if (token != null)
                return token.ToObject<T>();

            return default;
        }

        public static bool Set<T>(string key, T value)
        {
            try
            {
                var path = GetFilePath();
                JObject root;

                if (File.Exists(path))
                {
                    root = JObject.Parse(File.ReadAllText(path));
                }
                else
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    root = [];
                }

                root[key] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
                File.WriteAllText(path, root.ToString(Formatting.Indented));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}