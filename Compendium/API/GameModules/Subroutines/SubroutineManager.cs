using Common.Extensions;
using Common.Pooling.Pools;
using Common.Values;

using Compendium.API.Modules;
using Compendium.API.Roles.Scp0492.Abilities;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Subroutines;

using System;
using System.Collections.Generic;

namespace Compendium.API.GameModules.Subroutines
{
    public class SubroutineManager : ModuleBase, IWrapper<SubroutineManagerModule>
    {
        public SubroutineManagerModule Base { get; private set; }
        public Player Player { get; private set; }

        public IReadOnlyList<ISubroutine> Subroutines { get; private set; }

        public int SubroutineCount
        {
            get => Subroutines.Count;
        }

        public override ModuleUpdate OnStart()
            => new ModuleUpdate("OnUpdate", 5, false, false);

        public TRoutine Add<TRoutine>() where TRoutine : ICustomSubroutine
        {
            if (TryGet<TRoutine>(out var customRoutine))
                return customRoutine;

            customRoutine = typeof(TRoutine).Construct<TRoutine>();

            customRoutine.Player = Player;
            customRoutine.Start();

            AppendRoutine(customRoutine);

            return customRoutine;
        }

        public bool Remove<TRoutine>() where TRoutine : ICustomSubroutine
        {
            if (!TryGet<TRoutine>(out var customRoutine))
                return false;

            customRoutine.Destroy();
            customRoutine.Player = null;

            RemoveRoutine(customRoutine);

            return true;
        }

        public TRoutine Get<TRoutine>() where TRoutine : ISubroutine
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is TRoutine tRoutine)
                    return tRoutine;
            }

            return default;
        }

        public bool TryGet<TRoutine>(out TRoutine routine) where TRoutine : ISubroutine
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is TRoutine tRoutine)
                {
                    routine = tRoutine;
                    return true;
                }
            }

            routine = default;
            return false;
        }

        internal void Initialize(SubroutineManagerModule subroutineManagerModule, Player player)
        {
            Base = subroutineManagerModule;
            Player = player;

            var subroutines = ListPool<ISubroutine>.Shared.Rent();

            for (int i = 0; i < subroutineManagerModule.AllSubroutines.Length; i++)
            {
                var subroutine = GetSubroutine(subroutineManagerModule.AllSubroutines[i]);

                if (subroutine != null)
                    subroutines.Add(subroutine);
            }

            Subroutines = subroutines.AsReadOnly();

            ListPool<ISubroutine>.Shared.Return(subroutines);
        }

        private void AppendRoutine(ISubroutine subroutine)
        {
            var routines = ListPool<ISubroutine>.Shared.Rent(Subroutines);

            routines.Add(subroutine);

            Subroutines = routines.AsReadOnly();

            ListPool<ISubroutine>.Shared.Return(routines);
        }

        private void RemoveRoutine(ISubroutine subroutine)
        {
            var routines = ListPool<ISubroutine>.Shared.Rent(Subroutines);

            routines.Remove(subroutine);

            Subroutines = routines.AsReadOnly();

            ListPool<ISubroutine>.Shared.Return(routines);
        }

        private void OnUpdate()
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is ICustomSubroutine customSubroutine)
                {
                    try
                    {
                        customSubroutine.Player ??= Player;
                        customSubroutine.Update();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"An exception has been caught while updating subroutine '{customSubroutine.GetType().Name}':\n{ex}");
                    }
                }
            }
        }

        private ISubroutine GetSubroutine(SubroutineBase subroutineBase)
        {
            switch (subroutineBase)
            {
                case PlayerRoles.PlayableScps.Scp049.Scp049AttackAbility scp049AttackAbility:
                    return new Roles.Scp049.Abilities.Scp049AttackAbility(Player, scp049AttackAbility);

                case PlayerRoles.PlayableScps.Scp049.Scp049CallAbility scp049CallAbility:
                    return new Roles.Scp049.Abilities.Scp049CallAbility(Player, scp049CallAbility);

                case PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility scp049ResurrectAbility:
                    return new Roles.Scp049.Abilities.Scp049ResurrectAbility(Player, scp049ResurrectAbility);

                case PlayerRoles.PlayableScps.Scp049.Scp049SenseAbility scp049SenseAbility:
                    return new Roles.Scp049.Abilities.Scp049SenseAbility(Player, scp049SenseAbility);

                case ZombieAttackAbility zombieAttackAbility:
                    return new Scp0492AttackAbility(Player, zombieAttackAbility);

                default:
                    Log.Warn($"Unknown base-game subroutine! {subroutineBase.GetType().FullName}");
                    return null;
            }
        }
    }
}