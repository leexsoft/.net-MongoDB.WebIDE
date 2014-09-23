using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowInfoModel
    {
        public string Title { get; set; }

        public int Type { get; set; }

        public string JsonData { get; set; }
    }
}