namespace Compendium.Plugins
{
    public struct PluginMetadata
    {
        public string Name;
        public string Description;
        public string Author;

        public PluginVersion Version;

        public PluginVersion MinVersion;
        public PluginVersion MaxVersion;
    }
}