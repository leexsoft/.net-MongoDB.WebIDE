using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;

namespace MongoDB.WebIDE.Models
{
    public class ShowDataModel
    {
        public string Title { get; set; }

        public List<MongoTreeNode> Fields { get; set; }

        public List<Document> Data { get; set; }
    }
}