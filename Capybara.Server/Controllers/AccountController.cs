using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("account")]
    [AuthFilter(60)]
    public class AccountController : HController
    {
        private IAccoutService account_ { get; set; }
        [HttpGet("index")]
        [AuthReverse]
        [FileMapping("account/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        public AccountController(IAccoutService account)
        {
            account_ = account;
        }
        [HttpGet("login")]
        [AuthReverse]
        public HResult Login(string code, string password)
        {
            var user = account_.Login(code, password);
            if (user == null) return Result(1, "ÕËºÅ»òÃÜÂë´íÎó");
            var session = HttpSession.GetSession(Request);
            if (session == null) return Result(2, "µÇÂ¼Òì³£");

            session["user"] = user;
            session.SetAuthorize(true);

            return Result(0, "µÇÂ½³É¹¦");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var session = HttpSession.GetSession(Request);
            session.SetAuthorize(false);
            return Jump("/");
        }
    }
}
