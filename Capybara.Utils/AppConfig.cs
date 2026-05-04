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
    }
}
