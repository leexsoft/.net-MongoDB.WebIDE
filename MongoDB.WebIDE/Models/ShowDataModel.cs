using MongoDB.Model;
using System.Collections.Generic;

namespace MongoDB.WebIDE.Models
{
    public class ShowDataModel
    {
        public uint ID { get; set; }

        public uint TblID { get; set; }

        public string Title { get; set; }

        public List<MongoFieldModel> Fields { get; set; }
    }
}