using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class SaveIndexModel
    {
        public List<KeyModel> Keys { get; set; }

        public bool Unique { get; set; }

        public bool Background { get; set; }

        public bool Dropdups { get; set; }
    }

    public class KeyModel
    {
        public string Field { get; set; }
        public int Order { get; set; }
    }
}