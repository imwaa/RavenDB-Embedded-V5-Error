using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Testing_RavenDB_v4._1._6

{
    public class ExternalFileState
    {
        public string Filename { get; set; }

        public static ExternalFileState GetFromFilename(string filename)
        {
            return new ExternalFileState() { Filename = Path.GetFileName(filename) };
        }
    }
}
