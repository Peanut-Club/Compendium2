using Common.Values;

using Compendium.API.GameModules.FirstPerson;
using Compendium.API.GameModules.Subroutines;

using Compendium.API.Roles.Interfaces;
using Compendium.API.Roles.Spawnpoints;

using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerRoles.Subroutines;
using PlayerRoles.FirstPersonControl;

namespace Compendium.API.Roles.Other
{
    public class SubroutinedFirstPersonRole : Role, IFirstPersonRole, ISubroutineRole, IWrapper<ISubroutinedRole>
    {
        public SubroutinedFirstPersonRole(ISubroutinedRole subroutinedRole) : base((PlayerRoleBase)subroutinedRole)
        {
            Base = subroutinedRole;
            FpcBase = (FpcStandardRoleBase)subroutinedRole;

            Subroutines = Player.Add<SubroutineManager>();
            Subroutines.Initialize(subroutinedRole.SubroutineModule, Player);

            Module = new FirstPersonModule(FpcBase.FpcModule);

            SpawnPoint = Spawnpoints.SpawnPoint.Get(Module.Position, FpcBase.SpawnpointHandler);
        }

        public FirstPersonModule Module { get; }
        public FpcStandardRoleBase FpcBase { get; }

        public ISpawnPoint SpawnPoint { get; }

        public new ISubroutinedRole Base { get; }

        public BasicRagdoll RagdollPrefab
        {
            get => FpcBase.Ragdoll;
        }

        public SubroutineManager Subroutines { get; }


        public TRoutine AddRoutine<TRoutine>() where TRoutine : ICustomSubroutine
            => Subroutines.Add<TRoutine>();

        public TRoutine GetRoutine<TRoutine>() where TRoutine : ISubroutine
            => Subroutines.Get<TRoutine>();

        public bool RemoveRoutine<TRoutine>() where TRoutine : ICustomSubroutine
            => Subroutines.Remove<TRoutine>();

        public bool TryGetRoutine<TRoutine>(out TRoutine routine) where TRoutine : ISubroutine
            => Subroutines.TryGet(out routine);
    }
}