using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowIndexModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public List<MongoTreeNode> Fields { get; set; }

        public List<MongoIndexModel> Indexes { get; set; }
    }
}