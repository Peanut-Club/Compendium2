using Common.Values;

using Compendium.API.GameModules.FirstPerson;
using Compendium.API.Roles.Interfaces;
using Compendium.API.Roles.Spawnpoints;

using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;

namespace Compendium.API.Roles.Other
{
    public class FirstPersonRole : Role, IFirstPersonRole, IWrapper<FpcStandardRoleBase>
    {
        public FirstPersonRole(FpcStandardRoleBase roleBase) : base(roleBase)
        {
            Base = roleBase;
            Module = new FirstPersonModule(roleBase.FpcModule);
            SpawnPoint = Spawnpoints.SpawnPoint.Get(Module.Position, roleBase.SpawnpointHandler);
        }

        public ISpawnPoint SpawnPoint { get; }

        public FirstPersonModule Module { get; }

        public new FpcStandardRoleBase Base { get; }

        public BasicRagdoll RagdollPrefab
        {
            get => Base.Ragdoll;
        }        
    }
}