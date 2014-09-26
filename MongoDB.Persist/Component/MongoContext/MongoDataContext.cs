using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;
using System.Linq;
using MongoDB.Bson;

namespace MongoDB.Component
{
    public class MongoDataContext : MongoBase
    {
        public MongoDataContext(string id)
        {
            ID = Guid.Parse(id);
            var fieldNode = MongoCache.GetTreeNode(ID);

            var tbNode = MongoCache.GetTreeNode(fieldNode.PID);
            Table = MongoCache.GetMongoObject(tbNode.ID) as MongoCollectionModel;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        public List<MongoTreeNode> GetFieldNodes()
        {
            return MongoCache.GetTreeNodes().Where(n => n.PID == ID).ToList();
        }

        public List<BsonDocument> GetData(int limit)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var query = db.GetCollection(Table.Name).FindAll();
            query.Limit = limit;
            return query.ToList();
        }

        public List<MongoTreeNode> Explain(string key, string val)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var doc = db.GetCollection(Table.Name).Find(MongoDocument.CreateQuery(key, val)).Explain(true);

            var list = new List<MongoTreeNode>();
            BuildTreeNode(list, Guid.Empty, doc);
            return list;
        }
    }
}