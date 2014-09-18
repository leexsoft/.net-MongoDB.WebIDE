using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoDatabaseInfo : MongoBaseInfo
    {
        public MongoDatabaseInfo(string id)
        {
            var guid = Guid.Parse(id);

            var dbNode = MongoContext.GetTreeNode(guid);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(guid) as MongoDatabase;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            var list = new List<MongoTreeNode>();
            if (Server == null || Database == null)
            {
                return list;
            }

            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);
                var doc = db.SendCommand(new Document().Append("dbstats", 1));

                BuildTreeNode(list, Guid.Empty, doc);
                return list;
            }
        }

        public override int GetProfileStatus()
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