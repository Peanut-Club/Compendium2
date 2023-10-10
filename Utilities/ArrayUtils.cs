using HarmonyLib;

using System;
using System.Linq;

namespace Compendium.Utilities
{
    public static class ArrayUtils
    {
        public static void Add<TValue>(ref TValue[] array, TValue value)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            array = array.AddToArray(value);
        }

        public static void Remove<TValue>(ref TValue[] array, TValue value)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            array = array.Where(item => !(item?.Equals(value) ?? false)).ToArray();
        }
    }
}