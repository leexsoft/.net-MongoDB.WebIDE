using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace MongoDB.Component
{
    public class MongoIndexContext : MongoBase
    {
        public MongoIndexContext(uint id)
        {
            ID = id;
            var idxNode = MongoCache.GetTreeNode(ID);

            var tbNode = MongoCache.GetTreeNode(idxNode.PID);
            Table = MongoCache.GetMongoObject(tbNode.ID) as MongoCollectionModel;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        /// <summary>
        /// 获取字段节点信息
        /// </summary>
        /// <returns></returns>
        public List<MongoTreeNode> GetFieldNodes()
        {
            var fieldFiller = MongoCache.GetTreeNodes().Where(n => n.PID == Table.ID && n.Type == MongoTreeNodeType.TableFiller).First();
            return MongoCache.GetTreeNodes().Where(n => n.PID == fieldFiller.ID).ToList();
        }

        /// <summary>
        /// 获取索引信息
        /// </summary>
        /// <returns></returns>
        public List<MongoIndexModel> GetIndexes()
        {
            var list = MongoCache.GetTreeNodes().Where(node => node.PID == ID).ToList();

            var indexes = new List<MongoIndexModel>();
            foreach (var item in list)
            {
                indexes.Add(MongoCache.GetMongoObject(item.ID) as MongoIndexModel);
            }
            return indexes;
        }

        /// <summary>
        /// 新建索引
        /// </summary>
        /// <param name="jsonData"></param>
        public void CreateIndex(string jsonData)
        {
            var model = JsonConvert.DeserializeObject<SaveIndexModel>(jsonData);
            var idxDoc = ToDoc(model.Keys);
            var idxOption = new IndexOptionsDocument();
            idxOption.Add("background", model.Background);
            idxOption.Add("dropdups", model.Dropdups);

            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var tbl = db.GetCollection(Table.Name);
            var rst = tbl.CreateIndex(idxDoc, idxOption);

            MongoCache.Clear();
        }

        private IndexKeysDocument ToDoc(List<KeyModel> keys)
        {
            var doc = new IndexKeysDocument();
            if (keys != null && keys.Count > 0)
            {
                foreach (var key in keys)
                {
                    doc.Add(key.Field, key.Order);
                }
            }
            return doc;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="name"></param>
        public void DeleteIndex(uint id)
        {
            var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var server = mongo.GetServer();
            var db = server.GetDatabase(Database.Name);

            var idx = MongoCache.GetMongoObject(id) as MongoIndexModel;
            var tbl = db.GetCollection(Table.Name);
            tbl.DropIndex(idx.Keys.Select(k => k.FieldName).ToArray());

            MongoCache.Clear();
        }
    }
}