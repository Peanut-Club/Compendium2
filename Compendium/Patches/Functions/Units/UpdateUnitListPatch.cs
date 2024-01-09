using Respawning.NamingRules;
using System;

namespace Compendium.Patches.Functions.Units
{
    public static class UpdateUnitListPatch
    {
        public static event Action<string, byte, UnitNamingRule> OnUnitCreated;
        public static event Action<ReferenceHub, byte> OnUnitAssigned;
    }
}