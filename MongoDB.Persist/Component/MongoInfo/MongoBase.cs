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
        public MongoServer Server { get; set; }
        public MongoDatabase Database { get; set; }
        public MongoCollection Table { get; set; }
    }
}