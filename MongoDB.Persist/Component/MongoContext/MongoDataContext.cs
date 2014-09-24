using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MongoDB.Component
{
    public class MongoDataContext : MongoBase
    {
        public MongoDataContext(string id)
        {
            ID = Guid.Parse(id);
            var fieldNode = MongoCache.GetTreeNode(ID);

            var tbNode = MongoCache.GetTreeNode(fieldNode.PID);
            Table = MongoCache.GetMongoObject(tbNode.ID) as MongoCollection;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabase;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServer;
        }

        public List<MongoTreeNode> GetFieldNodes()
        {
            return MongoCache.GetTreeNodes().Where(n => n.PID == ID).ToList();
        }

        public List<Document> GetData(int limit)
        {
            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);

                var query = db.GetCollection(Table.Name).FindAll().Limit(limit);
                return query.Documents.ToList();
            }
        }
    }
}