using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using MongoDB.Defination;
using System.Collections;

namespace MongoDB.Component
{
    public static class MongoCache
    {
        private static readonly string NodeKey = "Cache_Nodes";
        private static readonly string ObjKey = "Cache_Objects";

        public static bool CacheValid()
        {
            var nodes = HttpContext.Current.Cache.Get(NodeKey) as HashSet<MongoTreeNode>;
            return nodes != null;
        }

        public static HashSet<MongoTreeNode> GetTreeNodes()
        {
            var nodes = HttpContext.Current.Cache.Get(NodeKey) as HashSet<MongoTreeNode>;
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
            var hash = GetTreeNodes();
            return hash.Single(i => i.ID == guid);
        }

        public static Hashtable GetMongoObjects()
        {
            var dict = HttpContext.Current.Cache.Get(ObjKey) as Hashtable;
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
            return dict.ContainsKey(guid) ? dict[guid] : null;
        }

        public static void Clear()
        {
            HttpContext.Current.Cache.Remove(NodeKey);
            HttpContext.Current.Cache.Remove(ObjKey);
        }
    }
}
