namespace DMD;

public static class World_Hooks
{
    public static void ApplyHooks()
    {
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObjectOnRealize;
        On.ItemSymbol.SpriteNameForItem += ItemSymbolOnSpriteNameForItem;
    }

    private static string ItemSymbolOnSpriteNameForItem(On.ItemSymbol.orig_SpriteNameForItem orig, AbstractPhysicalObject.AbstractObjectType itemType, int intData)
    {
        if (itemType == Enums.Guns.None)
        {
            return "dmd_icon_hand";
        }

        if (itemType == Enums.Guns.AKM)
        {
            return "dmd_icon_akm";
        }

        if (itemType == Enums.Guns.Shotgun)
        {
            return "dmd_icon_shotgun";
        }

        if (itemType == Enums.Guns.RocketLauncher)
        {
            return "dmd_icon_rocket_launcher";
        }

        if (itemType == Enums.Guns.BFG)
        {
            return "dmd_icon_bfg";
        }

        if (itemType == Enums.Guns.Minigun)
        {
            return "dmd_icon_minigun";
        }

        if (itemType == Enums.Guns.Derringer)
        {
            return "dmd_icon_derringer";
        }

        if (itemType == Enums.Guns.BFGOrb)
        {
            return "dmd_icon_bfg_orb";
        }

        if (itemType == Enums.Guns.Pipe)
        {
            return "dmd_icon_pipe";
        }

        return orig(itemType, intData);
    }

    private static void AbstractPhysicalObjectOnRealize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
    {
        if (self.realizedObject is null)
        {
            if (self.type == Enums.Guns.AKM)
            {
                self.realizedObject = new AKM(self, self.world);
            }
            else if (self.type == Enums.Guns.Shotgun)
            {
                self.realizedObject = new Shotgun(self, self.world);
            }
        }
        orig(self);
    }
}
