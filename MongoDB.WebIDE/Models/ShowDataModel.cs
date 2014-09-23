using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;

namespace MongoDB.WebIDE.Models
{
    public class ShowDataModel
    {
        public List<MongoTreeNode> Fields { get; set; }
        public List<Document> Data { get; set; }
    }
}