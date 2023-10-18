using System.IO;

namespace Compendium.IO
{
    public class File
    {
        public File(string path)
        {
            Info = new FileInfo(path.TrimEnd('/'));
        }

        public FileInfo Info { get; }

        public static byte[] Read(string path) => null;
    }
}