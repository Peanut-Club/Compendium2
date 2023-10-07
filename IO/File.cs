using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compendium.IO
{
    public class File
    {
        public File(string path)
        {
            Info = new FileInfo(path.TrimEnd('/'));
        }

        public FileInfo Info { get; }

        public void Append(params string[] lines) { }
        public void Append(IEnumerable<string> lines) { }
    }
}