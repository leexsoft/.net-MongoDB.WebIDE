using System;
using System.Collections.Generic;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoBase
    {
        protected string ConnString = "Server={0}";

        protected MongoServer Server { get; set; }
        protected MongoDatabase Database { get; set; }
        protected MongoCollection Table { get; set; }
    }
}