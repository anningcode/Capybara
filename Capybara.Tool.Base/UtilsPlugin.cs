using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool.Base
{
    public class UtilsPlugin : IToolPlugin
    {
        // 创建GUID
        [AgentFunction("create_guid")]
        [Description("创建guid")]
        public string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }
        // 获取当前时间
        [AgentFunction("get_current_datetime")]
        [Description("获取当前时间")]
        public string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        // 获取当前时间戳
        [AgentFunction("get_current_timestamp")]
        [Description("获取当前时间戳")]
        public string GetCurrentTimestamp()
        {
            return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        }
    }
}
