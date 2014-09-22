using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;

namespace MongoDB.Component
{
    public class MongoDataContext : MongoBase
    {
        public MongoDataContext(string id)
        {
            var guid = Guid.Parse(id);

            var tbNode = MongoContext.GetTreeNode(guid);
            var dbNode = MongoContext.GetTreeNode(tbNode.PID);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(dbNode.ID) as MongoDatabase;
            Table = MongoContext.GetMongoObject(guid) as MongoCollection;
        }

        public int GetData()
        {
            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);

                var query = db.GetCollection("Pangu").FindAll();
                query.Limit(50);

                var levelDoc = db.SendCommand(new Document().Append("profile", -1));
                if (levelDoc != null)
                {
                    int ok = int.Parse(levelDoc["ok"].ToString());
                    if (ok == 1)
                    {
                        return int.Parse(levelDoc["was"].ToString());
                    }
                }
                return 0;
            }
        }
    }
}