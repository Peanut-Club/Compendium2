using Compendium.Patches.Functions.Roles;
using Compendium.Patches.Functions.Units;

using PlayerRoles;

using Respawning.NamingRules;
using Respawning;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Units
{
    public static class UnitManager
    { 
        private static readonly List<Unit> allUnits;

        static UnitManager()
        {
            allUnits = new List<Unit>();

            UpdateRolePatch.OnRoleChanged += OnRoleChanged;
            UpdateUnitListPatch.OnUnitCreated += OnUnitCreated;
            UpdateUnitListPatch.OnUnitAssigned += OnUnitAssigned;
        }

        public static IReadOnlyCollection<Unit> Units => allUnits;

        public static Unit Get(byte unitId)
            => allUnits.FirstOrDefault(un => un.Id == unitId);

        public static Unit Get(string name)
            => allUnits.FirstOrDefault(un => un.Name.ToLower() == name.ToLower());

        public static string GetNewUnitName()
        {
            if (!UnitNamingRule.TryGetNamingRule(SpawnableTeamType.NineTailedFox, out var namingRule) || namingRule is not NineTailedFoxNamingRule ntfRule)
                return string.Empty;

            int ntfCodeId;
            int ntfUnitId;

            do
            {
                ntfCodeId = UnityEngine.Random.Range(0, NineTailedFoxNamingRule.PossibleCodes.Length - 1);
                ntfUnitId = UnityEngine.Random.Range(1, 19);
            }
            while (!ntfRule._usedCombos.Add(ntfCodeId * 255 + ntfUnitId));

            return $"{NineTailedFoxNamingRule.PossibleCodes[ntfCodeId]}-{ntfUnitId}";
        }

        private static void OnUnitCreated(string name, byte id, UnitNamingRule rule)
            => allUnits.Add(new Unit(name, id, rule));

        private static void OnUnitAssigned(ReferenceHub hub, byte unitId)
        {
            var unit = Get(unitId);

            if (unit is null)
                return;

            var player = Player.Get(hub);

            if (player is null)
                return;

            if (!unit.players.Contains(player))
                unit.players.Add(player);
        }

        private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            var player = Player.Get(hub);

            if (player is null)
                return;

            if (prevRole is HumanRole humanRole && humanRole.UsesUnitNames
                && (newRole is not HumanRole newHumanRole || !newHumanRole.UsesUnitNames || newHumanRole.UnitNameId != humanRole.UnitNameId))
            {
                var unit = Get(humanRole.UnitNameId);

                if (unit != null)
                    unit.players.Remove(player);
            }
        }
    }
}