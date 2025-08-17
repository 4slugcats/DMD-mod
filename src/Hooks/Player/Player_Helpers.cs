namespace DMD;

public static class Player_Helpers
{
    public static bool IsDMD(this Player? player)
    {
        return player?.SlugCatClass == Enums.DMD;
    }

    public static void InitializeDMD(Player self, PlayerModule playerModule)
    {
        var abstractGun = new AbstractPhysicalObject(self.abstractCreature.Room.world, Enums.Guns.AKM, null!, self.abstractCreature.pos, self.abstractCreature.world.game.GetNewID());

        abstractGun.RealizeInRoom();
    }

    public static void UpdateDMD(Player self, PlayerModule playerModule)
    {
        var gun = self.grasps.FirstOrDefault(x => x?.grabbed is Gun)?.grabbed as Gun;

        if (gun is null)
        {
            return;
        }

        var aimDir = self.input[0].analogueDir;

        gun.AimDir = Vector2.Lerp(gun.AimDir, aimDir, 0.1f);

        if (self.input[0].pckp)
        {
            gun.TryShoot(self, aimDir, true);
        }
    }
}
