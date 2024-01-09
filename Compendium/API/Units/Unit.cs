using Respawning.NamingRules;
using Respawning;

using System.Collections.Generic;
using System.Linq;

using PlayerRoles;

namespace Compendium.API.Units
{
    public class Unit
    {
        internal List<Player> players;

        public Unit(string name, byte id, UnitNamingRule rule)
        {
            players = new List<Player>(Player.List.Where(p => p.Hub.roleManager.CurrentRole is HumanRole humanRole && humanRole.UsesUnitNames && humanRole.UnitNameId == id));

            FullName = name;
            Rule = rule;
            Id = id;

            var split = FullName.Split('-');

            Name = split[0];
            Number = int.Parse(split[1]);

            ComboNumber = NineTailedFoxNamingRule.PossibleCodes.IndexOf(Name) * 255 + Number;
            CassieName = (rule as NineTailedFoxNamingRule).GetCassieUnitName(FullName);
        }

        public string FullName { get; }
        public string CassieName { get; }
        public string Name { get; }

        public int Number { get; }
        public int ComboNumber { get; }

        public byte Id { get; }

        public bool IsUsed => (Rule as NineTailedFoxNamingRule)._usedCombos.Contains(ComboNumber);

        public UnitNamingRule Rule { get; }

        public IReadOnlyList<Player> Players => players;
    }
}