namespace DMD;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;

public static class Hooks
{
    private static bool IsInit { get; set; }

    public static void ApplyInitHooks()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        //On.RainWorld.PostModsInit += RainWorld_PostModsInit;
    }

    public static void ApplyHooks()
    {
        // Misc
        SaveData_Hooks.ApplyHooks();
        On.Menu.IntroRoll.ctor += IntroRoll_ctor1;

        // Player
        Player_Hooks.ApplyHooks();
        PlayerGraphics_Hooks.ApplyHooks();

        // World
        World_Hooks.ApplyHooks();
        Room_Hooks.ApplyHooks();

    }

    private static void IntroRoll_ctor1(On.Menu.IntroRoll.orig_ctor orig, Menu.IntroRoll self, ProcessManager manager)
    {
        orig(self, manager);
        self.illustrations[2] = new Menu.MenuIllustration(self, self.pages[0], "", "Intro_Roll_C_dmd", new Vector2(0f, 0f), true, false);
        self.pages[0].subObjects.Add(self.illustrations[2]);
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
    public static bool Lock;
    private static void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        if (Lock) return;
        Lock = true;
        IL.Menu.IntroRoll.ctor += IntroRoll_ctor;

    }
    private static void IntroRoll_ctor(ILContext il)
    {
        var cursor = new ILCursor(il);
        int localVarNum = 0;

        if (cursor.TryGotoNext(i => i.MatchNewarr<string>())
            && cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(out localVarNum)))
        {
            cursor.Emit(OpCodes.Ldloc, localVarNum);
            cursor.EmitDelegate<Func<string[], string[]>>((oldTitleImages) => [.. oldTitleImages, "dmd"]);
            cursor.Emit(OpCodes.Stloc, localVarNum);
        }
    }
}
