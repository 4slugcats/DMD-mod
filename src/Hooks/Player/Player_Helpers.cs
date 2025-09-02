namespace DMD;

public static class Player_Helpers
{
    public static bool IsDMD(this Player? player)
    {
        return player?.SlugCatClass == Enums.DMD;
    }

    public static bool IsKeyboardPlayer(this Player player)
    {
        return player.input[0].controllerType == Options.ControlSetup.Preset.KeyboardSinglePlayer;
    }

    public static void InitializeDMD(Player self, PlayerModule playerModule)
    {
        self.abstractCreature.world.game.UnlockGun(Enums.Guns.AKM);
    }

    public static void UpdateDMD(Player self, PlayerModule playerModule)
    {
        UpdateInput(self, playerModule);
        UpdateHUD(playerModule);

        foreach (var gun in playerModule.GunInventory)
        {
            UpdateGun(self, playerModule, gun);
        }
    }

    private static void UpdateInput(Player self, PlayerModule playerModule)
    {
        var unblockedInput = playerModule.UnblockedInput;
        var allowInput = self.Consious && !self.inVoidSea && !self.Sleeping && self.controller is null;

        var swapLeftInput = self.IsSwapLeftInput() && allowInput;
        var swapRightInput = self.IsSwapRightInput() && allowInput;

        var swapInput = self.IsSwapKeybindPressed() && allowInput;

        playerModule.BlockInput = false;

        if (swapLeftInput && !playerModule.WasSwapLeftInput)
        {
            self.SelectPreviousGun(playerModule);
        }
        else if (swapRightInput && !playerModule.WasSwapRightInput)
        {
            self.SelectNextGun(playerModule);
        }
        else if (swapInput)
        {
            playerModule.BlockInput = true;
            playerModule.ShowHUD(10);

            if (!playerModule.WasSwapped)
            {
                if (unblockedInput.x < -0.5f)
                {
                    self.SelectPreviousGun(playerModule);
                    playerModule.WasSwapped = true;
                }
                else if (unblockedInput.x > 0.5f)
                {
                    self.SelectNextGun(playerModule);
                    playerModule.WasSwapped = true;
                }
            }
            else if (Mathf.Abs(unblockedInput.x) < 0.5f)
            {
                playerModule.WasSwapped = false;
            }
        }
    }

    private static void UpdateHUD(PlayerModule playerModule)
    {
        if (playerModule.HudFadeTimer > 0)
        {
            playerModule.HudFadeTimer--;
            playerModule.HudFade = Mathf.Lerp(playerModule.HudFade, 1.0f, 0.1f);
        }
        else
        {
            playerModule.HudFadeTimer = 0;
            playerModule.HudFade = Mathf.Lerp(playerModule.HudFade, 0.0f, 0.05f);
        }
    }

    private static void UpdateGun(Player player, PlayerModule playerModule, AbstractPhysicalObject abstractGun)
    {
        if (abstractGun != playerModule.ActiveGun)
        {
            abstractGun.world = player.abstractCreature.world;
            abstractGun.pos = player.abstractCreature.pos;

            player.abstractCreature.Room.AddEntity(abstractGun);
            abstractGun.RealizeInRoom();
        }
        else
        {
            abstractGun.Abstractize(player.abstractCreature.pos);
            abstractGun.Room?.RemoveEntity(abstractGun);
        }

        if (abstractGun.realizedObject is not Gun gun)
        {
            return;
        }

        if (player.IsShootInput())
        {
            var aimDir = player.GetAimDir();

            gun.TryShoot(player, aimDir, false);
        }
    }

    private static void SelectPreviousGun(this Player player, PlayerModule playerModule)
    {
        if (playerModule.GunInventory.Count == 0)
        {
            return;
        }

        var targetIndex = playerModule.ActiveGunIndex - 1;

        if (targetIndex < 0)
        {
            targetIndex = playerModule.GunInventory.Count - 1;
        }

        player.SetActiveGun(playerModule, targetIndex, true);
    }

    private static void SelectNextGun(this Player player, PlayerModule playerModule)
    {
        if (playerModule.GunInventory.Count == 0)
        {
            return;
        }

        var targetIndex = playerModule.ActiveGunIndex + 1;

        if (targetIndex >= playerModule.GunInventory.Count)
        {
            targetIndex = 0;
        }

        player.SetActiveGun(playerModule, targetIndex);
    }

    private static void SetActiveGun(this Player player, PlayerModule playerModule, int index, bool silent = false)
    {
        if (index < 0 || index >= playerModule.GunInventory.Count)
        {
            return;
        }

        if (playerModule.ActiveGunIndex == index)
        {
            return;
        }

        playerModule.ActiveGunIndex = index;

        if (silent)
        {
            return;
        }

        playerModule.ShowHUD(60);
        player.PlayHUDSound(SoundID.Rock_Hit_Wall);
    }
}
