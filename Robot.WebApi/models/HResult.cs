using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.models
{
    public class HResult<T>
    {
        public int code { get; set; } = 0;
        public string message { get; set; } = "成功";
        public T? data { get; set; } = default;
    }
    public class HResult
    {
        public int code { get; set; } = 0;
        public string message { get; set; } = "成功";
    }
}
