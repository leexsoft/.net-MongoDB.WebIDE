using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoBase
    {
        protected readonly string ConnString = "Server={0}";
        
        protected Guid ID { get; set; }
        public MongoServerModel Server { get; set; }
        public MongoDatabaseModel Database { get; set; }
        public MongoCollectionModel Table { get; set; }
    }
}