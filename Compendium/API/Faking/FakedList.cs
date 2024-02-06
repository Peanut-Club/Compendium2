using Common.IO.Collections;
using Common.Values;

using System;
using System.Collections.Generic;

namespace Compendium.API.Faking
{
    public class FakedList<TValue> 
    {
        private readonly LockedDictionary<uint, TValue> targets = new LockedDictionary<uint, TValue>();

        public Player Target { get; set; }

        public virtual bool TryGet(Player target, out TValue value)
        {
            if (target is null)
            {
                value = default;
                return false;
            }

            return targets.TryGetValue(target.NetworkId, out value);
        }

        public virtual bool GetRef(Player target, ref TValue value)
        {
            if (target is null)
                return false;

            return targets.TryGetValue(target.NetworkId, out value);
        }

        public virtual TValue Get(Player target, TValue defaultValue = default)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (targets.TryGetValue(target.NetworkId, out var fakedValue))
                return fakedValue;

            return defaultValue;
        }

        public virtual void Set(Player target, TValue value)
        {
            if (targets.ContainsKey(target.NetworkId))
            {
                var currentValue = targets[target.NetworkId];
                targets[target.NetworkId] = value;
                OnUpdated(target, new OptionalValue<TValue>(currentValue, true), value);
            }
            else
            {
                targets[target.NetworkId] = value;
                OnUpdated(target, new OptionalValue<TValue>(default, false), value);
            }
        }

        public virtual void Set(IEnumerable<Player> targets, TValue value)
        {
            foreach (var target in targets)
                Set(target, value);
        }

        public virtual void Set(Predicate<Player> predicate, TValue value)
        {
            foreach (var player in Player.List)
            {
                if (predicate(player))
                    Set(player, value);
            }
        }

        public virtual void Remove(Player target)
        {
            if (!targets.ContainsKey(target.NetworkId))
                return;

            var value = targets[target.NetworkId];

            if (!targets.Remove(target.NetworkId))
                return;

            OnRemoved(target, value);
        }

        public virtual void Remove(IEnumerable<Player> targets)
        {
            foreach (var target in targets)
                Remove(target);
        }

        public virtual void Remove(Predicate<Player> predicate)
        {
            foreach (var target in Player.List)
            {
                if (targets.ContainsKey(target.NetworkId) && predicate(target))
                    Remove(target);
            }
        }

        public virtual void Remove()
        {
            foreach (var targetId in targets.Keys)
            {
                var target = Player.Get(targetId);

                if (target != null)
                    Remove(target);
            }

            targets.Clear();
        }

        public virtual void OnUpdated(Player target, OptionalValue<TValue> previousValue, TValue currentValue) { }
        public virtual void OnRemoved(Player target, TValue value) { }
    }
}