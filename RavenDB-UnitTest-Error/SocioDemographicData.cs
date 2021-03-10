using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RavenDB_UnitTest_Error
{
    [Serializable]
    public class SocioDemographicData
    {
        public double Weight { get; set; }
        public IDictionary<string, string> Answers { get; set; }
    }
}
