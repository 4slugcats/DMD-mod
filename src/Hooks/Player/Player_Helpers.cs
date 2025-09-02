using RWCustom;

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
        var game = self.abstractCreature.world.game;

        game.UnlockGun(Enums.Guns.AKM);
        game.UnlockGun(Enums.Guns.Shotgun);
        game.UnlockGun(Enums.Guns.Minigun);
        // game.UnlockGun(Enums.Guns.RocketLauncher);
        // game.UnlockGun(Enums.Guns.BFG);
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

        playerModule.WasSwapLeftInput = swapLeftInput;
        playerModule.WasSwapRightInput = swapRightInput;
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
        if (abstractGun.type == Enums.Guns.None)
        {
            return;
        }

        if (abstractGun == playerModule.ActiveGun)
        {
            TryRealizeGun(player, abstractGun);
        }
        else
        {
            TryAbstractGun(player, abstractGun);
        }

        if (abstractGun.realizedObject is not Gun gun)
        {
            return;
        }

        if (!gun.OneHanded && player.grasps.Any(x => x is not null && x.grabbed != gun))
        {
            gun.AllGraspsLetGoOfThisObject(true);
            gun.ChangeOverlap(false);

            gun.mode = Weapon.Mode.OnBack;
            gun.firstChunk.pos = player.firstChunk.pos;

            gun.setRotation = Vector3.Slerp(gun.rotation, Custom.rotateVectorDeg(Vector2.up, player.flipDirection * 20.0f), 0.2f);
            gun.IsFlipped = player.flipDirection == -1;

            Plugin.Logger.LogWarning(gun.rotation);

            if (player.firstChunk.vel.x > 1.0f)
            {
                gun.setRotation = Custom.rotateVectorDeg(gun.rotation, Random.Range(0.5f, 3.0f));
            }
        }
        else
        {
            if (!gun.grabbedBy.Any())
            {
                player.SlugcatGrab(gun, player.FreeHand());
            }

            gun.ChangeOverlap(true);

            var aimDir = player.GetAimDir();

            gun.AimDir = aimDir;
            gun.IsFlipped = aimDir.x > 0;

            if (player.IsShootInput())
            {
                gun.TryShoot(player, aimDir);
            }
        }

    }

    private static void TryAbstractGun(Player player, AbstractPhysicalObject abstractGun)
    {
        if (abstractGun.realizedObject is null)
        {
            return;
        }

        abstractGun.realizedObject.AllGraspsLetGoOfThisObject(true);

        abstractGun.Abstractize(player.abstractCreature.pos);
        abstractGun.Room?.RemoveEntity(abstractGun);
    }

    private static void TryRealizeGun(Player player, AbstractPhysicalObject abstractGun)
    {
        if (abstractGun.realizedObject is not null)
        {
            return;
        }

        abstractGun.world = player.abstractCreature.world;
        abstractGun.pos = player.abstractCreature.pos;

        player.abstractCreature.Room.AddEntity(abstractGun);
        abstractGun.RealizeInRoom();
    }

    private static void SelectPreviousGun(this Player player, PlayerModule playerModule)
    {
        var targetIndex = playerModule.ActiveGunIndex - 1;

        if (targetIndex < 0)
        {
            targetIndex = playerModule.GunInventory.Count - 1;
        }

        player.SetActiveGun(playerModule, targetIndex);
    }

    private static void SelectNextGun(this Player player, PlayerModule playerModule)
    {
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
