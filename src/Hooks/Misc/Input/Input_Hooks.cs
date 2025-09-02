using static DMD.Input_Helpers;

namespace DMD;

public static class Input_Hooks
{
    public static void ApplyHooks()
    {
        On.RainWorld.Update += RainWorldOnUpdate;
    }

    private static void RainWorldOnUpdate(On.RainWorld.orig_Update orig, RainWorld self)
    {
        orig(self);

        if (LastMousePos != Futile.mousePosition || Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            IsMouseActive = true;
            MouseInactivityTimer = 0.0f;
        }
        else if (MouseInactivityTimer < MouseInactivityTimeout)
        {
            MouseInactivityTimer += Time.deltaTime;
        }
        else
        {
            IsMouseActive = false;
        }

        LastMousePos = Futile.mousePosition;
    }
}
