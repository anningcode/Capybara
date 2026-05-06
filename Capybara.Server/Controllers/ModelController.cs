using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("model")]
    [AuthFilter(60)]
    public class ModelController : HController
    {
        private IModelService model_ { get; set; }
        public ModelController(IModelService model)
        {
            model_ = model;
        }
        [HttpGet("index")]
        [FileMapping("model/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<AgentChatModelInfo> Load(int page, int limit, int? id, string? name, bool? isSubAgent, bool? enable)
        {
            return model_.Select(id, name, isSubAgent, enable);
        }
        [HttpGet("editView")]
        [FileMapping("model/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = model_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到模型!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] AgentChatModelInfo value)
        {
            if (model_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("model/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] AgentChatModelInfo value)
        {
            if (value.ModelName.Length < 1) return Result(1, "模型名称错误!");
            if (model_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (model_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
