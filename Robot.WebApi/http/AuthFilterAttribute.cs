using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Robot.WebApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.http
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public AuthFilterAttribute() { }
        public AuthFilterAttribute(int time)
        {
            SessionKeyManager.minute_ = time;
        }
        // 请求
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSession session = HttpSession.IsSession(context) ? HttpSession.GetSession(context) : new HttpSession(context);

            // 检测是否需要授权
            var hasIgnore = context.ActionDescriptor.FilterDescriptors
                .Any(fd => fd.Filter is AuthIgnoreAttribute);

            if (hasIgnore) { base.OnActionExecuting(context); return; }

            // 授权后不能访问
            var hasNo = context.ActionDescriptor.FilterDescriptors
            .Any(fd => fd.Filter is AuthReverseAttribute);

            if (hasNo)
            {
                if (session.IsAuthorize())
                {
                    context.Result = new ContentResult()
                    {
                        Content = JsonConvert.SerializeObject(new HResult { code = 101, message = "已经授权不能访问!" }),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }
            }
            else if (!session.IsAuthorize())
            {
                context.Result = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(new HResult { code = 100, message = "未授权!" }),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
            base.OnActionExecuting(context);
        }
        // 响应
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            HttpSession.UpdateSession(context);
            base.OnActionExecuted(context);
        }
    }
    // 忽略授权
    public class AuthIgnoreAttribute : ActionFilterAttribute { }
    // 授权后不能进去
    public class AuthReverseAttribute : ActionFilterAttribute { }
    // 文件映射
    public class FileMappingAttribute : Attribute
    { 
        public string FileName { get; set; }
        public FileMappingAttribute(string fielName) 
        {
            FileName = fielName;
        }
    }
}
