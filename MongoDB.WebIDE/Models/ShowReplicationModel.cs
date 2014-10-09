using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowReplicationModel
    {
        public uint ID { get; set; }

        public string Title { get; set; }

        public string JsonServer { get; set; }

        public string JsonData { get; set; }
    }
}