using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Defination;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoServerContext
    {
        private XDocument XML { get; set; }
        private string ConfigFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/servers.config"); }
        }

        public MongoServerContext()
        {
            XML = XDocument.Load(ConfigFile);
        }

        /// <summary>
        /// 获取服务器列表
        /// </summary>
        /// <returns></returns>
        public List<MongoServerModel> GetServerNodes()
        {
            var list = new List<MongoServerModel>();
            XML.Descendants("Server").ToList().ForEach(item =>
            {
                list.Add(new MongoServerModel
                {
                    ID = MongoConst.GetRandomId(),
                    IP = item.Attribute("IP").Value,
                    Port = item.Attribute("Port").Value
                });
            });
            return list;
        }

        public void AddServer(string ip, int port)
        {
            // 添加需要新增的XElement对象
            var element = new XElement("Server", new XAttribute("IP", ip), new XAttribute("Port", port));
            XML.Root.Add(element);
            XML.Save(ConfigFile);

            MongoCache.Clear();
        }

        public void DeleteServer(string ip, int port)
        {
            // 添加需要新增的XElement对象
            var server = XML.Descendants("Server").ToList().Find(p => p.Attribute("IP").Value == ip && p.Attribute("Port").Value == port.ToString());
            if (server != null)
            {
                server.Remove();
                XML.Save(ConfigFile);

                MongoCache.Clear();
            }
        }
    }
}