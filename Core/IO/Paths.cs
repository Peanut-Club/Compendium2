using Compendium.Configuration;
using Compendium.Utilities.Reflection;

using System;

namespace Compendium.IO
{
    public static class Paths
    {
        public static bool Initialized { get; private set; }

        public static Directory Directory { get; private set; }

        public static Directory ServerDirectory { get; private set; }
        public static Directory GlobalDirectory { get; private set; }

        public static Directory ConfigsDirectory { get; private set; }
        public static Directory PluginsDirectory { get; private set; }
        public static Directory LogsDirectory { get; private set; }
        public static Directory CacheDirectory { get; private set; }
        public static Directory DataDirectory { get; private set; }

        public static Directory GlobalConfigsDirectory { get; private set; }
        public static Directory GlobalPluginsDirectory { get; private set; }
        public static Directory GlobalLogsDirectory { get; private set; }
        public static Directory GlobalCacheDirectory { get; private set; }
        public static Directory GlobalDataDirectory { get; private set; }

        public static event Action OnInitialized;

        [Configuration]
        public static string[] GlobalPaths { get; set; } = new string[]
        {
            "*"
        };

        [Configuration]
        public static string[] ServerPaths { get; set; } = new string[]
        {
         
        };

        public static string GetPath(string name, string id, PathType type)
        {
            switch (type)
            {
                case PathType.Plugin:
                    return ((GlobalPaths.Contains("*") || GlobalPaths.Contains(id)) && !ServerPaths.Contains(id)) ? $"{GlobalPluginsDirectory.Info.FullName}/{name}" : $"{PluginsDirectory.Info.FullName}/{name}";

                case PathType.Config:
                    return ((GlobalPaths.Contains("*") || GlobalPaths.Contains(id)) && !ServerPaths.Contains(id)) ? $"{GlobalConfigsDirectory.Info.FullName}/{name}" : $"{ConfigsDirectory.Info.FullName}/{name}";

                case PathType.Cache:
                    return ((GlobalPaths.Contains("*") || GlobalPaths.Contains(id)) && !ServerPaths.Contains(id)) ? $"{GlobalCacheDirectory.Info.FullName}/{name}" : $"{CacheDirectory.Info.FullName}/{name}";

                case PathType.Data:
                    return ((GlobalPaths.Contains("*") || GlobalPaths.Contains(id)) && !ServerPaths.Contains(id)) ? $"{GlobalDataDirectory.Info.FullName}/{name}" : $"{DataDirectory.Info.FullName}/{name}";

                case PathType.Log:
                    return ((GlobalPaths.Contains("*") || GlobalPaths.Contains(id)) && !ServerPaths.Contains(id)) ? $"{GlobalLogsDirectory.Info.FullName}/{name}" : $"{LogsDirectory.Info.FullName}/{name}";

                default:
                    return $"{Directory.Info.FullName}/{name}";
            }
        }

        internal static void Load()
        {
            if (Initialized)
                throw new InvalidOperationException($"Paths have already been initialized!");

            Directory = new Directory(System.IO.Directory.GetCurrentDirectory()).CheckExistance();

            ServerDirectory = new Directory($"{Directory.Info.FullName}/server_{ServerStatic.ServerPort}").CheckExistance();
            GlobalDirectory = new Directory($"{Directory.Info.FullName}/global").CheckExistance();

            ConfigsDirectory = new Directory($"{ServerDirectory.Info.FullName}/configs").CheckExistance();
            PluginsDirectory = new Directory($"{ServerDirectory.Info.FullName}/plugins").CheckExistance();
            LogsDirectory = new Directory($"{ServerDirectory.Info.FullName}/logs").CheckExistance();
            CacheDirectory = new Directory($"{ServerDirectory.Info.FullName}/cache").CheckExistance();
            DataDirectory = new Directory($"{ServerDirectory.Info.FullName}/data").CheckExistance();

            GlobalConfigsDirectory = new Directory($"{GlobalDirectory.Info.FullName}/configs").CheckExistance();
            GlobalPluginsDirectory = new Directory($"{GlobalDirectory.Info.FullName}/plugins").CheckExistance();
            GlobalLogsDirectory = new Directory($"{GlobalDirectory.Info.FullName}/logs").CheckExistance();
            GlobalDataDirectory = new Directory($"{GlobalDirectory.Info.FullName}/data").CheckExistance();
            GlobalCacheDirectory = new Directory($"{GlobalDirectory.Info.FullName}/cache").CheckExistance();

            OnInitialized.SafeCall();
        }
    }
}
