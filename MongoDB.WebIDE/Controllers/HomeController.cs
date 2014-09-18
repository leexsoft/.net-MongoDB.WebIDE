using System.Web.Mvc;
using MongoDB.Component;
using MongoDB.Defination;
using System;
using MongoDB.WebIDE.Models;
using MongoDB.Model;
using MongoDB.Component;

namespace MongoDB.WebIDE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CacheExpire()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetServerDetail()
        {
            var nodes = MongoContext.GetTreeNodes();
            return Json(nodes);
        }

        public ActionResult ShowInfo(string id, int type)
        {
            var model = new ShowInfoModel
            {
                Id = id,
                Type = type
            };

            var gid = Guid.Parse(id);
            if (type == (int)MongoTreeNodeType.Server)
            {
                var server = MongoContext.GetMongoObject(gid) as MongoServer;
                if (server == null)
                {
                    RedirectToAction("CacheExpire");
                }
                else
                {
                    model.Name = server.FullInfo;
                }
            }
            else if (type == (int)MongoTreeNodeType.Database)
            {
                var database = MongoContext.GetMongoObject(gid) as MongoDatabase;
                if (database == null)
                {
                    RedirectToAction("CacheExpire");
                }
                else
                {
                    model.Name = database.FullInfo;
                }
            }
            else if (type == (int)MongoTreeNodeType.Collection)
            {
                var table = MongoContext.GetMongoObject(gid) as MongoCollection;
                if (table == null)
                {
                    RedirectToAction("CacheExpire");
                }
                else
                {
                    model.Name = table.FullInfo;
                }
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult GetShowInfo(string id, int type)
        {
            var mongo = MongoInfoFactory.Create(id, type);
            var nodes = mongo.GetInfo();
            return Json(nodes);
        }

        [HttpPost]
        public JsonResult GetPrfileInfo(string id, int type)
        {
            var mongo = MongoInfoFactory.Create(id, type);
            var model = new ProfileInfoModel
            {
                Status = mongo.GetProfileStatus()
            };
            return Json(model);
        }
    }
}
