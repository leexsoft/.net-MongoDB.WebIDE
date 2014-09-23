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
            Server = MongoContext.GetMongoObject(ID) as MongoServer;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            var list = new List<MongoTreeNode>();
            if (Server == null)
            {
                return list;
            }

            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var admindb = mongo.GetDatabase("admin");
                var doc = admindb.SendCommand(new Document().Append("serverStatus", 1));
                
                BuildTreeNode(list, Guid.Empty, doc);
                return list;
            }
        }
    }
}