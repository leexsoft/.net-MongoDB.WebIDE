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
        public MongoReplicationContext(uint id)
        {
            var serverNode = MongoCache.GetTreeNode(id);
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
                BuildTreeNode(serverInfo, 0, stats.Response);
                hash.Add(0, serverInfo);

                var localDB = server.GetDatabase(MongoConst.LocalDBName);
                if (stats.Response["ismaster"].AsBoolean)
                {
                    #region 日志信息
                    var docs = localDB.GetCollection(MongoConst.OplogTableName).FindAll().SetLimit(10);
                    var doc = new BsonDocument();
                    var idx = 0;
                    foreach (var d in docs)
                    {
                        idx++;
                        doc.Add("日志 No." + idx, d);
                    }
                    BuildTreeNode(dataInfo, 0, doc);
                    #endregion
                }
                else
                {
                    #region 源服务器信息
                    var doc = localDB.GetCollection(MongoConst.SourceTableName).FindOne();
                    BuildTreeNode(dataInfo, 0, doc);
                    #endregion
                }
                hash.Add(1, dataInfo);
            }
            return hash;
        }
    }
}