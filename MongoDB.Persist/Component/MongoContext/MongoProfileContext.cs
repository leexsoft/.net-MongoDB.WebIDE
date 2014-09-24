using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;

namespace MongoDB.Component
{
    public class MongoProfileContext : MongoBase
    {
        public MongoProfileContext(string id)
        {
            var guid = Guid.Parse(id);

            var dbNode = MongoCache.GetTreeNode(guid);
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);

            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoCache.GetMongoObject(guid) as MongoDatabase;
        }

        public int GetProfileStatus()
        {
            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);
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