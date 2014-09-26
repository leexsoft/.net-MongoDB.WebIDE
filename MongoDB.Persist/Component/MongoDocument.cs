using System.Collections;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

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

        public static QueryDocument CreateQuery(string json)
        {
            var hash = JsonConvert.DeserializeObject<Hashtable>(json);

            var query = new QueryDocument();
            foreach (string key in hash.Keys)
            {
                query.Add(key, BsonValue.Create(hash[key]));
            }
            return query;
        }
    }
}