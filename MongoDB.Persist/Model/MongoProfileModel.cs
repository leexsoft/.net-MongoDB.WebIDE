using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Model
{
    /// <summary>
    /// Profile信息
    /// </summary>
    public class MongoProfileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Client { get; set; }

        public DateTime Timestamp { get; set; }

        public double Duration { get; set; }

        public string Info { get; set; }

        public string Op { get; set; }

        public int NumberToReturn { get; set; }

        public int NumberReturned { get; set; }

        public int NumberScanned { get; set; }

        public int NumberOfYields { get; set; }

        public int NumberMoved { get; set; }

        public int NumberUpdated { get; set; }
    }
}