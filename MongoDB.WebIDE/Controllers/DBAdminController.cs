using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Component;
using MongoDB.Defination;
using MongoDB.Model;
using MongoDB.WebIDE.Models;
using Newtonsoft.Json;


namespace MongoDB.WebIDE.Controllers
{
    public class DBAdminController : Controller
    {
        public ActionResult ShowInfo(string id, int type)
        {
            var model = new ShowInfoModel 
            {
                Type = type
            };

            //获取描述
            var gid = Guid.Parse(id);
            if (type == (int)MongoTreeNodeType.Server)
            {
                var server = MongoContext.GetMongoObject(gid) as MongoServer;
                if (server == null)
                {
                    return RedirectToAction("CacheExpire", "Home");
                }
                else
                {
                    model.Title = server.FullInfo;
                }
            }
            else if (type == (int)MongoTreeNodeType.Database)
            {
                var database = MongoContext.GetMongoObject(gid) as MongoDatabase;
                if (database == null)
                {
                    return RedirectToAction("CacheExpire", "Home");
                }
                else
                {
                    model.Title = database.FullInfo;
                }
            }
            else if (type == (int)MongoTreeNodeType.Collection)
            {
                var table = MongoContext.GetMongoObject(gid) as MongoCollection;
                if (table == null)
                {
                    return RedirectToAction("CacheExpire", "Home");
                }
                else
                {
                    model.Title = table.FullInfo;
                }
            }

            //获取数据
            var mongo = MongoInfoFactory.Create(id, type);
            model.JsonData = JsonConvert.SerializeObject(mongo.GetInfo());

            return View(model);
        }

        [HttpPost]
        public JsonResult ShowPrfileInfo(string id)
        {
            var mongo = new MongoProfileContext(id);
            var model = new ProfileInfoModel
            {
                Status = mongo.GetProfileStatus()
            };
            return Json(model);
        }

        public ActionResult ShowData(string id)
        {
            var mongo = new MongoDataContext(id);
            var model = new ShowDataModel
            {
                Title = mongo.Table.FullInfo,
                Fields = mongo.GetFieldNodes(),
                Data = mongo.GetData(50)
            };
            return View(model);
        }

        public ActionResult ShowIndex(string id)
        {
            var mongo = new MongoIndexContext(id);
            var model = new ShowIndexModel
            {
                Title = mongo.Table.FullInfo,
                Indexes = mongo.GetIndexes()
            };
            return View(model);
        }
    }
}
