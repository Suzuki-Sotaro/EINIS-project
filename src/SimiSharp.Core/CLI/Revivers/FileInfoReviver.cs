using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PowerArgs;

namespace SimiSharp.Core.CLI.Revivers
{
    public class FileInfoReviver
    {
        [ArgReviver]
        public static FileInfo Revive(string key, string val)
        {
            return new FileInfo(fileName: val);
        }
    }
}
