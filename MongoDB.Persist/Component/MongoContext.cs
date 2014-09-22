using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoContext
    {
        private static readonly string ConnString = "Server={0}:{1}";
        private static readonly string IndexTableName = "system.indexes";
        private static readonly string NodeKey = "TreeNodes_Cache";
        private static readonly string ObjKey = "MongoObjects_Cache";

        private List<MongoTreeNode> TreeNodes { get; set; }
        private Dictionary<Guid, object> MongoObjects { get; set; }

        public MongoContext()
        {
            TreeNodes = new List<MongoTreeNode>();
            MongoObjects = new Dictionary<Guid, object>();

            GetServerDetail();
        }

        private void GetServerDetail()
        {
            var xml = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/servers.config"));
            xml.Descendants("Server").ToList().ForEach(item =>
            {
                var server = new MongoServer
                {
                    ID = Guid.NewGuid(),
                    IP = item.Attribute("IP").Value,
                    Port = item.Attribute("Port").Value
                };

                TreeNodes.Add(new MongoTreeNode
                {
                    ID = server.ID,
                    PID = Guid.Empty,
                    Name = server.Name,
                    Type = MongoTreeNodeType.Server
                });

                MongoObjects.Add(server.ID, server);
            });

            Parallel.ForEach(TreeNodes, node => GetServerDB(node));
        }

        private void GetServerDB(MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var server = obj as MongoServer;
                if (server != null)
                {
                    using (var mongo = new Mongo(string.Format(ConnString, server.IP, server.Port)))
                    {
                        try
                        {
                            mongo.Connect();
                            server.IsOK = true;

                            var adminDB = mongo.GetDatabase("admin");
                            var dbDoc = adminDB.SendCommand(new Document().Append("listDatabases", 1));
                            server.TotalSize = Convert.ToInt64(dbDoc["totalSize"]);

                            var dbList = dbDoc["databases"] as List<Document>;
                            if (dbList != null)
                            {
                                dbList.ForEach(item =>
                                {
                                    var db = new MongoDatabase
                                    {
                                        ID = Guid.NewGuid(),
                                        Name = item["name"].ToString(),
                                        Size = Convert.ToInt64(item["sizeOnDisk"])
                                    };

                                    TreeNodes.Add(new MongoTreeNode
                                    {
                                        ID = db.ID,
                                        PID = server.ID,
                                        Name = db.Name,
                                        Type = MongoTreeNodeType.Database
                                    });

                                    MongoObjects.Add(db.ID, db);
                                });

                                var dbs = TreeNodes.Where(n => n.PID == server.ID).ToList();
                                Parallel.ForEach(dbs, db => GetDBCollection(mongo, db));
                            }
                        }
                        catch (Exception ex)
                        {
                            server.IsOK = false;
                        }
                    }
                }
            }
        }

        private void GetDBCollection(Mongo mongo, MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var database = obj as MongoDatabase;
                if (database != null)
                {
                    var db = mongo.GetDatabase(database.Name);
                    var collections = db.GetCollectionNames();
                    if (collections != null)
                    {
                        collections.Where(t => !t.Contains("$") && !t.Contains(IndexTableName)).ToList().ForEach(t =>
                        {
                            var tbl = new MongoCollection
                            {
                                ID = Guid.NewGuid(),
                                Namespace = t,
                                Name = t.Substring(t.IndexOf(".")).TrimStart('.')
                            };

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = tbl.ID,
                                PID = database.ID,
                                Name = tbl.Name,
                                Type = MongoTreeNodeType.Collection
                            });

                            MongoObjects.Add(tbl.ID, tbl);
                        });

                        var tbls = TreeNodes.Where(n => n.PID == database.ID).ToList();
                        Parallel.ForEach(tbls, table => GetCollectionInfo(db, table));
                    }
                }
            }
        }

        private void GetCollectionInfo(Database db, MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var table = obj as MongoCollection;
                if (table != null)
                {
                    //字段类型信息节点
                    var fieldNode = new MongoTreeNode
                    {
                        ID = Guid.NewGuid(),
                        PID = table.ID,
                        Name = "字段",
                        Type = MongoTreeNodeType.FieldFiller
                    };
                    TreeNodes.Add(fieldNode);

                    //字段节点
                    var doc = db[table.Name].FindOne(new Document());
                    if (doc != null)
                    {
                        foreach (var item in doc.Keys)
                        {
                            var field = new MongoField
                            {
                                ID = Guid.NewGuid(),
                                Name = item.ToString()
                            };

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = field.ID,
                                PID = fieldNode.ID,
                                Name = field.Name,
                                Type = MongoTreeNodeType.Field
                            });

                            MongoObjects.Add(field.ID, field);
                        }
                    }

                    //索引类型信息节点
                    var indexNode = new MongoTreeNode
                    {
                        ID = Guid.NewGuid(),
                        PID = table.ID,
                        Name = "索引",
                        Type = MongoTreeNodeType.IndexFiller
                    };
                    TreeNodes.Add(indexNode);

                    //索引节点
                    var indexes = db[IndexTableName].Find(new Document().Append("ns", table.Namespace));
                    if (indexes != null)
                    {
                        foreach (var idx in indexes.Documents)
                        {
                            var index = new MongoIndex
                            {
                                ID = Guid.NewGuid(),
                                Name = idx["name"].ToString(),
                                Namespace = idx["ns"].ToString(),
                                Unique = Convert.ToBoolean(idx["unique"]),
                                Keys = new List<MongoIndexKey>()
                            };
                            var docFields = idx["key"] as Document;
                            foreach (var key in docFields.Keys)
                            {
                                index.Keys.Add(new MongoIndexKey
                                {
                                    FieldName = key.ToString(),
                                    OrderType = int.Parse(docFields[key.ToString()].ToString()) == 1 ? MongoIndexOrderType.Ascending : MongoIndexOrderType.Descending
                                });
                            }

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = index.ID,
                                PID = indexNode.ID,
                                Name = index.Name,
                                Type = MongoTreeNodeType.Index
                            });

                            MongoObjects.Add(index.ID, index);
                        }
                    }
                }
            }
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
    }
}
