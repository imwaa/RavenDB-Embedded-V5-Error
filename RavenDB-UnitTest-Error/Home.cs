using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RavenDB_UnitTest_Error
{
    public class Home
    {
        public Home()
        {
            Data = new Dictionary<DateTime, SocioDemographicData>();
            Members = new List<Member>();
        }

        public string Id { get; set; }

        public IDictionary<DateTime, SocioDemographicData> Data { get; set; }

        public IList<Member> Members { get; set; }
    }
}
