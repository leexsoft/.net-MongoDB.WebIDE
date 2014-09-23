using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoCollectionInfo : MongoBaseInfo
    {
        public MongoCollectionInfo(string id)
        {
            ID = Guid.Parse(id);

            var tbNode = MongoContext.GetTreeNode(ID);
            var dbNode = MongoContext.GetTreeNode(tbNode.PID);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(dbNode.ID) as MongoDatabase;
            Table = MongoContext.GetMongoObject(ID) as MongoCollection;
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
                var doc = db.SendCommand(new Document().Append("collstats", Table.Name));
                
                BuildTreeNode(list, Guid.Empty, doc);
                return list;
            }
        }
    }
}