using SlugBase.SaveData;
using System.Runtime.CompilerServices;

namespace DMD;

public static class ModuleManager
{
    // Player
    public static ConditionalWeakTable<AbstractCreature, PlayerModule> DMDData { get; } = new();

    public static bool TryGetDMDModule(this Player self, out PlayerModule playerModule)
    {
        if (!self.IsDMD())
        {
            playerModule = null!;
            return false;
        }

        playerModule = DMDData.GetValue(self.abstractCreature, _ => new PlayerModule(self));
        return true;
    }

    public static List<PlayerModule> GetAllDMDModules(this RainWorldGame game)
    {
        var allDMDs = game.GetAllDMDs();
        var playerModules = new List<PlayerModule>();

        foreach (var abstractCreature in allDMDs)
        {
            if (abstractCreature.realizedObject is not Player player)
            {
                continue;
            }

            if (!player.TryGetDMDModule(out var playerModule))
            {
                continue;
            }

            playerModules.Add(playerModule);
        }

        return playerModules;
    }

    // Save Data
    public static SaveMiscWorld? GetMiscWorld(this RainWorldGame game)
    {
        return game.IsStorySession ? GetMiscWorld(game.GetStorySession.saveState.miscWorldSaveData) : null;
    }

    public static SaveMiscWorld GetMiscWorld(this MiscWorldSaveData data)
    {
        if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out SaveMiscWorld save))
        {
            data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
        }

        return save;
    }

    public static SaveMiscProgression GetMiscProgression(this RainWorld rainWorld)
    {
        return GetMiscProgression(rainWorld.progression.miscProgressionData);
    }

    public static SaveMiscProgression GetMiscProgression(this PlayerProgression.MiscProgressionData data)
    {
        if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out SaveMiscProgression save))
        {
            data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
        }

        return save;
    }
}
