using Menu;

namespace DMD;

public static partial class Enums
{
    // Don't need to register these, SlugBase does it for us
    public static class Scenes
    {
        // Select Screen
        public static MenuScene.SceneID Slugcat_DMD { get; } = new(nameof(Slugcat_DMD));
    }
}
