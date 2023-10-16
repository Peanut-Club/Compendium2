using System;

namespace Compendium.Configuration
{
    [Flags]
    public enum ConfigurationFlags : byte
    {
        None = 0,

        Name_Lowered = 2,
        Name_SnakeCase = 4,
        Name_IncludeDeclaringType = 8,

        Description_IncludeEnumValues = 16,
        Description_IncludeValueType = 32,
    }
}