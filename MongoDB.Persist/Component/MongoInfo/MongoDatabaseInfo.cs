using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoDatabaseInfo : MongoBaseInfo
    {
        public MongoDatabaseInfo(string id)
        {
            ID = Guid.Parse(id);

            var dbNode = MongoCache.GetTreeNode(ID);
            Database = MongoCache.GetMongoObject(ID) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);
            var doc = db.SendCommand(MongoDocument.CreateQuery("dbstats", 1));

            var list = new List<MongoTreeNode>();
            BuildTreeNode(list, Guid.Empty, doc);
            return list;
        }
    }
}