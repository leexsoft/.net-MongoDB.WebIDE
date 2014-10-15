using System;
using System.Linq;
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
        public ActionResult ShowInfo(uint id, int type)
        {
            var model = new ShowInfoModel
            {
                ID = id,
                Type = type
            };

            //获取描述
            if (type == (int)MongoTreeNodeType.Server)
            {
                var server = MongoCache.GetMongoObject(id) as MongoServerModel;
                model.Title = server.FullInfo;
            }
            else if (type == (int)MongoTreeNodeType.Database)
            {
                var database = MongoCache.GetMongoObject(id) as MongoDatabaseModel;
                model.Title = database.FullInfo;
            }
            else if (type == (int)MongoTreeNodeType.Collection)
            {
                var table = MongoCache.GetMongoObject(id) as MongoCollectionModel;
                model.Title = table.FullInfo;
                var tblFilter = MongoCache.GetTreeNodes().Single(node => node.PID == id && node.Type == MongoTreeNodeType.TableFiller);
                model.TblFillerID = tblFilter.ID;
                var idxFilter = MongoCache.GetTreeNodes().Single(node => node.PID == id && node.Type == MongoTreeNodeType.IndexFiller);
                model.IdxFillerID = idxFilter.ID;
            }

            //获取数据
            var mongo = MongoInfoFactory.Create(id, type);
            model.JsonData = JsonConvert.SerializeObject(mongo.GetInfo());

            return View(model);
        }
        #endregion

        #region 主从信息
        public ActionResult ShowReplication(uint id)
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
        public ActionResult ShowProfile(uint id)
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
        public JsonResult SetProfile(uint id, int level, int slowms)
        {
            var mongo = new MongoProfileContext(id);
            mongo.SetProfile(level, slowms);
            return Json(new { Success = true, Message = "Profile设置成功" });
        }

        [HttpPost]
        [JsonException]
        public JsonResult GetProfileData(uint id, int limit)
        {
            var mongo = new MongoProfileContext(id);
            var list = mongo.GetProfileData(limit);
            return Json(new { Success = true, Message = string.Empty, Result = list });
        }
        #endregion

        #region 查询数据
        public ActionResult ShowData(uint id)
        {
            var mongo = new MongoDataContext(id);
            var model = new ShowDataModel
            {
                ID = id,
                TblID = mongo.Table.ID,
                Title = mongo.Table.FullInfo,
                Fields = mongo.GetFields()
            };
            return View(model);
        }

        [HttpPost]
        [JsonException]
        public JsonResult GetData(uint id, string jsonfind, string jsonsort, int skip, int limit)
        {
            var mongo = new MongoDataContext(id);
            var list = mongo.GetData(jsonfind, jsonsort, skip, limit);
            var str = JsonConvert.SerializeObject(list);
            return Json(new { Success = true, Message = string.Empty, Result = str });
        }

        [HttpPost]
        [JsonException]
        public JsonResult Explain(uint id, string jsonfind, string jsonsort)
        {
            var mongo = new MongoDataContext(id);
            var list = mongo.Explain(jsonfind, jsonsort);
            return Json(new { Success = true, Message = string.Empty, Result = list });
        }
        #endregion

        #region 索引管理
        public ActionResult ShowIndex(uint id)
        {
            var mongo = new MongoIndexContext(id);
            var model = new ShowIndexModel
            {
                ID = id,
                TblID = mongo.Table.ID,
                Title = mongo.Table.FullInfo,
                Fields = mongo.GetFieldNodes(),
                Indexes = mongo.GetIndexes()
            };
            return View(model);
        }

        [HttpPost]
        [JsonException]
        public JsonResult CreateIndex(uint id, string data)
        {
            var mongo = new MongoIndexContext(id);
            mongo.CreateIndex(data);
            return Json(new { Success = true, Message = "索引创建成功" });
        }

        [HttpPost]
        [JsonException]
        public JsonResult DeleteIndex(uint id, uint idx)
        {
            var mongo = new MongoIndexContext(id);
            mongo.DeleteIndex(idx);
            return Json(new { Success = true, Message = "索引删除成功" });
        }
        #endregion
    }
}
