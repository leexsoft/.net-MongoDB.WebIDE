using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Driver.Builders;

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

        public List<MongoProfileModel> GetProfileData(int limit)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var cursors = db.GetProfilingInfo(Query.Null).SetLimit(limit);
            var list = new List<MongoProfileModel>();
            foreach (var item in cursors)
            {
                list.Add(new MongoProfileModel 
                {
                    Client = item.Client,
                    Timestamp = item.Timestamp,
                    Duration = item.Duration.TotalMilliseconds,
                    Info = string.IsNullOrEmpty(item.Info) ? "" : item.Info,
                    Op = item.Op,
                    NumberToReturn = item.NumberToReturn,
                    NumberReturned = item.NumberReturned,
                    NumberScanned = item.NumberScanned,
                    NumberOfYields = item.NumberOfYields,
                    NumberMoved = item.NumberMoved,
                    NumberUpdated = item.NumberUpdated
                });
            }
            return list;
        }

        public bool SetProfile(int level, int slowms)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            CommandResult rst = null;
            if ((ProfilingLevel)level == ProfilingLevel.Slow)
            {
                rst = db.SetProfilingLevel((ProfilingLevel)level, new TimeSpan(0, 0, 0, 0, slowms));
            }
            else
            {
                rst = db.SetProfilingLevel((ProfilingLevel)level);
            }
            
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