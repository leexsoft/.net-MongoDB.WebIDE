using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoServerInfo : MongoBaseInfo
    {
        public MongoServerInfo(string id)
        {
            ID = Guid.Parse(id);
            Server = MongoCache.GetMongoObject(ID) as MongoServerModel;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var adminDB = server.GetDatabase(MongoConst.AdminDBName);
            var rst = adminDB.RunCommand(new CommandDocument { { "serverStatus", 1 } });

            var list = new List<MongoTreeNode>();
            if (rst.Ok)
            {
                BuildTreeNode(list, Guid.Empty, rst.Response);
            }
            return list;
        }
    }
}