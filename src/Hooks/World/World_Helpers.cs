namespace DMD;

public static class World_Helpers
{
    public static bool IsDMDStory(this RainWorldGame? game)
    {
        return game?.StoryCharacter == Enums.DMD;
    }

    public static AbstractCreature? GetFirstDMD(this RainWorldGame? game)
    {
        return game?.GetAllDMDs().FirstOrDefault();
    }

    public static List<AbstractCreature> GetAllDMDs(this RainWorldGame? game)
    {
        return game.GetAllPlayers().Where(x => x.realizedObject is Player player && player.IsDMD()).ToList();
    }

    public static List<AbstractCreature> GetAllPlayers(this RainWorldGame? game)
    {
        return game is null ? [] : game.Players;
    }

    public static void AddTextPrompt(this RainWorldGame game, string text, int wait, int time, bool darken = false, bool? hideHud = null)
    {
        hideHud ??= ModManager.MMF;

        game.cameras.First().hud.textPrompt.AddMessage(Utils.Translator.Translate(text), wait, time, darken, (bool)hideHud);
    }
}
