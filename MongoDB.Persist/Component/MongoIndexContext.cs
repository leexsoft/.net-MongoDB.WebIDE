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

            var idxNode = MongoContext.GetTreeNode(ID);
            var tbNode = MongoContext.GetTreeNode(idxNode.PID);
            var dbNode = MongoContext.GetTreeNode(tbNode.PID);
            var serverNode = MongoContext.GetTreeNode(dbNode.PID);

            Server = MongoContext.GetMongoObject(serverNode.ID) as MongoServer;
            Database = MongoContext.GetMongoObject(dbNode.ID) as MongoDatabase;
            Table = MongoContext.GetMongoObject(tbNode.ID) as MongoCollection;
        }

        /// <summary>
        /// 获取字段节点信息
        /// </summary>
        /// <returns></returns>
        public List<MongoTreeNode> GetFieldNodes()
        {
            var fieldFiller = MongoContext.GetTreeNodes().Where(n => n.PID == Table.ID && n.Type == MongoTreeNodeType.FieldFiller).First();
            return MongoContext.GetTreeNodes().Where(n => n.PID == fieldFiller.ID).ToList();
        }

        /// <summary>
        /// 获取索引信息
        /// </summary>
        /// <returns></returns>
        public List<MongoIndex> GetIndexes()
        {
            var list = MongoContext.GetTreeNodes().Where(node => node.PID == ID).ToList();

            var indexes = new List<MongoIndex>();
            foreach (var item in list)
            {
                indexes.Add(MongoContext.GetMongoObject(item.ID) as MongoIndex);
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
    }
}