using System;

namespace Compendium.Utilities
{
    public static class AssertUtils
    {
        public static bool AssertionEnabled;

        static AssertUtils()
        {
#if DEBUG
            AssertionEnabled = true;
#else
            AssertionEnabled = false;
#endif
        }

        public static void ThrowIfNull(params object[] args)
        {
            if (!AssertionEnabled)
                return;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is null)
                    throw new ArgumentNullException($"Argument at index '{i}' of method is null.", default(Exception));
            }
        }
    }
}