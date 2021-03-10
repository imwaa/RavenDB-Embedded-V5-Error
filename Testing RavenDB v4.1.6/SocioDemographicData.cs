using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Testing_RavenDB_v4._1._6
{
    [Serializable]
    public class SocioDemographicData
    {
        public double Weight { get; set; }
        public IDictionary<string, string> Answers { get; set; }
    }
}
