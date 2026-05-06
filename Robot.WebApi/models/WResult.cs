using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.models
{
    public class WResult<T>
    {
        public int code { get; set; } = 0;
        public string message { get; set; } = "成功";
        public string route { get; set; } = string.Empty;
        public T? data { get; set; } = default;
    }
    public class WResult
    {
        public int code { get; set; } = 0;
        public string message { get; set; } = "成功";
        public string route { get; set; } = string.Empty;
    }
}
