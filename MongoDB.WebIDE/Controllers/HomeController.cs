using System;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Component;
using MongoDB.Persist.Web.Atrribute;

namespace MongoDB.WebIDE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [JsonException]
        public JsonResult GetServerDetail()
        {
            var nodes = MongoCache.GetTreeNodes().ToList();
            return Json(new { Success = true, Message = "获取数据库对象成功", Result = nodes });
        }

        #region 服务器管理
        public ActionResult ServerManager()
        {
            var mongo = new MongoServerContext();
            return View(mongo.GetServerNodes());
        }

        [HttpPost]
        [JsonException]
        public JsonResult AddServer(string ip, int port)
        {
            var mongo = new MongoServerContext();
            mongo.AddServer(ip, port);
            return Json(new { Success = true, Message = "添加服务器成功" });
        }

        [HttpPost]
        [JsonException]
        public JsonResult DeleteServer(string ip, int port)
        {
            var mongo = new MongoServerContext();
            mongo.DeleteServer(ip, port);
            return Json(new { Success = true, Message = "删除服务器成功" });
        }
        #endregion

        public ActionResult CacheExpire()
        {
            return View();
        }
    }
}
