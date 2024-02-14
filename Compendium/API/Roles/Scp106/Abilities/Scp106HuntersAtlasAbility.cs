using Compendium.API.Roles.Abilities;

using MapGeneration;

using UnityEngine;

namespace Compendium.API.Roles.Scp106.Abilities
{
    public class Scp106HuntersAtlasAbility : VigorAbility<PlayerRoles.PlayableScps.Scp106.Scp106HuntersAtlasAbility>
    {
        public Scp106HuntersAtlasAbility(Player player, PlayerRoles.PlayableScps.Scp106.Scp106HuntersAtlasAbility ability) : base(player, ability)
        {
            VigorMin = 0.25f;
            VigorPerMeter = 0.019f;
        }

        public float VigorMin { get; set; }
        public float VigorPerMeter { get; set; }

        public bool IsSubmerged
        {
            get => Base._submerged;
            set => Base.SetSubmerged(value);
        }

        public RoomIdentifier TargetRoom
        {
            get => Base._syncRoom;
        }

        public Vector3 TargetPosition
        {
            get => Base._syncPos;
        }
    }
}