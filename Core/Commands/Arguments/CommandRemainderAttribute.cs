﻿using System;

namespace Compendium.Commands.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CommandRemainderAttribute : Attribute
    {
    }
}