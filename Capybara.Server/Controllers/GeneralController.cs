using Capybara.IService;
using Capybara.Models;
using Microsoft.AspNetCore.Mvc;
using Robot.WebApi.http;
using Robot.WebApi.models;

namespace Capybara.Server.Controllers
{
    [ApiController]
    [Route("general")]
    [AuthFilter(60)]
    public class GeneralController : HController
    {
        private IGeneralService general_ { get; set; }
        public GeneralController(IGeneralService general)
        {
            general_ = general;
        }
        [HttpGet("index")]
        [FileMapping("general/index.html")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("load")]
        public WebResponseInfo<WebGeneralConfigInfo> Load(int page, int limit, int? id, string? key, string? value, bool? enable)
        {
            return general_.Select(id, key, value, enable);
        }
        [HttpGet("editView")]
        [FileMapping("general/edit.html")]
        public IActionResult EditView(int id)
        {
            var user = general_.Select(id);
            if (user == null) return NotFound(new { error = "没有找到配置!" });
            return View(user);
        }
        [HttpPost("edit")]
        public HResult Edit([FromBody] WebGeneralConfigInfo value)
        {
            if (general_.Update(value))
            {
                return Result(0, "更新成功!");
            }
            return Result(1, "更新失败!");
        }
        [HttpGet("addView")]
        [FileMapping("general/add.html")]
        public IActionResult AddView()
        {
            return View();
        }
        [HttpPost("add")]
        public HResult Add([FromBody] WebGeneralConfigInfo value)
        {
            if (general_.Insert(value))
            {
                return Result(0, "添加成功!");
            }
            return Result(1, "添加失败!");
        }
        [HttpPost("remove")]
        public HResult Remove([FromForm] int id)
        {
            if (general_.Delete(id))
            {
                return Result(0, "删除成功!");
            }
            return Result(1, "删除失败!");
        }
    }
}
