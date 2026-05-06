using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("role")]
    [AuthFilter(60)]
    public class RoleController : HController
    {
        private IRoleService role_ { get; set; }
        public RoleController(IRoleService role)
        {
            role_ = role;
        }
        [HttpGet("index")]
        [FileMapping("role/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<AgentChatRoleInfo> Load(int page, int limit, int? id, string? name, int? modelId, bool? thinking, bool? enable)
        {
            return role_.Select(id, name, modelId, thinking, enable);
        }
        [HttpGet("editView")]
        [FileMapping("role/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = role_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到角色!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] AgentChatRoleInfo value)
        {
            if (role_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("role/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] AgentChatRoleInfo value)
        {
            if (role_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (role_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
