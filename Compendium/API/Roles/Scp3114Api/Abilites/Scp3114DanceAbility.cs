using Common.Utilities;

using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp3114;

using UnityEngine;

namespace Compendium.API.Roles.Scp3114Api.Abilites
{
    public class Scp3114DanceAbility : AbilityWrapper<Scp3114Dance>
    {
        public Scp3114DanceAbility(Player player, Scp3114Dance ability) : base(player, ability) { }

        public bool IsDancing
        {
            get => Base.IsDancing;
            set
            {
                Base.IsDancing = value;

                if (value && Variant < 0)
                    Variant = Generator.Instance.GetInt32(0, 255) % Base._danceVariants;

                if (value)
                    Base._serverStartPos = new RelativePositioning.RelativePosition(Player.Position);

                Base.ServerSendRpc(true);
            }
        }

        public int Variant { get; set; } = -1;

        public Vector3 Position
        {
            get => Base._serverStartPos.Position;
            set => Base._serverStartPos = new RelativePositioning.RelativePosition(value);
        }

        public void Start(int variant = -1)
        {
            if (variant > 0)
                Variant = variant;
            else
                Variant = -1;

            IsDancing = true;
        }

        public void Stop()
            => IsDancing = false;
    }
}