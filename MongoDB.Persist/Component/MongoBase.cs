using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Bson;

namespace MongoDB.Component
{
    public class MongoBase
    {
        protected readonly string ConnString = "Server={0}";

        protected uint ID { get; set; }
        public MongoServerModel Server { get; set; }
        public MongoDatabaseModel Database { get; set; }
        public MongoCollectionModel Table { get; set; }
        
        /// <summary>
        /// 根据文档对象组织树节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pid"></param>
        /// <param name="doc"></param>
        protected void BuildTreeNode(List<MongoTreeNode> list, uint pid, BsonDocument doc)
        {
            foreach (var key in doc.Names)
            {
                var node = new MongoTreeNode
                {
                    ID = MongoConst.GetRandomId(),
                    PID = pid
                };

                var value = doc[key.ToString()];
                if (value is BsonDocument)
                {
                    node.Name = key.ToString();
                    list.Add(node);
                    BuildTreeNode(list, node.ID, value as BsonDocument);
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