using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;

namespace MongoDB.Component
{
    public class MongoInfoFactory : IMongoInfo
    {
        private IMongoInfo MongoInfo { get; set; }

        public static MongoInfoFactory Create(uint id, int type)
        {
            var factory = new MongoInfoFactory();
            if (type == (int)MongoTreeNodeType.Server)
            {
                factory.MongoInfo = new MongoServerInfo(id);
            }
            else if (type == (int)MongoTreeNodeType.Database)
            {
                factory.MongoInfo = new MongoDatabaseInfo(id);
            }
            else if (type == (int)MongoTreeNodeType.Collection)
            {
                factory.MongoInfo = new MongoCollectionInfo(id);
            }
            return factory;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public List<MongoTreeNode> GetInfo()
        {
            return MongoInfo.GetInfo();
        }
    }
}