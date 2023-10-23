using Compendium.IO;
using Compendium.Logging;
using Compendium.Pooling.Pools;
using Compendium.Utilities;
using Compendium.Utilities.Instances;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Configuration
{
    public class ConfigurationHandler
    {
        public static ConfigurationHandler ApiHandler { get; }
        public static ConfigurationHandler GameHandler { get; }

        static ConfigurationHandler()
        {
            ApiHandler = new ConfigurationHandler(Paths.GetPath("api_config.ini", "apiConfig", PathType.Config));
            GameHandler = new ConfigurationHandler(Paths.GetPath("game_config.ini", "gameConfig", PathType.Config));
        }

        private File _file;
        private List<ConfigurationTargetInfo> _configs;

        public File File
        {
            get => _file;
            set
            {
                _file = value;
                OnFileChanged();
            }
        }

        public string Path
        {
            get => File?.Info.FullName ?? null;
            set => File = new File(value);
        }

        public ConfigurationStatus Status { get; private set; } = ConfigurationStatus.NotLoaded;

        public event Action OnSaved;
        public event Action OnLoaded;

        public ConfigurationHandler(string path, params Type[] searchTypes)
        {
            _configs = new List<ConfigurationTargetInfo>();

            Path = path;

            foreach (var type in searchTypes)
                Register(type, InstanceTracker.Get(type));
        }

        public ConfigurationHandler(string path, Assembly searchAssembly) : this(path, searchAssembly.GetTypes()) { }

        public void Register()
            => Register(Assembly.GetCallingAssembly());

        public void Register(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                Register(type, InstanceTracker.Get(type));
        }

        public void Register(Type type, object handle)
        {
            var fields = type.GetAllFields();
            var props = type.GetAllProperties();

            foreach (var field in fields)
            {
                if (!field.HasAttribute<ConfigurationAttribute>(out var configurationAttribute))
                    continue;

                if (!field.IsStatic && !ObjectUtilities.VerifyClassInstanceForMember(field, handle))
                {
                    Log.Error("Configuration", $"Cannot register field '{field.ToName()}': the provided class handle is not valid for this field.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(configurationAttribute.Name))
                    configurationAttribute.Name = field.Name.SnakeCase();

                if (_configs.Any(c => c.Name == configurationAttribute.Name))
                {
                    Log.Error("Configuration", $"Cannot register field '{field.ToName()}': there already is a config key with the same name.");
                    continue;
                }

                _configs.Add(new ConfigurationTargetInfo(type, field, handle, configurationAttribute.Name, configurationAttribute.Description));
            }

            foreach (var prop in props)
            {
                if (!prop.HasAttribute<ConfigurationAttribute>(out var configurationAttribute))
                    continue;

                if (prop.SetMethod is null || prop.GetMethod is null)
                {
                    Log.Error("Configuration", $"Cannot register property '{prop.ToName()}': the property's set or get method is missing.");
                    continue;
                }

                if ((!prop.SetMethod.IsStatic || !prop.GetMethod.IsStatic) && !ObjectUtilities.VerifyClassInstanceForMember(prop, handle))
                {
                    Log.Error("Configuration", $"Cannot register property '{prop.ToName()}': the provided class handle is not valid for this property.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(configurationAttribute.Name))
                    configurationAttribute.Name = prop.Name.SnakeCase();

                if (_configs.Any(c => c.Name == configurationAttribute.Name))
                {
                    Log.Error("Configuration", $"Cannot register property '{prop.ToName()}': there already is a config key with the same name.");
                    continue;
                }

                _configs.Add(new ConfigurationTargetInfo(type, prop, handle, configurationAttribute.Name, configurationAttribute.Description));
            }
        }

        public void Unregister()
            => Unregister(Assembly.GetCallingAssembly());

        public void Unregister(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                Unregister(type, InstanceTracker.Get(type));
        }

        public void Unregister(Type type, object handle)
            => _configs.RemoveAll(c => c.Target.DeclaringType == type && ObjectUtilities.IsInstance(handle, c.Handle));

        public void Unregister(string name)
            => _configs.RemoveAll(c => c.Name == name);

        public void Load()
        {
            if (Status.IsBusy())
                return;

            if (File is null)
                throw new InvalidOperationException($"You need to set the config file's path before loading it.");

            Status = ConfigurationStatus.Loading;

            if (!File.Info.Exists)
                Save();
            else
            {
                var lines = File.ReadLines(Path);

                if (lines.Length <= 0)
                    Save();
                else
                {
                    var configDict = ConfigurationReader.Read(lines, str => _configs.Any(c => c.Name == str));
                    var missingKeys = ListPool<string>.Shared.Rent();
                    var isCorrupted = false;

                    if (configDict.Count > 0)
                    {
                        if (_configs.Count > 0)
                        {
                            foreach (var cfgData in configDict)
                            {
                                var targetStatus = ConfigurationTargetStatus.None;

                                for (int i = 0; i < _configs.Count; i++)
                                {
                                    if (_configs[i].Name == cfgData.Key)
                                    {
                                        _configs[i].Status = ConfigurationTargetStatus.None;

                                        try
                                        {
                                            _configs[i].Value = ConfigurationSerializer.Deserialize(cfgData.Value, _configs[i].ValueType);
                                            _configs[i].Status = targetStatus = ConfigurationTargetStatus.Ok;
                                        }
                                        catch (Exception ex)
                                        {
                                            _configs[i].Status = targetStatus = ConfigurationTargetStatus.ParsingFailed;
                                            isCorrupted = true;
                                            Log.Error("Configuration", $"Failed to parse config key '{cfgData.Key}' of file '{File.Info.Name}'", ex);
                                        }
                                    }
                                }

                                if (targetStatus is ConfigurationTargetStatus.None)
                                    missingKeys.Add(cfgData.Key);
                            }

                            if (missingKeys.Count > 0)
                            {
                                Log.Error("Configuration", $"The configuration file '{File.Info.Name}' contains the following unknown config keys ({missingKeys.Count})");

                                for (int x = 0; x < missingKeys.Count; x++)
                                    Log.Error("Configuration", missingKeys[x]);
                            }

                            if (_configs.Any(c => c.Status is ConfigurationTargetStatus.None))
                            {
                                Log.Error("Configuration", $"The configuration file '{File.Info.Name}' is missing the following config keys ({_configs.Count(c => c.Status is ConfigurationTargetStatus.None)})");

                                for (int x = 0; x < _configs.Count; x++)
                                    Log.Error("Configuration", $"{_configs[x].Name} ({_configs[x].Description})");
                            }

                            ListPool<string>.Shared.Return(missingKeys);
                            DictionaryPool<string, string>.Shared.Return(configDict);

                            if (isCorrupted || _configs.Any(c => c.Status is ConfigurationTargetStatus.ParsingFailed))
                            {
                                var newPath = $"{File.Info.Directory.FullName}/{System.IO.Path.GetFileNameWithoutExtension(File.Info.FullName)}_corrupted.{File.Info.Extension}";

                                Log.Error("Configuration", $"The configuration file '{File.Info.Name}' is corrupted, moving the corrupted file to '{System.IO.Path.GetFileName(newPath)}'");

                                System.IO.File.Move(File.Info.FullName, newPath);

                                File.CreateIfMissing();
                            }

                            for (int x = 0; x < _configs.Count; x++)
                                _configs[x].Status = ConfigurationTargetStatus.None;
                        }
                        else
                            Log.Error("Configuration", $"Tried loading configuration with no registered config keys.");
                    }
                    else
                        Save();
                }
            }

            Status = ConfigurationStatus.Loaded;

            OnLoaded.SafeCall();
        }

        public void Save()
        {
            if (Status.IsBusy())
                return;

            Status = ConfigurationStatus.Saving;

            File.Write(Path, ConfigurationWriter.Write(_configs));

            Status = ConfigurationStatus.RecentlySaved;

            OnSaved.SafeCall();
        }

        public void Clear()
            => _configs.Clear();

        public void OnFileChanged()
        {
            if (Status.IsBusy())
                return;

            Load();
        }

        public void OnFileDataChanged()
        {
            if (Status is ConfigurationStatus.RecentlySaved)
                return;

            Load();
        }
    }
}