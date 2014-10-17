using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

namespace MongoDB.Component
{
    public class MongoProfileContext : MongoBase
    {
        public MongoProfileContext(uint id)
        {
            var dbNode = MongoCache.GetTreeNode(id);
            Database = MongoCache.GetMongoObject(id) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        public ProfilingLevel GetProfileStatus()
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var rst = db.GetProfilingLevel();
            if (string.IsNullOrEmpty(rst.ErrorMessage))
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

        public List<ProfileModel> GetProfileData(int limit)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var cursors = db.GetProfilingInfo(Query.Null).SetLimit(limit);
            var list = new List<ProfileModel>();
            foreach (var item in cursors)
            {
                list.Add(new ProfileModel
                {
                    Client = item.Client,
                    Op = item.Op,
                    Namespace = item.Namespace,
                    Command = GetCommand(item),
                    Timestamp = item.Timestamp,
                    Duration = item.Duration.TotalMilliseconds,
                    NumberToReturn = item.NumberToReturn,
                    NumberScanned = item.NumberScanned,
                    NumberReturned = item.NumberReturned,
                    ResponseLength = item.ResponseLength
                });
            }
            return list;
        }

        private string GetCommand(SystemProfileInfo info)
        {
            if (info.Command != null)
            {
                return info.Command.ToString();
            }
            if (info.Query != null)
            {
                return info.Query.ToString();
            }
            if (info.UpdateObject != null)
            {
                return info.UpdateObject.ToString();
            }
            return string.Empty;
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

            if (string.IsNullOrEmpty(rst.ErrorMessage))
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