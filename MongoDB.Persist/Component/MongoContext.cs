using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

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
            var watch = new Stopwatch();
            watch.Start();

            var serverNodes = new HashSet<uint>();
            var xml = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/servers.config"));
            xml.Descendants("Server").ToList().ForEach(item =>
            {
                var serverModel = new MongoServerModel
                {
                    ID = MongoConst.GetRandomId(),
                    IP = item.Attribute("IP").Value,
                    Port = item.Attribute("Port").Value
                };

                //用来做并行计算的临时集合
                serverNodes.Add(serverModel.ID);

                //树节点
                TreeNodes.Add(new MongoTreeNode
                {
                    ID = serverModel.ID,
                    PID = 0,
                    Name = serverModel.Name,
                    Type = MongoTreeNodeType.Server
                });

                //对象节点
                MongoObjects.Add(serverModel.ID, serverModel);
            });

            //并行
            Parallel.ForEach(serverNodes, id => GetDB(id));
            //循环
            //serverNodes.ToList().ForEach(id => GetDB(id));

            LogManager.GetLogger("InfoLog").Info(string.Format("**获取所有对象共花费了大约{0}毫秒的时间", watch.ElapsedMilliseconds));
            watch.Stop();
        }

        private void GetDB(uint serverid)
        {
            if (MongoObjects.ContainsKey(serverid))
            {
                var serverModel = MongoObjects[serverid] as MongoServerModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    var mongo = new MongoClient(string.Format(MongoConst.ConnString, serverModel.Name));
                    var server = mongo.GetServer();
                    var adminDB = server.GetDatabase(MongoConst.AdminDBName);
                    var rst = adminDB.RunCommand(new CommandDocument { { "listDatabases", 1 } });

                    if (rst.Ok)
                    {
                        var dbDoc = rst.Response;
                        serverModel.IsOK = true;
                        serverModel.TotalSize = dbDoc["totalSize"].AsDouble;

                        var dbNodes = new HashSet<uint>();
                        var dbList = dbDoc["databases"].AsBsonArray;
                        if (dbList != null)
                        {
                            dbList.ToList().ForEach(item =>
                            {
                                var db = new MongoDatabaseModel
                                {
                                    ID = MongoConst.GetRandomId(),
                                    Name = item["name"].AsString,
                                    Size = item["sizeOnDisk"].AsDouble
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

                            //并行
                            Parallel.ForEach(dbNodes, id => GetCollection(server, id));
                            //循环
                            //dbNodes.ToList().ForEach(id => GetCollection(server, id));
                        }
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

        private void GetCollection(MongoServer server, uint dbid)
        {
            if (MongoObjects.ContainsKey(dbid))
            {
                var database = MongoObjects[dbid] as MongoDatabaseModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    var db = server.GetDatabase(database.Name);
                    var collections = db.GetCollectionNames();
                    if (collections != null)
                    {
                        var tblNodes = new HashSet<uint>();
                        collections.Where(t => !t.Contains("$") && !t.Contains(MongoConst.IndexTableName) && !t.Contains(MongoConst.ProfileTableName)).ToList().ForEach(t =>
                        {
                            var table = db.GetCollection(t);
                            var tbl = new MongoCollectionModel
                            {
                                ID = MongoConst.GetRandomId(),
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

                        //并行
                        Parallel.ForEach(tblNodes, id => GetFieldAndIndex(db, id));
                        //循环
                        //tblNodes.ToList().ForEach(id => GetFieldAndIndex(db, id));
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

        private void GetFieldAndIndex(MongoDatabase db, uint tblid)
        {
            if (MongoObjects.ContainsKey(tblid))
            {
                var table = MongoObjects[tblid] as MongoCollectionModel;
                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    #region 字段
                    //字段类型信息节点
                    var fieldNode = new MongoTreeNode
                    {
                        ID = MongoConst.GetRandomId(),
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
                                ID = MongoConst.GetRandomId(),
                                Name = item.ToString(),
                                Type = doc[item].BsonType
                            };

                            TreeNodes.Add(new MongoTreeNode
                            {
                                ID = field.ID,
                                PID = fieldNode.ID,
                                Name = field.Name + string.Format(" ({0})", field.Type),
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
                        ID = MongoConst.GetRandomId(),
                        PID = table.ID,
                        Name = "索引",
                        Type = MongoTreeNodeType.IndexFiller
                    };
                    TreeNodes.Add(indexNode);

                    //索引节点
                    var indexes = db.GetCollection(MongoConst.IndexTableName).Find(new QueryDocument { { "ns", table.Namespace } });
                    if (indexes != null)
                    {
                        foreach (var idx in indexes.ToList())
                        {
                            var index = new MongoIndexModel
                            {
                                ID = MongoConst.GetRandomId(),
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
