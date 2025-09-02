using RWCustom;

namespace DMD;

public static class Input_Helpers
{
    public static string TriggerAxisId => "DschockHorizontalRight";

    public static float MouseInactivityTimeout => 5.0f;
    public static float MouseInactivityTimer { get; set; }
    public static Vector3 LastMousePos { get; set; }
    public static bool IsMouseActive { get; set; }

    public static bool IsSwapKeybindPressed(this Player player)
    {
        return player.playerState.playerNumber switch
        {
            0 => Input.GetKey(ModOptions.SwapKeybindPlayer1.Value) || Input.GetKey(ModOptions.SwapKeybindKeyboard.Value),
            1 => Input.GetKey(ModOptions.SwapKeybindPlayer2.Value),
            2 => Input.GetKey(ModOptions.SwapKeybindPlayer3.Value),
            3 => Input.GetKey(ModOptions.SwapKeybindPlayer4.Value),

            _ => false,
        };
    }

    public static bool IsSwapLeftInput(this Player player)
    {
        if (ModOptions.SwapTriggerPlayer.Value != 0)
        {
            // Normal
            if (ModOptions.SwapTriggerPlayer.Value > 0)
            {
                if (player.playerState.playerNumber == ModOptions.SwapTriggerPlayer.Value - 1)
                {
                    if (Input.GetAxis(TriggerAxisId) < -0.25f)
                    {
                        return true;
                    }
                }
            }
            // Inverted
            else
            {
                if (player.playerState.playerNumber == -ModOptions.SwapTriggerPlayer.Value + 1)
                {
                    if (Input.GetAxis(TriggerAxisId) > 0.25f)
                    {
                        return true;
                    }
                }
            }
        }

        if (player.IsKeyboardPlayer())
        {
            if (Input.GetKey(ModOptions.SwapLeftKeybind.Value))
            {
                return true;
            }

            if (Input.GetMouseButton(3))
            {
                return true;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0.01f)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsSwapRightInput(this Player player)
    {
        if (ModOptions.SwapTriggerPlayer.Value != 0)
        {
            // Normal
            if (ModOptions.SwapTriggerPlayer.Value > 0)
            {
                if (player.playerState.playerNumber == ModOptions.SwapTriggerPlayer.Value - 1)
                {
                    if (Input.GetAxis(TriggerAxisId) > 0.25f)
                    {
                        return true;
                    }
                }
            }
            // Inverted
            else
            {
                if (player.playerState.playerNumber == -ModOptions.SwapTriggerPlayer.Value + 1)
                {
                    if (Input.GetAxis(TriggerAxisId) < -0.25f)
                    {
                        return true;
                    }
                }
            }
        }

        if (player.IsKeyboardPlayer())
        {
            if (Input.GetKey(ModOptions.SwapRightKeybind.Value))
            {
                return true;
            }

            if (Input.GetMouseButton(4))
            {
                return true;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < -0.01f)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsShootInput(this Player player)
    {
        if (player.IsKeyboardPlayer() && IsMouseActive)
        {
            if (Input.GetMouseButton(0))
            {
                return true;
            }
        }

        return player.input[0].spec;
    }

    public static Vector2 GetAimDir(this Player player)
    {
        if (player.IsKeyboardPlayer())
        {
            var mouseRoomPos = (Vector2)Futile.mousePosition + player.abstractCreature.world.game.cameras.First().pos;

            return Custom.DirVec(player.firstChunk.pos, mouseRoomPos);
        }

        return player.input[0].analogueDir;
    }
}
