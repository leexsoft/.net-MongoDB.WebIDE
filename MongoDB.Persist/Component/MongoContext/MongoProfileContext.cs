using System;
using System.Collections;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

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

        public ProfilingLevel GetProfileStatus()
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var rst = db.GetProfilingLevel();
            if(string.IsNullOrEmpty(rst.ErrorMessage))
            {
                if (rst.Ok)
                {
                    return rst.Level;
                }
                return ProfilingLevel.None;
            }
            else
            {
                throw new MongoException(rst.ErrorMessage);
            }
        }

        public bool SetProfile(int level, int slowms)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var rst = db.SetProfilingLevel((ProfilingLevel)level, new TimeSpan(0, 0, 0, 0, slowms));
            if(string.IsNullOrEmpty(rst.ErrorMessage))
            {
                return true;
            }
            else
            {
                throw new MongoException(rst.ErrorMessage);
            }
        }
    }
}