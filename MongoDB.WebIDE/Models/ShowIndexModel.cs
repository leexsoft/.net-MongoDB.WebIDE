using System.Collections.Generic;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowIndexModel
    {
        public string Title { get; set; }

        public List<MongoIndex> Indexes { get; set; }
    }
}