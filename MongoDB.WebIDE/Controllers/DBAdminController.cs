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

        #region 查询数据
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

        [HttpPost]
        public JsonResult Explain(string id, string data)
        {
            var mongo = new MongoDataContext(id);
            try
            {
                //mongo.CreateIndex(data);
                return Json(new { Success = true, Message = "索引创建成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
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
        public JsonResult CreateIndex(string id, string data)
        {
            var mongo = new MongoIndexContext(id);
            try
            {
                mongo.CreateIndex(data);
                return Json(new { Success = true, Message = "索引创建成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteIndex(string id, string guid)
        {
            var mongo = new MongoIndexContext(id);
            try
            {
                mongo.DeleteIndex(guid);
                return Json(new { Success = true, Message = "索引删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        #endregion

        #region Prfile优化
        [HttpPost]
        public JsonResult ShowPrfileInfo(string id)
        {
            var mongo = new MongoProfileContext(id);
            var model = new ProfileInfoModel
            {
                Status = 0 //mongo.GetProfileStatus()
            };
            return Json(model);
        }
        #endregion
    }
}
