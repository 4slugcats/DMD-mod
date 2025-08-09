namespace DMD;

public static class Player_Helpers
{
    public static bool IsDMD(this Player? player)
    {
        return player?.SlugCatClass == Enums.DMD;
    }

    public static void UpdateDMD(Player self, PlayerModule playerModule)
    {

    }
}
