using Newtonsoft.Json;

namespace Capybara.Utils
{
    public static class DeepCopy
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
        };

        /// <summary>深拷贝对象</summary>
        public static T? Clone<T>(T source)
        {
            if (source is null)
                return default;

            var json = JsonConvert.SerializeObject(source, Settings);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>深拷贝对象，支持引用循环</summary>
        public static T? ClonePreserveReferences<T>(T source)
        {
            if (source is null)
                return default;

            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            };

            var json = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>深拷贝</summary>
        public static void DeepCopyExcluding<T>(T source, ref T target, params string[] excludeProperties)
        {
            var excludeSet = new HashSet<string>(excludeProperties);
            foreach (var prop in typeof(T).GetProperties())
            {
                if (excludeSet.Contains(prop.Name)) continue;
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(target, prop.GetValue(source));
                }
            }
        }

        /// <summary>深拷贝对象，使用自定义序列化设置</summary>
        public static T? Clone<T>(T source, JsonSerializerSettings settings)
        {
            if (source is null)
                return default;

            var json = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
