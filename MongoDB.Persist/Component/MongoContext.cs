using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using System.Collections;
using log4net;
using System.Diagnostics;

namespace MongoDB.Component
{
    public class MongoContext
    {
        public HashSet<MongoTreeNode> TreeNodes { get; set; }
        public Hashtable MongoObjects { get; set; }

        public MongoContext()
        {
            TreeNodes = new HashSet<MongoTreeNode>();
            MongoObjects = Hashtable.Synchronized(new Hashtable());

            GetServer();
        }

        private void GetServer()
        {
            var serverNodes = new HashSet<Guid>();
            var xml = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/servers.config"));
            xml.Descendants("Server").ToList().ForEach(item =>
            {
                var serverModel = new MongoServerModel
                {
                    ID = Guid.NewGuid(),
                    IP = item.Attribute("IP").Value,
                    Port = item.Attribute("Port").Value
                };

                //用来做并行计算的临时集合
                serverNodes.Add(serverModel.ID);

                //树节点
                TreeNodes.Add(new MongoTreeNode
                {
                    ID = serverModel.ID,
                    PID = Guid.Empty,
                    Name = serverModel.Name,
                    Type = MongoTreeNodeType.Server
                });

                //对象节点
                MongoObjects.Add(serverModel.ID, serverModel);
            });

            Parallel.ForEach(serverNodes, id => GetDB(id));
        }

        private void GetDB(Guid guid)
        {
            if (MongoObjects.ContainsKey(guid))
            {
                var serverModel = MongoObjects[guid] as MongoServerModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    var mongo = new MongoClient(string.Format(MongoConst.ConnString, serverModel.Name));
                    var server = mongo.GetServer();
                    var adminDB = server.GetDatabase(MongoConst.AdminDBName);
                    var dbDoc = adminDB.SendCommand(MongoDocument.CreateQuery("listDatabases", 1));

                    serverModel.IsOK = true;
                    serverModel.TotalSize = Convert.ToInt64(dbDoc["totalSize"]);

                    var dbNodes = new HashSet<Guid>();
                    var dbList = dbDoc["databases"].AsBsonArray;
                    if (dbList != null)
                    {
                        dbList.AsParallel().ForAll(item =>
                        {
                            var db = new MongoDatabaseModel
                            {
                                ID = Guid.NewGuid(),
                                Name = item["name"].AsString,
                                Size = Convert.ToInt64(item["sizeOnDisk"])
                            };

                            dbNodes.Add(db.ID);

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = db.ID,
                                PID = serverModel.ID,
                                Name = db.Name,
                                Type = MongoTreeNodeType.Database
                            });

                            MongoObjects.Add(db.ID, db);
                        });

                        Parallel.ForEach(dbNodes, id => GetCollection(server, id));
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger("MongoContext").Error("获取数据库信息时出错", ex);
                    serverModel.IsOK = false;
                }

                LogManager.GetLogger("InfoLog").Info(string.Format("获取服务器{0}的所有数据库对象共花费了大约{1}毫秒的时间", serverModel.Name, watch.ElapsedMilliseconds));
                watch.Stop();
            }
        }

        private void GetCollection(MongoServer server, Guid guid)
        {
            if (MongoObjects.ContainsKey(guid))
            {
                var database = MongoObjects[guid] as MongoDatabaseModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    var db = server.GetDatabase(database.Name);
                    var collections = db.GetCollectionNames();
                    if (collections != null)
                    {
                        var tblNodes = new HashSet<Guid>();
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

                            tblNodes.Add(tbl.ID);

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = tbl.ID,
                                PID = database.ID,
                                Name = string.Format("{0} ({1})", tbl.Name, tbl.TotalCount),
                                Type = MongoTreeNodeType.Collection
                            });

                            MongoObjects.Add(tbl.ID, tbl);
                        });

                        Parallel.ForEach(tblNodes, id => GetFieldAndIndex(db, id));
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger("MongoContext").Error("获取数据表信息时出错", ex);
                }

                LogManager.GetLogger("InfoLog").Info(string.Format("获取服务库{0}的所有表对象共花费了大约{1}毫秒的时间", database.Name, watch.ElapsedMilliseconds));
                watch.Stop();
            }
        }

        private void GetFieldAndIndex(MongoDatabase db, Guid guid)
        {
            if (MongoObjects.ContainsKey(guid))
            {
                var table = MongoObjects[guid] as MongoCollectionModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    #region 字段
                    //字段类型信息节点
                    var fieldNode = new MongoTreeNode
                    {
                        ID = Guid.NewGuid(),
                        PID = table.ID,
                        Name = "表信息",
                        Type = MongoTreeNodeType.TableFiller
                    };
                    TreeNodes.Add(fieldNode);

                    //字段节点
                    var doc = db.GetCollection(table.Name).FindOne();
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
                    #endregion

                    #region 索引
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
                    var indexes = db.GetCollection(MongoConst.IndexTableName).Find(MongoDocument.CreateQuery("ns", table.Namespace));
                    if (indexes != null)
                    {
                        foreach (var idx in indexes.ToList())
                        {
                            var index = new MongoIndexModel
                            {
                                ID = Guid.NewGuid(),
                                Name = idx["name"].AsString,
                                Namespace = idx["ns"].AsString,
                                Unique = idx.Contains("unique") ? (idx["unique"].AsDouble == 1.0 ? true : false) : false,
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
                    #endregion
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger("MongoContext").Error("获取字段和索引信息时出错", ex);
                }

                LogManager.GetLogger("InfoLog").Info(string.Format("获取服务表{0}的所有字段和索引对象共花费了大约{1}毫秒的时间", table.Name, watch.ElapsedMilliseconds));
                watch.Stop();
            }
        }
    }
}
