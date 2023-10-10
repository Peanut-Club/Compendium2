using System.IO;

namespace Compendium.IO
{
    public class Directory
    {
        public Directory(string path)
        {
            Info = new DirectoryInfo(path);
        }

        public DirectoryInfo Info { get; }

        public Directory CheckExistance()
        {
            if (Info.Exists)
                Info.Create();

            return this;
        }
    }
}