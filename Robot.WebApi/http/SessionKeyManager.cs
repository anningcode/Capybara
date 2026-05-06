using Newtonsoft.Json.Linq;
using Robot.WebApi.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.http
{
    public class SessionKeyManager
    {
        public static int minute_ { get; set; } = 180;
        public static List<(long, string)> sessionKeys_ { get; set; } = new();
        private Timer timer_ { get; set; }
        public SessionKeyManager()
        {
            timer_ = new Timer(new TimerCallback(OnTimeout));
            timer_.Change(1000 * 60 * 60, 1000 * 60 * 60);
            OnTimeout(null);
        }
        // 加密
        public string GetEncryptValue(string data)
        {
            string key = string.Empty;
            lock (sessionKeys_)
            {
                key = sessionKeys_[0].Item2;
            }
            return AESEncryptor.Encrypt(data, key);
        }
        // 解密
        public (bool, string) GetDecryptValue(string data)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<(long, string)> sessionKeys = new();
            lock (sessionKeys_)
            {
                if (sessionKeys_.Count > 1)
                {
                    sessionKeys_.RemoveAll(x => x.Item1 + 180 * 60 < currentTime);
                }
                sessionKeys.AddRange(sessionKeys_);
            }
            foreach (var key in sessionKeys)
            {
                try
                {
                    string json = AESEncryptor.Decrypt(data, key.Item2);
                    return (true, json);
                }
                catch { }
            }
            return (false, string.Empty);
        }

        private void OnTimeout(object? state)
        {
            string path = "appsettings.json";
            (long, string)? sessionKey = null;
            while (true)
            {
                try
                {
                    if (!File.Exists(path)) break;
                    string content = File.ReadAllText(path);
                    JObject json = JObject.Parse(content);
                    if (json.ContainsKey("SessionKey"))
                    {
                        sessionKey = (DateTimeOffset.UtcNow.ToUnixTimeSeconds(), ((string?)json["SessionKey"]) ?? "zhejiangyingjiekeji123456789");
                    }
                }
                catch { }
                break;
            }
            if (sessionKey == null)
            {
                sessionKey = (DateTimeOffset.UtcNow.ToUnixTimeSeconds(), "zhejiangyingjiekeji123456789");
            }

            lock (sessionKeys_)
            {
                if (sessionKeys_.Where(x => x.Item2 == sessionKey.Value.Item2).Count() == 0)
                {
                    sessionKeys_.Insert(0, (sessionKey.Value.Item1, sessionKey.Value.Item2));
                }
            }
        }
    }
}
