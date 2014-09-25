using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using System.Xml.Linq;
using MongoDB.Bson;

namespace MongoDB.Component
{
    public class MongoContext
    {
        public List<MongoTreeNode> TreeNodes { get; set; }
        public Dictionary<Guid, object> MongoObjects { get; set; }

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
                var serverModel = new MongoServerModel
                {
                    ID = Guid.NewGuid(),
                    IP = item.Attribute("IP").Value,
                    Port = item.Attribute("Port").Value
                };

                TreeNodes.Add(new MongoTreeNode
                {
                    ID = serverModel.ID,
                    PID = Guid.Empty,
                    Name = serverModel.Name,
                    Type = MongoTreeNodeType.Server
                });

                MongoObjects.Add(serverModel.ID, serverModel);
            });

            Parallel.ForEach(TreeNodes, node => GetServerDB(node));
        }

        private void GetServerDB(MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var serverModel = obj as MongoServerModel;
                if (serverModel != null)
                {
                    try
                    {
                        var mongo = new MongoClient(string.Format(MongoConst.ConnString, serverModel.Name));
                        var server = mongo.GetServer();
                        var adminDB = server.GetDatabase(MongoConst.AdminDBName);
                        var dbDoc = adminDB.SendCommand(MongoDocument.CreateQuery("listDatabases", 1));

                        serverModel.IsOK = true;
                        serverModel.TotalSize = Convert.ToInt64(dbDoc["totalSize"]);

                        var dbList = dbDoc["databases"].AsBsonArray;
                        if (dbList != null)
                        {
                            dbList.AsParallel().ForAll(item =>
                            {
                                var db = new MongoDatabaseModel
                                {
                                    ID = Guid.NewGuid(),
                                    Name = item["name"].ToString(),
                                    Size = Convert.ToInt64(item["sizeOnDisk"])
                                };

                                TreeNodes.Add(new MongoTreeNode
                                {
                                    ID = db.ID,
                                    PID = serverModel.ID,
                                    Name = db.Name,
                                    Type = MongoTreeNodeType.Database
                                });

                                MongoObjects.Add(db.ID, db);
                            });

                            var dbs = TreeNodes.Where(n => n.PID == serverModel.ID).ToList();
                            GetDBCollection(server, dbs[0]);
                            Parallel.ForEach(dbs, db => GetDBCollection(server, db));
                        }
                    }
                    catch (Exception ex)
                    {
                        serverModel.IsOK = false;
                    }
                }
            }
        }


        private void GetDBCollection(MongoServer server, MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var database = obj as MongoDatabaseModel;
                if (database != null)
                {
                    var db = server.GetDatabase(database.Name);
                    var collections = db.GetCollectionNames();
                    if (collections != null)
                    {
                        collections.Where(t => !t.Contains("$") && !t.Contains(MongoConst.IndexTableName)).ToList().ForEach(t =>
                        {
                            var table = db.GetCollection(t);
                            var tbl = new MongoCollectionModel
                            {
                                ID = Guid.NewGuid(),
                                Name = t,
                                Namespace = table.FullName,
                                TotalCount = table.Count()
                            };

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = tbl.ID,
                                PID = database.ID,
                                Name = string.Format("{0} ({1})", tbl.Name, tbl.TotalCount),
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


        private void GetCollectionInfo(MongoDatabase db, MongoTreeNode node)
        {
            object obj;
            if (MongoObjects.TryGetValue(node.ID, out obj))
            {
                var table = obj as MongoCollectionModel;
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
                    var doc = db[table.Name].FindOne();
                    if (doc != null)
                    {
                        foreach (var item in doc.Names)
                        {
                            var field = new MongoFieldModel
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
                    var indexes = db[MongoConst.IndexTableName].Find(MongoDocument.CreateQuery("ns", table.Namespace));
                    if (indexes != null)
                    {
                        foreach (var idx in indexes.ToList())
                        {
                            var index = new MongoIndexModel
                            {
                                ID = Guid.NewGuid(),
                                Name = idx["name"].ToString(),
                                Namespace = idx["ns"].ToString(),
                                Unique = Convert.ToBoolean(idx["unique"]),
                                Keys = new List<MongoIndexKey>()
                            };
                            var docFields = idx["key"].AsBsonDocument;
                            foreach (var key in docFields.Names)
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
    }
}
