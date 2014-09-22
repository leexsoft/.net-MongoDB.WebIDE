using System;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Component;
using MongoDB.Defination;
using MongoDB.WebIDE.Models;
using MongoDB.Model;
using MongoDB.Component;
using System.Collections.Generic;

namespace MongoDB.WebIDE.Controllers
{
    public class DBAdminController : Controller
    {
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
        public JsonResult GetPrfileInfo(string id)
        {
            var mongo = new MongoProfileContext(id);
            var model = new ProfileInfoModel
            {
                Status = mongo.GetProfileStatus()
            };
            return Json(model);
        }

        public ActionResult ShowIndex(string id)
        {
            var guid = Guid.Parse(id);
            var list = MongoContext.GetTreeNodes().Where(node => node.PID == guid).ToList();

            var indexes = new List<MongoIndex>();
            foreach (var item in list)
            {
                indexes.Add(MongoContext.GetMongoObject(item.ID) as MongoIndex);
            }
            return View(indexes);
        }
    }
}
