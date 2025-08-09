namespace DMD;

public static class Hooks
{
    private static bool IsInit { get; set; }

    public static void ApplyInitHooks()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
    }

    public static void ApplyHooks()
    {
        // Misc
        SaveData_Hooks.ApplyHooks();

        // Player
        Player_Hooks.ApplyHooks();
        PlayerGraphics_Hooks.ApplyHooks();

        // World
        World_Hooks.ApplyHooks();
        Room_Hooks.ApplyHooks();
    }


    private static void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        try
        {
            ModOptions.RegisterOI();

            if (IsInit)
            {
                return;
            }

            IsInit = true;


            // Init Info
            var mod = ModManager.ActiveMods.FirstOrDefault(mod => mod.id == Plugin.MOD_ID);

            if (mod is null)
            {
                Plugin.Logger.LogError($"Failed to initialize: ID '{Plugin.MOD_ID}' wasn't found in the active mods list!");
                return;
            }

            Plugin.ModName = mod.name;
            Plugin.Version = mod.version;
            Plugin.Author = mod.authors;


            // Init
            Enums.InitEnums();
            AssetLoader.LoadAssets();

            ApplyHooks();


            // Startup Log
            var initMessage = $"DIEHARD MASTER-DISASTER SAYS HELLO FROM INIT! (VERSION: {Plugin.Version})";

            Debug.Log(initMessage);
            Plugin.Logger.LogInfo(initMessage);
        }
        catch (Exception e)
        {
            e.LogHookException();
        }
        finally
        {
            orig(self);
        }
    }
}
