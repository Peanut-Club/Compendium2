namespace Compendium.Patching
{
    public static class Patches
    {
        public static PatchFlags PropertySetterPrefix = PatchFlags.IsPrefix | PatchFlags.Set;
        public static PatchFlags PropertySetterPostfix = PatchFlags.IsPostfix | PatchFlags.Set;
        public static PatchFlags PropertySetterTranspiler = PatchFlags.IsTranspiler | PatchFlags.Set;
        public static PatchFlags PropertySetterIL = PatchFlags.IsIL | PatchFlags.Set;

        public static PatchFlags PropertyGetterPrefix = PatchFlags.IsPrefix | PatchFlags.Get;
        public static PatchFlags PropertyGetterPostfix = PatchFlags.IsPostfix | PatchFlags.Get;
        public static PatchFlags PropertyGetterTranspiler = PatchFlags.IsTranspiler | PatchFlags.Get;
        public static PatchFlags PropertyGetterIL = PatchFlags.IsIL | PatchFlags.Get;

        public static PatchFlags Prefix = PatchFlags.IsPrefix;
        public static PatchFlags Postfix = PatchFlags.IsPostfix;
        public static PatchFlags Transpiler = PatchFlags.IsTranspiler;
        public static PatchFlags IL = PatchFlags.IsIL;
    }
}