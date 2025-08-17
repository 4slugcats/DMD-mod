using Vector2 = UnityEngine.Vector2;

using static DMD.PlayerGraphics_Helpers;

namespace DMD;

public static class PlayerGraphics_Hooks
{
    public static void ApplyHooks()
    {
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.Reset += PlayerGraphics_Reset;
    }

    // Initialization
    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (!self.player.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        var graphics = playerModule.Graphics;

        // Init Indexes
        graphics.FirstSprite = sLeaser.sprites.Length;

        // TODO
        // var spriteIndex = graphics.FirstSprite;
        //
        //
        //
        // graphics.LastSprite = spriteIndex;
        // Array.Resize(ref sLeaser.sprites, spriteIndex);
        graphics.LastSprite = sLeaser.sprites.Length - 1;

        self.AddToContainer(sLeaser, rCam, null);
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);

        if (!self.player.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        var graphics = playerModule.Graphics;

        if (graphics.FirstSprite <= 0 || sLeaser.sprites.Length < graphics.LastSprite)
        {
            Plugin.Logger.LogError("Invalid sprite indexes detected - something went wrong during sprite initialization.");
            return;
        }

        newContatiner ??= rCam.ReturnFContainer("Midground");

        OrderAndColorSprites(self, sLeaser, rCam, Vector2.zero, playerModule, newContatiner);
    }

    private static void PlayerGraphics_Reset(On.PlayerGraphics.orig_Reset orig, PlayerGraphics self)
    {
        orig(self);

        if (!self.player.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        // TODO
    }

    // Draw
    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (!self.player.TryGetDMDModule(out var playerModule))
        {
            return;
        }

        UpdateReplacementPlayerSprite(sLeaser, HEAD_SPRITE, "Head", "dmd_full");
        UpdateReplacementPlayerSprite(sLeaser, FACE_SPRITE, self.RenderAsPup ? "PFace" : "Face", "dmd_full");

        UpdateReplacementPlayerSprite(sLeaser, ARM_L_SPRITE, "PlayerArm", "dmd_full");
        UpdateReplacementPlayerSprite(sLeaser, ARM_R_SPRITE, "PlayerArm", "dmd_full");

        UpdateReplacementPlayerSprite(sLeaser, HAND_L_SPRITE, "OnTopOfTerrainHand", "dmd_full");
        UpdateReplacementPlayerSprite(sLeaser, HAND_R_SPRITE, "OnTopOfTerrainHand", "dmd_full");

        UpdateReplacementPlayerSprite(sLeaser, BODY_SPRITE, "Body", "dmd_full");
        UpdateReplacementPlayerSprite(sLeaser, HIPS_SPRITE, "Hips", "dmd_full");

        UpdateReplacementPlayerSprite(sLeaser, LEGS_SPRITE, "Legs", "dmd_full");

        OrderAndColorSprites(self, sLeaser, rCam, camPos, playerModule);
    }
}
