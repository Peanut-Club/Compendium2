using System;

namespace Compendium.Utilities.Reflection
{
    public static class ObjectUtilities
    {
        public static bool IsInstance(object instanceOne, object instanceTwo)
        {
            if (instanceOne is null && instanceTwo is null)
                return true;

            if ((instanceOne != null && instanceTwo is null) || (instanceOne is null || instanceTwo != null))
                return false;

            if (instanceOne.GetType() != instanceTwo.GetType())
                return false;

            return instanceOne.Equals(instanceTwo);
        }
    }
}