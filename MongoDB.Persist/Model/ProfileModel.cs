using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Model
{
    /// <summary>
    /// Profile信息
    /// </summary>
    public class ProfileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Client { get; set; }

        public string Op { get; set; }

        public string Namespace { get; set; } 

        public string Command { get; set; }       

        public DateTime Timestamp { get; set; }

        public double Duration { get; set; }

        public int NumberToReturn { get; set; }

        public int NumberScanned { get; set; }

        public int NumberReturned { get; set; }

        public int ResponseLength { get; set; }
    }
}