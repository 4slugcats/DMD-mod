using System.Text.RegularExpressions;

namespace DMD;

public static class Input_Helpers
{
    public static string TriggerAxisId => "DschockHorizontalRight";

    public static bool IsSwapKeybindPressed(this Player player)
    {
        return player.playerState.playerNumber switch
        {
            0 => Input.GetKey(ModOptions.SwapKeybindPlayer1) || Input.GetKey(ModOptions.SwapKeybindKeyboard),
            1 => Input.GetKey(ModOptions.SwapKeybindPlayer2),
            2 => Input.GetKey(ModOptions.SwapKeybindPlayer3),
            3 => Input.GetKey(ModOptions.SwapKeybindPlayer4),

            _ => false,
        };
    }
}
