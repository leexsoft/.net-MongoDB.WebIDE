using System;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Component;

namespace MongoDB.WebIDE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetServerDetail()
        {
            var nodes = MongoCache.GetTreeNodes().ToList();
            return Json(nodes);
        }

        #region 服务器管理
        public ActionResult ServerManager()
        {
            var mongo = new MongoServerContext();
            return View(mongo.GetServerNodes());
        }

        [HttpPost]
        public JsonResult AddServer(string ip, int port)
        {
            var mongo = new MongoServerContext();
            try
            {
                mongo.AddServer(ip, port);
                return Json(new { Success = true, Message = "添加服务器成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteServer(string ip, int port)
        {
            var mongo = new MongoServerContext();
            try
            {
                mongo.DeleteServer(ip, port);
                return Json(new { Success = true, Message = "删除服务器成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        #endregion

        public ActionResult CacheExpire()
        {
            return View();
        }
    }
}
