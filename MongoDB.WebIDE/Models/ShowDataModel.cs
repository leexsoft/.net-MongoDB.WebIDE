using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;

namespace MongoDB.WebIDE.Models
{
    public class ShowDataModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public List<MongoTreeNode> Fields { get; set; }

        public List<BsonDocument> Data { get; set; }
    }
}