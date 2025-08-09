namespace DMD;

public static class SaveData_Hooks
{
    public static void ApplyHooks()
    {
        On.WinState.CycleCompleted += WinState_CycleCompleted;
        On.SaveState.LoadGame += SaveState_LoadGame;
        On.PlayerProgression.WipeAll += PlayerProgression_WipeAll;
    }


    // Reset misc progression when the slot is reset
    private static void PlayerProgression_WipeAll(On.PlayerProgression.orig_WipeAll orig, PlayerProgression self)
    {
        var miscProg = Utils.MiscProgression;

        miscProg.ResetSave();

        orig(self);
    }


    // Assess and update save data at the end of the cycle
    private static void WinState_CycleCompleted(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
    {
        if (game.IsDMDStory())
        {
            try
            {
                UpdateSaveAfterCycle(game);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"ERROR UPDATING SAVE DATA ON CYCLE COMPLETION:\n{e}");
            }
        }

        orig(self, game);
    }

    private static void UpdateSaveAfterCycle(RainWorldGame game)
    {
        var miscWorld = game.GetMiscWorld();

        if (miscWorld is null)
        {
            return;
        }

        var miscProg = Utils.MiscProgression;
        var saveState = game.GetStorySession.saveState;

        // TODO
    }


    // Assess and update save data just before a cycle
    private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
    {
        orig(self, str, game);

        if (self.saveStateNumber != Enums.DMD)
        {
            return;
        }

        try
        {
            UpdateSaveBeforeCycle(self);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"ERROR UPDATING SAVE BEFORE CYCLE START:\n{e}");
        }
    }

    private static void UpdateSaveBeforeCycle(SaveState self)
    {
        var miscWorld = self.miscWorldSaveData.GetMiscWorld();
        var miscProg = Utils.MiscProgression;

        // TODO
    }
}
