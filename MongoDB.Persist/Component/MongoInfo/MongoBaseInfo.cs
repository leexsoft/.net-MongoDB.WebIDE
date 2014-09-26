using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;

namespace MongoDB.Component
{
    public class MongoBaseInfo : MongoBase, IMongoInfo
    {
        #region 接口实现
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public virtual List<MongoTreeNode> GetInfo()
        {
            return null;
        }
        #endregion
    }
}