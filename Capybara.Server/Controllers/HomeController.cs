using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("home")]
    [AuthFilter(60)]
    public class HomeController : HController
    {
        [HttpGet("index")]
        [FileMapping("home/index.html")]
        public IActionResult Index()
        {
            var session = HttpSession.GetSession(Request);
            var user = session["user"] as dynamic;
            return View(new { UserName = user.name });
        }
        [HttpGet("init")]
        [FileMapping("content/api/init.json")]
        public IActionResult Init()
        {
            return View();
        }
        [HttpGet("clear")]
        [FileMapping("content/api/clear.json")]
        public IActionResult Clear()
        {
            return View();
        }
    }
}
