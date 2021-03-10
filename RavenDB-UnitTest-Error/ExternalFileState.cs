using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RavenDB_UnitTest_Error

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
