using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("skill")]
    [AuthFilter(60)]
    public class SkillController : HController
    {
        private ISkillService skill_ { get; set; }
        public SkillController(ISkillService skill)
        {
            skill_ = skill;
        }
        [HttpGet("index")]
        [FileMapping("skill/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<AgentChatSkillInfo> Load(int page, int limit, int? id, string? skillName, bool? enable)
        {
            return skill_.Select(id, skillName, enable);
        }
        [HttpGet("editView")]
        [FileMapping("skill/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = skill_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到技能!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] AgentChatSkillInfo value)
        {
            if (skill_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("skill/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] AgentChatSkillInfo value)
        {
            if (skill_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (skill_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
