using Common.Values;

using Compendium.API.GameModules.Subroutines;
using Compendium.API.Roles.Interfaces;

using PlayerRoles;
using PlayerRoles.Subroutines;

namespace Compendium.API.Roles.Other
{
    public class SubroutinedRole : Role, ISubroutineRole, IWrapper<ISubroutinedRole>
    {
        public SubroutinedRole(ISubroutinedRole subroutinedRole) : base((PlayerRoleBase)subroutinedRole)
        {
            Base = subroutinedRole;

            Subroutines = Player.Add<SubroutineManager>();
            Subroutines.Initialize(subroutinedRole.SubroutineModule, Player);
        }

        public new ISubroutinedRole Base { get; }

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