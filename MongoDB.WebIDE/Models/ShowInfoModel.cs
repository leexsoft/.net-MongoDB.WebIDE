namespace MongoDB.WebIDE.Models
{
    public class ShowInfoModel
    {
        public uint ID { get; set; }

        public uint TblFillerID { get; set; }

        public uint IdxFillerID { get; set; }

        public string Title { get; set; }

        public int Type { get; set; }

        public string JsonData { get; set; }
    }
}