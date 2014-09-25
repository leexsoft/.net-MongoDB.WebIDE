using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Component
{
    /// <summary>
    /// 生成各类文档
    /// </summary>
    internal static class MongoDocument
    {
        public static QueryDocument CreateQuery(string command, BsonValue value)
        {
            var query = new QueryDocument();
            query.Add(command, value);
            return query;
        }
    }
}