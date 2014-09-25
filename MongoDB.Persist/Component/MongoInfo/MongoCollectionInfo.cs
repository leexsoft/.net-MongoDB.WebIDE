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
            Table = MongoCache.GetMongoObject(ID) as MongoCollectionModel;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabaseModel;
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
            var adminDB = server.GetDatabase(MongoConst.AdminDBName);
            var doc = adminDB.SendCommand(MongoDocument.CreateQuery("collstats", Table.Name));

            var list = new List<MongoTreeNode>();
            BuildTreeNode(list, Guid.Empty, doc);
            return list;
        }
    }
}