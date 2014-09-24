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
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);

            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoCache.GetMongoObject(ID) as MongoDatabase;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            var list = new List<MongoTreeNode>();
            if (Server == null || Database == null)
            {
                return list;
            }

            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);
                var doc = db.SendCommand(new Document().Append("dbstats", 1));

                BuildTreeNode(list, Guid.Empty, doc);
                return list;
            }
        }
    }
}