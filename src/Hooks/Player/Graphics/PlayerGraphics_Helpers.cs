using Vector2 = UnityEngine.Vector2;

namespace DMD;

public static class PlayerGraphics_Helpers
{
    // Default sprite indexes, as set by the game
    public const int BODY_SPRITE = 0;
    public const int HIPS_SPRITE = 1;
    public const int TAIL_SPRITE = 2;
    public const int HEAD_SPRITE = 3;
    public const int LEGS_SPRITE = 4;
    public const int ARM_L_SPRITE = 5;
    public const int ARM_R_SPRITE = 6;
    public const int HAND_L_SPRITE = 7;
    public const int HAND_R_SPRITE = 8;
    public const int FACE_SPRITE = 9;
    public const int MARK_GLOW_SPRITE = 10;
    public const int MARK_SPRITE = 11;


    public static void OrderAndColorSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, Vector2 camPos, PlayerModule playerModule, FContainer? newContainer = null)
    {
        // Base
        var bodySprite = sLeaser.sprites[BODY_SPRITE];
        var armLSprite = sLeaser.sprites[ARM_L_SPRITE];
        var armRSprite = sLeaser.sprites[ARM_R_SPRITE];
        var hipsSprite = sLeaser.sprites[HIPS_SPRITE];
        var tailSprite = sLeaser.sprites[TAIL_SPRITE];
        var headSprite = sLeaser.sprites[HEAD_SPRITE];
        var handLSprite = sLeaser.sprites[HAND_L_SPRITE];
        var handRSprite = sLeaser.sprites[HAND_R_SPRITE];
        var legsSprite = sLeaser.sprites[LEGS_SPRITE];
        var markGlowSprite = sLeaser.sprites[MARK_GLOW_SPRITE];
        var markSprite = sLeaser.sprites[MARK_SPRITE];

        if (self.player.inVoidSea)
        {
            markSprite.alpha = 0.0f;
            markGlowSprite.alpha = 0.0f;
        }

        // Correct arm layering
        if ((Mathf.Abs(self.player.firstChunk.vel.x) <= 1.0f && self.player.bodyMode != Player.BodyModeIndex.Crawl) || self.player.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
        {
            armLSprite.MoveBehindOtherNode(bodySprite);
            armRSprite.MoveBehindOtherNode(bodySprite);
        }
        else
        {
            // Right
            if (self.player.flipDirection == 1)
            {
                armLSprite.MoveInFrontOfOtherNode(headSprite);
                armRSprite.MoveBehindOtherNode(bodySprite);
            }
            // Left
            else
            {
                armRSprite.MoveInFrontOfOtherNode(headSprite);
                armLSprite.MoveBehindOtherNode(bodySprite);
            }
        }
    }

    public static void UpdateCustomPlayerSprite(RoomCamera.SpriteLeaser sLeaser, int spriteIndexToCopy, string toCopy, string atlasName, string customName, int spriteIndex)
    {
        sLeaser.sprites[spriteIndex].isVisible = false;

        var atlas = AssetLoader.GetAtlas(atlasName);

        if (atlas is null)
        {
            return;
        }

        var name = sLeaser.sprites[spriteIndexToCopy]?.element?.name;

        if (name is null)
        {
            return;
        }

        name = name.Replace(toCopy, customName);


        if (!atlas._elementsByName.TryGetValue(Plugin.MOD_ID + name, out var element))
        {
            return;
        }

        sLeaser.sprites[spriteIndex].element = element;


        var spriteToCopy = sLeaser.sprites[spriteIndexToCopy];

        sLeaser.sprites[spriteIndex].isVisible = spriteToCopy.isVisible;

        sLeaser.sprites[spriteIndex].SetPosition(spriteToCopy.GetPosition());
        sLeaser.sprites[spriteIndex].SetAnchor(spriteToCopy.GetAnchor());

        sLeaser.sprites[spriteIndex].scaleX = spriteToCopy.scaleX;
        sLeaser.sprites[spriteIndex].scaleY = spriteToCopy.scaleY;
        sLeaser.sprites[spriteIndex].rotation = spriteToCopy.rotation;
    }

    public static void UpdateReplacementPlayerSprite(RoomCamera.SpriteLeaser sLeaser, int spriteIndex, string toReplace, string atlasName, string nameSuffix = "")
    {
        var atlas = AssetLoader.GetAtlas(atlasName);

        if (atlas is null)
        {
            return;
        }

        var name = sLeaser.sprites[spriteIndex]?.element?.name;

        if (name is null)
        {
            return;
        }

        if (!name.StartsWith(toReplace))
        {
            return;
        }

        if (!atlas._elementsByName.TryGetValue(Plugin.MOD_ID + name + nameSuffix, out var element))
        {
            return;
        }

        sLeaser.sprites[spriteIndex].element = element;
    }
}
