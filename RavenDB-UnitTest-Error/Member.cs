using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RavenDB_UnitTest_Error
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
