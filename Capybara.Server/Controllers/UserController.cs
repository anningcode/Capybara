using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("user")]
    [AuthFilter(60)]
    public class UserController : HController
    {
        private IUserService user_ { get; set; }
        public UserController(IUserService user)
        {
            user_ = user;
        }
        [HttpGet("index")]
        [FileMapping("user/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<AgentChatUserInfo> Load(int page, int limit, string? id, string? name, int? roleId, bool? enable)
        {
            return user_.Select(id, name, roleId, enable);
        }
        [HttpGet("editView")]
        [FileMapping("user/edit.html")]
        public IActionResult EditView(string id)
        {
            var user = user_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到用户!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] AgentChatUserInfo value)
        {
            if (user_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("user/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] AgentChatUserInfo value)
        {
            if (user_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] string id)
        {
            if (user_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
