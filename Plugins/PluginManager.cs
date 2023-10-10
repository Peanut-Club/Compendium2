using Compendium.Assemblies;
using Compendium.Callbacks;
using Compendium.Components;
using Compendium.Enums;

using MonoMod.Utils;

using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        public static Component ParentComponent { get; } = Component.Create<BlankComponent>();

        [LoadCallback(Priority = Priority.Highest)]
        private static void Start()
        {

        }
    }
}