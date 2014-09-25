using MongoDB.Bson;

namespace MongoDB.Driver
{
    /// <summary>
    /// 索引存储的排序方式
    /// </summary>
    public static class MongoExtension
    {
        public static BsonDocument SendCommand(this MongoDatabase db, QueryDocument command)
        {
            var cmd = db.GetCollection("$cmd");
            var rst = cmd.FindOne(command);

            var num = rst["ok"].AsDouble;
            if (num == 1.0)
            {
                return rst;
            }

            var errMsg = string.Empty;
            if (rst.Contains("msg"))
            {
                errMsg = rst["msg"].AsString;
            }
            else if (rst.Contains("errmsg"))
            {
                errMsg = rst["errmsg"].AsString;
            }
            throw new MongoException(errMsg);
        }
    }
}