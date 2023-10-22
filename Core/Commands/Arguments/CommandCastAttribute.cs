using System;

using UnityEngine;

namespace Compendium.Commands.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CommandCastAttribute : Attribute
    {
        public CommandArgumentCastOptions Options { get; set; } = new CommandArgumentCastOptions() { Distance = float.MaxValue, Mask = Physics.AllLayers };
    }
}