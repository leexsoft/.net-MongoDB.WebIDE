using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Driver.Builders;
using System.Collections;

namespace MongoDB.Component
{
    public class MongoReplicationContext : MongoBase
    {
        public MongoReplicationContext(string id)
        {
            var guid = Guid.Parse(id);

            var serverNode = MongoCache.GetTreeNode(guid);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        public Hashtable GetReplicationInfo()
        {
            var hash = new Hashtable();

            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var adminDB = server.GetDatabase(MongoConst.AdminDBName);
            var stats = adminDB.RunCommand(new CommandDocument { { "ismaster", 1 } });

            var serverInfo = new List<MongoTreeNode>();
            var dataInfo = new List<MongoTreeNode>();
            if (stats.Ok)
            {
                BuildTreeNode(serverInfo, Guid.Empty, stats.Response);
                hash.Add(0, serverInfo);

                var localDB = server.GetDatabase(MongoConst.LocalDBName);
                if (stats.Response["ismaster"].AsBoolean)
                {
                    #region 主服务器信息
                    var docs = localDB.GetCollection(MongoConst.OplogTableName).FindAll().SetLimit(10);
                    foreach (var doc in docs)
                    {
                        BuildTreeNode(dataInfo, Guid.Empty, doc);
                    }
                    #endregion
                }
                else
                {
                    #region 从服务器信息
                    var doc = localDB.GetCollection(MongoConst.SourcesTableName).FindOne();
                    if (doc["syncTo"] != null)
                    {
                        var logTime = doc["syncTo"].ToLocalTime();
                        var now = DateTime.Now;
                        double v = (now - logTime).TotalSeconds;
                        doc["syncTo"] = doc["syncTo"] + "  =" + v + "s ago";
                    }
                    BuildTreeNode(dataInfo, Guid.Empty, doc);
                    #endregion
                }
                hash.Add(1, dataInfo);
            }
            return hash;
        }
    }
}