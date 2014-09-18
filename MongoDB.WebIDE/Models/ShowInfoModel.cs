using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.WebIDE.Models
{
    public class ShowInfoModel
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }
}