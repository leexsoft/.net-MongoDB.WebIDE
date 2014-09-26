using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowProfileModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public int Status { get; set; }
    }
}