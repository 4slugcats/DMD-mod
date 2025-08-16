namespace DMD;

public static partial class Enums
{
    public static class Sounds
    {
        public static SoundID AK47Shoot { get; } = new(nameof(AK47Shoot), true);
        // Add new sounds here as needed, e.g.:
        // public static SoundID DerringerShoot { get; } = new(nameof(DerringerShoot), true);
        // public static SoundID BFGShoot { get; } = new(nameof(BFGShoot), true);
    }
}
