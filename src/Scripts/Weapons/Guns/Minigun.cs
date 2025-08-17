using RWCustom;

namespace DMD;

public class Minigun : Gun
{
    public Minigun(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 3;
        ReloadSpeed = 110;
        FullClip = 50;
        DamageStat = 0.2f;
        Automatic = true;
        GunSpriteName = "dmd_minigun1";
        GunLength = 120;
        RandomSpreadStat = 1.4f;
        PipAngleDiff = 3;
        ClipCost = 1;
        CheckIfArena(world);
    }
    bool shootswap = false;

    public override void ShootSound()
    {
        room.PlaySound(Enums.Sounds.AKMShoot, bodyChunks[0], false, .4f + Random.value * .1f, 1.15f + Random.value * .2f);
    }

    protected override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var newBullet = new Bullet(user, firstChunk.pos + UpDir * 2f + AimDir * 20, (AimDir.normalized + (Random.insideUnitCircle * RandomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, DamageStat, 4.5f + 2f * DamageStat, 15f + 30f * DamageStat, false);
        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= AimDir * 2.5f;
        user.bodyChunks[1].vel -= AimDir * 2f;
    }

    protected override void ShootEffects()
    {
        if (shootswap)
        {
            FireDelay += 10;
        }

        var upDir = Custom.PerpendicularVector(AimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + AimDir * 35f, 60f, 1f, 4, Color.yellow));
        for (var i = 0; i < 2; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + LastAimDir * 25f, AimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1 + FullClip];
        sLeaser.sprites[0] = new FSprite(GunSpriteName + "1", true);
        sLeaser.sprites[0].anchorY = 0.5f;

        for (var i = 1; i <= FullClip; i++)
        {
            sLeaser.sprites[i] = new FSprite("pixel")
            {
                scale = 4,
                isVisible = false
            };
        }
        FirstPipAngle = 0 + (PipAngleDiff / 2 * (FullClip - 1));

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        // TODO
        //sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName(GunSpriteName + (Clip == 0 ? "empty" : shootswap ? "1" : "0"));
        sLeaser.sprites[0].x = Mathf.Lerp(firstChunk.lastPos.x, firstChunk.pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(firstChunk.lastPos.y, firstChunk.pos.y, timeStacker) - camPos.y;
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), Vector3.Slerp(LastAimDir, AimDir, timeStacker)) - 90f;
        if (mode == Mode.OnBack)
        {
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            var perpV = Custom.PerpendicularVector(v);
            sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), perpV);
            sLeaser.sprites[0].scaleY = -1f;
        }
        else
        {
            sLeaser.sprites[0].scaleY = (IsFlipped ? 1f : -1f);
        }


        if (Owner != null && Owner is Player p && mode == Mode.Carried)
        {
            for (var i = 1; i <= FullClip; i++)
            {
                sLeaser.sprites[i].isVisible = i <= Clip; // TODO
                sLeaser.sprites[i].SetPosition(Custom.DegToVec((FirstPipAngle - (PipAngleDiff * (i - 1)))) * 30 + Owner.firstChunk.pos + new Vector2(0, 20) - camPos);
            }
        }
        else
        {
            for (var i = 1; i <= FullClip; i++)
            {
                sLeaser.sprites[i].isVisible = false;
            }
        }

        for (var i = 1; i <= FullClip; i++)
        {
            sLeaser.sprites[i].alpha = OwnerAge / 20f;
        }


        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
