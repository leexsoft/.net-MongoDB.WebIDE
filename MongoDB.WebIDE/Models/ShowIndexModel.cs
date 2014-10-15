using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowIndexModel
    {
        public uint ID { get; set; }

        public uint TblID { get; set; }

        public string Title { get; set; }

        public List<MongoTreeNode> Fields { get; set; }

        public List<MongoIndexModel> Indexes { get; set; }
    }
}