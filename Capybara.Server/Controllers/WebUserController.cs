using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("webuser")]
    [AuthFilter(60)]
    public class WebUserController : HController
    {
        private IWebUserService user_ { get; set; }
        public WebUserController(IWebUserService user)
        {
            user_ = user;
        }
        [HttpGet("index")]
        [FileMapping("webuser/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<WebUserConfigInfo> Load(int page, int limit, int? id, string? name, string? code, bool? enable)
        {
            return user_.Select(id, name, code, enable);
        }
        [HttpGet("editView")]
        [FileMapping("webuser/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = user_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到用户!" });
            user.Password = "******";
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] WebUserConfigInfo value)
        {
            if (user_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("webuser/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] WebUserConfigInfo value)
        {
            if (value.Code.Length < 5) return Result(1, "账号错误!");
            else if (value.Password.Length < 6) return Result(2, "密码不能小于6位数!");
            else if (value.Name.Length < 0) return Result(3, "名称不能是空!");
            if (user_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (user_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
