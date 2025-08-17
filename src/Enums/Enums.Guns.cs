namespace DMD;

using ObjectType = AbstractPhysicalObject.AbstractObjectType;

public static partial class Enums
{
    public static class Guns
    {
        public static ObjectType AKM { get; } = new(nameof(AKM), true);
        public static ObjectType RocketLauncher { get; } = new(nameof(RocketLauncher), true);
        public static ObjectType Shotgun { get; } = new(nameof(Shotgun), true);
        public static ObjectType BFG { get; } = new(nameof(BFG), true);
        public static ObjectType Minigun { get; } = new(nameof(Minigun), true);
        public static ObjectType Derringer { get; } = new(nameof(Derringer), true);

        public static ObjectType BFGOrb { get; } = new(nameof(BFGOrb), true);
        public static ObjectType Pipe { get; } = new(nameof(Pipe), true);
    }
}
