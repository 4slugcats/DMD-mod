namespace DMD;

public static class SaveData_Helpers
{
    //fix duplicate unlocks on new region load
    public static void UnlockGun(this RainWorldGame game, AbstractPhysicalObject.AbstractObjectType id)
    {
        var miscWorld = game.GetMiscWorld();

        if (miscWorld != null)
        {
            if (!miscWorld.UnlockedGuns.Contains(id.value))
            {
                miscWorld.UnlockedGuns.Add(id.value);
            }
        }
        foreach (var module in game.GetAllDMDModules())
        {
            if (module.PlayerRef == null)
            {
                continue;
            }
            Debug.Log("unlocked " + id);
            AbstractPhysicalObject gun = new AbstractPhysicalObject(game.world,id, null!, module.PlayerRef.abstractCreature.pos,game.GetNewID());
            module.GunInventory.Add(gun);
        }
    }
}
