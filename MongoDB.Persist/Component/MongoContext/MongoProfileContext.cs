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
            Database = MongoCache.GetMongoObject(guid) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        public int GetProfileStatus()
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var levelDoc = db.SendCommand(MongoDocument.CreateQuery("profile", -1));
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