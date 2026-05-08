using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Robot.WebApi.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.http
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        private string? defautlPage_ { get; set; } = null;
        private string? homePage_ { get; set; } = null; 
        public AuthFilterAttribute() { }
        public AuthFilterAttribute(int time, string? defautlPage = null, string? homePage = null)
        {
            defautlPage_ = defautlPage;
            homePage_ = homePage;
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
                    if (homePage_ != null)
                    {
                        context.Result = new RedirectResult(homePage_);
                    }
                    else
                    {
                        context.Result = new ContentResult()
                        {
                            Content = JsonConvert.SerializeObject(new HResult { code = 101, message = "已经授权不能访问!" }),
                            ContentType = "application/json",
                            StatusCode = (int)HttpStatusCode.Forbidden
                        };
                    }
                }
            }
            else if (!session.IsAuthorize())
            {
                if (defautlPage_ != null)
                {
                    context.Result = new RedirectResult(defautlPage_);
                }
                else
                {
                    context.Result = new ContentResult()
                    {
                        Content = JsonConvert.SerializeObject(new HResult { code = 100, message = "未授权!" }),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }
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
