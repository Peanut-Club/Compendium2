using System;

using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

using Compendium.Logging;
using Compendium.Configuration.YamlApi;

namespace Compendium.Configuration
{
    public static class ConfigurationSerializer
    {
        public static ISerializer Serializer { get; set; } = new SerializerBuilder()
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .DisableAliases()
            .Build();

        public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        public static string Serialize(object value) 
        {
            if (value is null)
                return "null";

            try
            {
                return Serializer.Serialize(value);
            }
            catch (Exception ex)
            {
                Log.Error("Configuration Serializer", $"Failed to serialize value '{value.GetType().FullName}'", ex);
                return "null";
            }
        }

        public static object Deserialize(string value, Type type) 
        {
            if (value is "null")
                return null;

            return Deserializer.Deserialize(value, type);
        }
    }
}