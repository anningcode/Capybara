using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("mcp")]
    [AuthFilter(60)]
    public class MCPController : HController
    {
        private IMCPService mcp_ { get; set; }
        public MCPController(IMCPService mcp)
        {
            mcp_ = mcp;
        }
        [HttpGet("index")]
        [FileMapping("mcp/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<WebMCPConfigInfo> Load(int page, int limit, int? id, bool? enable)
        {
            return mcp_.Select(id, enable);
        }
        [HttpGet("editView")]
        [FileMapping("mcp/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = mcp_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到配置!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] WebMCPConfigInfo value)
        {
            if (mcp_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("mcp/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] WebMCPConfigInfo value)
        {
            if (mcp_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (mcp_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
