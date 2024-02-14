namespace Compendium.API.Enums
{
    public enum DamageType : byte
    {
        #region Vanilla
        AlphaWarhead = 1,
        Asphyxiated = 4,

        Bleeding = 5,
        BulletWounds = 19,

        CardiacArrest = 24,
        Crushed = 20,

        Decontamination = 8,

        Explosion = 14,

        Falldown = 6,
        FriendlyFireDetector = 22,

        Hypothermia = 23,

        MarshmallowMan = 27,
        MicroHid = 12,

        PocketDecay = 7,
        Poison = 9,

        Recontained = 0,

        Scp049 = 2,
        Scp096 = 15,
        Scp173 = 16,
        Scp207 = 10,

        Scp3114Slap = 26,

        Scp939Lunge = 17,
        Scp939Other = 25,

        SeveredHands = 11,

        TeslaGate = 13,

        Unknown = 3,

        Scp106Bait = 21,

        Zombie = 18,
        #endregion

        #region Custom
        Scp3114Strangulation = 100,
        Scp3114 = 101,
        Scp018 = 102,
        Custom = 103,

        Com15 = 110,
        Com18,
        Com45,

        A7,
        Ak,

        Fsp9,
        FrMg0,

        Epsilon11,
        Disruptor,
        Revolver,
        Jailbird,
        Crossvec,
        Logicer,
        Shotgun,

        Firearm
        #endregion
    }
}
