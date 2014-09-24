using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using MongoDB.Defination;

namespace MongoDB.Component
{
    public static class MongoCache
    {
        private static readonly string NodeKey = "Cache_Nodes";
        private static readonly string ObjKey = "Cache_Objects";

        public static bool CacheValid()
        {
            var nodes = HttpContext.Current.Cache.Get(NodeKey) as List<MongoTreeNode>;
            return nodes != null;
        }

        public static List<MongoTreeNode> GetTreeNodes()
        {
            var nodes = HttpContext.Current.Cache.Get(NodeKey) as List<MongoTreeNode>;
            if (nodes == null)
            {
                var context = new MongoContext();
                nodes = context.TreeNodes;
                HttpContext.Current.Cache.Insert(NodeKey, context.TreeNodes, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
                HttpContext.Current.Cache.Insert(ObjKey, context.MongoObjects, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }
            return nodes;
        }

        public static MongoTreeNode GetTreeNode(Guid guid)
        {
            var list = GetTreeNodes();
            return list.Find(i => i.ID == guid);
        }

        public static Dictionary<Guid, object> GetMongoObjects()
        {
            var dict = HttpContext.Current.Cache.Get(ObjKey) as Dictionary<Guid, object>;
            if (dict == null)
            {
                var context = new MongoContext();
                dict = context.MongoObjects;
                HttpContext.Current.Cache.Insert(NodeKey, context.TreeNodes, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
                HttpContext.Current.Cache.Insert(ObjKey, context.MongoObjects, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }
            return dict;
        }

        public static object GetMongoObject(Guid guid)
        {
            var dict = GetMongoObjects();
            object obj;
            if (dict.TryGetValue(guid, out obj))
            {
                return obj;
            }
            return null;
        }

        public static void Clear()
        {
            HttpContext.Current.Cache.Remove(NodeKey);
            HttpContext.Current.Cache.Remove(ObjKey);
        }
    }
}
