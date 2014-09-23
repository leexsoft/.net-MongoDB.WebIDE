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
            var guid = Guid.Parse(id);

            var fieldNode = MongoContext.GetTreeNode(guid);
            var tbNode = MongoContext.GetTreeNode(fieldNode.PID);
            var dbNode = MongoContext.GetTreeNode(tbNode.PID);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(dbNode.ID) as MongoDatabase;
            Table = MongoContext.GetMongoObject(tbNode.ID) as MongoCollection;
        }

        public List<MongoTreeNode> GetFieldNodes(string id)
        {
            var guid = Guid.Parse(id);
            return MongoContext.GetTreeNodes().Where(n => n.PID == guid).ToList();
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