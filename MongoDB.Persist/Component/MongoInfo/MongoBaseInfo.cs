using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

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

        /// <summary>
        /// 根据文档对象组织树节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pid"></param>
        /// <param name="doc"></param>
        protected void BuildTreeNode(List<MongoTreeNode> list, Guid pid, Document doc)
        {
            foreach (var key in doc.Keys)
            {
                var node = new MongoTreeNode
                {
                    ID = Guid.NewGuid(),
                    PID = pid
                };

                var value = doc[key.ToString()];
                if (value is Document)
                {
                    node.Name = key.ToString();
                    list.Add(node);
                    BuildTreeNode(list, node.ID, value as Document);
                }
                else
                {
                    node.Name = string.Format("{0} : {1}", key, doc[key.ToString()]);
                    list.Add(node);
                }
            }
        }
    }
}