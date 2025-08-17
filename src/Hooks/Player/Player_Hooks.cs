using static DMD.Player_Helpers;

namespace DMD;

public static class Player_Hooks
{
    public static void ApplyHooks()
    {
        On.Player.ctor += PlayerOnctor;
        On.Player.Update += Player_Update;
    }

    private static void PlayerOnctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (!self.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        InitializeDMD(self, playerModule);
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
