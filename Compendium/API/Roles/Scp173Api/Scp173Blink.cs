using Common.Extensions;

using Compendium.API.Roles.Abilities;

using Mirror;

using PlayerRoles.PlayableScps.Scp173;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Roles.Scp173Api
{
    public class Scp173Blink : AbilityWrapper<Scp173BlinkTimer>
    {
        private Scp173ObserversTracker tracker;

        public Scp173Blink(Player player, Scp173BlinkTimer ability) : base(player, ability)
        {
            tracker = ability._observers;
        }

        public IEnumerable<Player> Observers
        {
            get => tracker.Observers.Select(Player.Get);
            set
            {
                tracker.Observers.Clear();
                
                if (value is null)
                {
                    tracker.UpdateObservers();
                    return;
                }

                tracker.Observers.AddRange(value.Select(p => p.Base));
                tracker.UpdateObservers();
            }
        }

        public float RemainingCooldown
        {
            get => Base.RemainingBlinkCooldown;
            set
            {
                Base._initialStopTime = NetworkTime.time + value;
                Base.ServerSendRpc(true);
            }
        }

        public bool IsObserved
        {
            get => tracker.IsObserved;
        }
    }
}