using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("tool")]
    [AuthFilter(60)]
    public class ToolController : HController
    {
        private IToolService tool_ { get; set; }
        public ToolController(IToolService tool)
        {
            tool_ = tool;
        }
        [HttpGet("index")]
        [FileMapping("tool/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<AgentChatToolInfo> Load(int page, int limit, int? id, string? toolName, bool? enable)
        {
            return tool_.Select(id, toolName, enable);
        }
        [HttpGet("editView")]
        [FileMapping("tool/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = tool_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到工具!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] AgentChatToolInfo value)
        {
            if (tool_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("tool/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] AgentChatToolInfo value)
        {
            if (tool_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (tool_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
