using static DMD.Player_Helpers;

namespace DMD;

public static class Player_Hooks
{
    public static void ApplyHooks()
    {
        On.Player.Update += Player_Update;
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {

        orig(self, eu);

        if (!self.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        UpdateDMD(self, playerModule);
    }
}
