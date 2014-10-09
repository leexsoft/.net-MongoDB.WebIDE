using System;
using System.Web.Mvc;
using MongoDB.Component;
using MongoDB.Defination;
using MongoDB.Model;
using MongoDB.Persist.Web.Atrribute;
using MongoDB.WebIDE.Models;
using Newtonsoft.Json;

namespace MongoDB.WebIDE.Controllers
{
    [CacheValid]
    public class DBAdminController : Controller
    {
        #region 查看统计信息
        public ActionResult ShowInfo(string id, int type)
        {
            var model = new ShowInfoModel
            {
                ID = id,
                Type = type
            };

            //获取描述
            var gid = Guid.Parse(id);
            if (type == (int)MongoTreeNodeType.Server)
            {
                var server = MongoCache.GetMongoObject(gid) as MongoServerModel;
                model.Title = server.FullInfo;
            }
            else if (type == (int)MongoTreeNodeType.Database)
            {
                var database = MongoCache.GetMongoObject(gid) as MongoDatabaseModel;
                model.Title = database.FullInfo;
            }
            else if (type == (int)MongoTreeNodeType.Collection)
            {
                var table = MongoCache.GetMongoObject(gid) as MongoCollectionModel;
                model.Title = table.FullInfo;
            }

            //获取数据
            var mongo = MongoInfoFactory.Create(id, type);
            model.JsonData = JsonConvert.SerializeObject(mongo.GetInfo());

            return View(model);
        }
        #endregion

        #region 主从信息
        public ActionResult ShowReplication(string id)
        {
            var mongo = new MongoReplicationContext(id);
            var hash = mongo.GetReplicationInfo();
            var model = new ShowReplicationModel
            {
                ID = id,
                Title = mongo.Server.FullInfo,
                JsonServer = JsonConvert.SerializeObject(hash[0]),
                JsonData = JsonConvert.SerializeObject(hash[1])
            };
            return View(model);
        }
        #endregion

        #region Prfile优化
        public ActionResult ShowProfile(string id)
        {
            var mongo = new MongoProfileContext(id);
            var model = new ShowProfileModel
            {
                ID = id,
                Title = mongo.Database.FullInfo,
                Status = mongo.GetProfileStatus()
            };
            return View(model);
        }

        [HttpPost]
        [JsonException]
        public JsonResult SetProfile(string id, int level, int slowms)
        {
            var mongo = new MongoProfileContext(id);
            mongo.SetProfile(level, slowms);
            return Json(new { Success = true, Message = "Profile设置成功" });
        }

        [HttpPost]
        [JsonException]
        public JsonResult GetProfileData(string id, int limit)
        {
            var mongo = new MongoProfileContext(id);
            var list = mongo.GetProfileData(limit);
            return Json(new { Success = true, Message = string.Empty, Result = list });
        }
        #endregion

        #region 查询数据
        public ActionResult ShowData(string id)
        {
            var mongo = new MongoDataContext(id);
            var model = new ShowDataModel
            {
                ID = id,
                Title = mongo.Table.FullInfo,
                Fields = mongo.GetFields()
            };
            return View(model);
        }

        [HttpPost]
        [JsonException]
        public JsonResult GetData(string id, string jsonfind, string jsonsort, int skip, int limit)
        {
            var mongo = new MongoDataContext(id);
            var list = mongo.GetData(jsonfind, jsonsort, skip, limit);
            var str = JsonConvert.SerializeObject(list);
            return Json(new { Success = true, Message = string.Empty, Result = str });
        }

        [HttpPost]
        [JsonException]
        public JsonResult Explain(string id, string jsonfind, string jsonsort)
        {
            var mongo = new MongoDataContext(id);
            var list = mongo.Explain(jsonfind, jsonsort);
            return Json(new { Success = true, Message = string.Empty, Result = list });
        }
        #endregion

        #region 索引管理
        public ActionResult ShowIndex(string id)
        {
            var mongo = new MongoIndexContext(id);
            var model = new ShowIndexModel
            {
                ID = id,
                Title = mongo.Table.FullInfo,
                Fields = mongo.GetFieldNodes(),
                Indexes = mongo.GetIndexes()
            };
            return View(model);
        }

        [HttpPost]
        [JsonException]
        public JsonResult CreateIndex(string id, string data)
        {
            var mongo = new MongoIndexContext(id);
            mongo.CreateIndex(data);
            return Json(new { Success = true, Message = "索引创建成功" });
        }

        [HttpPost]
        [JsonException]
        public JsonResult DeleteIndex(string id, string guid)
        {
            var mongo = new MongoIndexContext(id);
            mongo.DeleteIndex(guid);
            return Json(new { Success = true, Message = "索引删除成功" });
        }
        #endregion
    }
}
