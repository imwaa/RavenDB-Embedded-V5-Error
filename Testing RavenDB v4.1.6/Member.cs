using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Testing_RavenDB_v4._1._6
{
    [Serializable]
    public class Member
    {
        public Member()
        {
            Data = new Dictionary<DateTime, SocioDemographicData>();
        }

        public string Id { get; set; }

        public string HomeId { get; set; }

        public IDictionary<DateTime, SocioDemographicData> Data { get; set; }
    }
}
