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

            var tbNode = MongoCache.GetTreeNode(ID);
            Table = MongoCache.GetMongoObject(ID) as MongoCollection;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabase;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServer;
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