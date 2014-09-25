using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using Newtonsoft.Json;

namespace MongoDB.Component
{
    public class MongoIndexContext : MongoBase
    {
        public MongoIndexContext(string id)
        {
            ID = Guid.Parse(id);
            var idxNode = MongoCache.GetTreeNode(ID);

            var tbNode = MongoCache.GetTreeNode(idxNode.PID);
            Table = MongoCache.GetMongoObject(tbNode.ID) as MongoCollection;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabase;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServer;
        }

        /// <summary>
        /// 获取字段节点信息
        /// </summary>
        /// <returns></returns>
        public List<MongoTreeNode> GetFieldNodes()
        {
            var fieldFiller = MongoCache.GetTreeNodes().Where(n => n.PID == Table.ID && n.Type == MongoTreeNodeType.FieldFiller).First();
            return MongoCache.GetTreeNodes().Where(n => n.PID == fieldFiller.ID).ToList();
        }

        /// <summary>
        /// 获取索引信息
        /// </summary>
        /// <returns></returns>
        public List<MongoIndex> GetIndexes()
        {
            var list = MongoCache.GetTreeNodes().Where(node => node.PID == ID).ToList();

            var indexes = new List<MongoIndex>();
            foreach (var item in list)
            {
                indexes.Add(MongoCache.GetMongoObject(item.ID) as MongoIndex);
            }
            return indexes;
        }

        public void CreateIndex(string jsonData)
        {
            var model = JsonConvert.DeserializeObject<SaveIndexModel>(jsonData);
            var doc = ToDoc(model.Keys);
            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);

                var tbl = db.GetCollection(Table.Name);
                tbl.MetaData.CreateIndex(doc, model.Unique, model.Background, model.Dropdups);
                mongo.Disconnect();

                MongoCache.Clear();
            }
        }

        private Document ToDoc(List<KeyModel> keys)
        {
            Document doc = new Document();
            if (keys != null && keys.Count > 0)
            {
                foreach (var key in keys)
                {
                    doc.Append(key.Field, key.Order);
                }
            }
            return doc;
        }

        public void DeleteIndex(string name)
        {
            using (var mongo = new Mongo(string.Format(ConnString, Server.Name)))
            {
                mongo.Connect();
                var db = mongo.GetDatabase(Database.Name);

                var tbl = db.GetCollection(Table.Name);
                tbl.MetaData.DropIndex(name);
                mongo.Disconnect();

                MongoCache.Clear();
            }
        }
    }
}