using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowProfileModel
    {
        public uint ID { get; set; }

        public string Title { get; set; }

        public ProfilingLevel Status { get; set; }
    }
}