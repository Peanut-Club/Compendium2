using Common.Values;

using PlayerRoles;

using System;
using UnityEngine;

namespace Compendium.API.Roles
{
    public class Role : IWrapper<PlayerRoleBase>
    {
        public PlayerRoleBase Base { get; }

        public Vector3 SpawnPosition { get; }
    }
}