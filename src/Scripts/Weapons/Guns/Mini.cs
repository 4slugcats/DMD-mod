using RWCustom;

namespace DMD;

public class Mini : Gun
{
    public Mini(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 3;
        reloadSpeed = 110;
        fullClip = 50;
        damageStat = 0.2f;
        automatic = true;
        GunSpriteName = "minigun";
        gunLength = 120;
        randomSpreadStat = 1.4f;
        angleDiff = 3;
        clipCost = 1;
        CheckIfArena(world);
    }
    bool shootswap = false;

    public override void ShootSound()
    {
        room.PlaySound(Enums.Sounds.AK47Shoot, bodyChunks[0], false, .4f + Random.value * .1f, 1.15f + Random.value * .2f);
    }

    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var newBullet = new Bullet(user, firstChunk.pos + upDir * 2f + aimDir * 20, (aimDir.normalized + (Random.insideUnitCircle * randomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, damageStat, 4.5f + 2f * damageStat, 15f + 30f * damageStat, false);
        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= aimDir * 2.5f;
        user.bodyChunks[1].vel -= aimDir * 2f;
    }

    public override void ShootEffects()
    {
        if (shootswap)
        {
            fireDelay += 10;
        }

        var upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 60f, 1f, 4, Color.yellow));
        for (var i = 0; i < 2; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1 + fullClip];
        sLeaser.sprites[0] = new FSprite(GunSpriteName + "1", true);
        sLeaser.sprites[0].anchorY = 0.5f;

        for (var i = 1; i <= fullClip; i++)
        {
            sLeaser.sprites[i] = new FSprite("pixel")
            {
                scale = 4,
                isVisible = false
            };
        }
        firstPipAngle = 0 + (angleDiff / 2 * (fullClip - 1));

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName(GunSpriteName + (Clip == 0 ? "NoMag" : shootswap ? "1" : "0"));
        sLeaser.sprites[0].x = Mathf.Lerp(firstChunk.lastPos.x, firstChunk.pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(firstChunk.lastPos.y, firstChunk.pos.y, timeStacker) - camPos.y;
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), Vector3.Slerp(lastAimDir, aimDir, timeStacker)) - 90f;
        if (mode == Mode.OnBack)
        {
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            var perpV = Custom.PerpendicularVector(v);
            sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), perpV);
            sLeaser.sprites[0].scaleY = -1f;
        }
        else
        {
            sLeaser.sprites[0].scaleY = (flipGun ? 1f : -1f);
        }


        if (owner != null && owner is Player p && mode == Mode.Carried)
        {
            for (var i = 1; i <= fullClip; i++)
            {
                sLeaser.sprites[i].isVisible = i <= Clip && (!HHUtils.inventories.GetOrCreateValue(p)?.active ?? false);
                sLeaser.sprites[i].SetPosition(Custom.DegToVec((firstPipAngle - (angleDiff * (i - 1)))) * 30 + owner.firstChunk.pos + new Vector2(0, 20) - camPos);
            }
        }
        else
        {
            for (var i = 1; i <= fullClip; i++)
            {
                sLeaser.sprites[i].isVisible = false;
            }
        }

        for (var i = 1; i <= fullClip; i++)
        {
            sLeaser.sprites[i].alpha = ownerAge / 20f;
        }


        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
