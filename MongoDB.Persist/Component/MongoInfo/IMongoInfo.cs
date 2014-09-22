using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;

namespace MongoDB.Component
{
    public interface IMongoInfo
    {
        /// <summary>
        /// 获取Mongo对象统计信息
        /// </summary>
        /// <returns></returns>
        List<MongoTreeNode> GetInfo();
    }
}