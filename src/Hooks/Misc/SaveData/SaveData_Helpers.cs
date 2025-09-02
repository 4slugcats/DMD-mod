namespace DMD;

public static class SaveData_Helpers
{
    public static void UnlockGun(this RainWorldGame game, AbstractPhysicalObject.AbstractObjectType id)
    {
        var miscWorld = game.GetMiscWorld();

        if (miscWorld is null)
        {
            return;
        }

        if (miscWorld.UnlockedGuns.Contains(id.value))
        {
            return;
        }

        miscWorld.UnlockedGuns.Add(id.value);

        foreach (var module in game.GetAllDMDModules())
        {
            if (module.PlayerRef is null)
            {
                continue;
            }

            var gun = new AbstractPhysicalObject(game.world,
                id, null!, module.PlayerRef.abstractCreature.pos,
                game.GetNewID());

            module.GunInventory.Add(gun);
        }
    }
}
