namespace DMD;

public static partial class Enums
{
    public static SlugcatStats.Name DMD { get; } = new(nameof(DMD));

    public static void InitEnums()
    {
        _ = DMD;
        _ = Scenes.Slugcat_DMD;
        _ = Guns.AKM;
    }
}
