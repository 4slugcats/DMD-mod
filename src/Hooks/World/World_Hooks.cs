namespace DMD;

public static class World_Hooks
{
    public static void ApplyHooks()
    {
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObjectOnRealize;
        On.ItemSymbol.SpriteNameForItem += ItemSymbolOnSpriteNameForItem;

        On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
        On.HUD.HUD.InitSafariHud += HUD_InitSafariHud;
        On.ArenaGameSession.AddHUD += ArenaGameSession_AddHUD;
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
            else if (self.type == Enums.Guns.Minigun)
            {
                self.realizedObject = new Minigun(self, self.world);
            }
        }
        orig(self);
    }

    // Add Inventory HUD
    private static void ArenaGameSession_AddHUD(On.ArenaGameSession.orig_AddHUD orig, ArenaGameSession self)
    {
        orig(self);

        var hud = self.game.cameras[0].hud;

        hud.AddPart(new InventoryHUD(hud, hud.fContainers[1]));
    }

    private static void HUD_InitSafariHud(On.HUD.HUD.orig_InitSafariHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        self.AddPart(new InventoryHUD(self, self.fContainers[1]));
    }

    private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        self.AddPart(new InventoryHUD(self, self.fContainers[1]));
    }

}
