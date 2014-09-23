using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoIndexContext : MongoBase
    {
        public MongoIndexContext(string id)
        {
            ID = Guid.Parse(id);

            var idxNode = MongoContext.GetTreeNode(ID);
            var tbNode = MongoContext.GetTreeNode(idxNode.PID);
            var dbNode = MongoContext.GetTreeNode(tbNode.PID);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(dbNode.ID) as MongoDatabase;
            Table = MongoContext.GetMongoObject(tbNode.ID) as MongoCollection;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public List<MongoIndex> GetIndexes()
        {
            var list = MongoContext.GetTreeNodes().Where(node => node.PID == ID).ToList();

            var indexes = new List<MongoIndex>();
            foreach (var item in list)
            {
                indexes.Add(MongoContext.GetMongoObject(item.ID) as MongoIndex);
            }
            return indexes;
        }
    }
}