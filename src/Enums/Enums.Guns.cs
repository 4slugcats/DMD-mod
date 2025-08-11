namespace DMD;

using ObjectType = AbstractPhysicalObject.AbstractObjectType;

public static partial class Enums
{
    public static class Guns
    {
        public static ObjectType AK47 { get; } = new(nameof(AK47), true);
        public static ObjectType RocketLauncher { get; } = new(nameof(RocketLauncher), true);
        public static ObjectType ShotGun { get; } = new(nameof(ShotGun), true);
        public static ObjectType BFG { get; } = new(nameof(BFG), true);
        public static ObjectType MiniGun { get; } = new(nameof(MiniGun), true);
        public static ObjectType Derringer { get; } = new(nameof(Derringer), true);

        public static ObjectType BFGOrb { get; } = new(nameof(BFGOrb), true);
    }
}
