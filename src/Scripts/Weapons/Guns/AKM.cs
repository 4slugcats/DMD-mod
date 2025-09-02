using RWCustom;

namespace DMD;

public class AKM : Gun
{
    public AKM(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 6;
        ReloadSpeed = 35;
        FullClip = 15;
        DamageStat = 0.3f;
        Automatic = true;
        GunSpriteName = "dmd_akm";
        GunLength = 60;
        RandomSpreadStat = 1.4f;
        PipAngleDiff = 19;
        ClipCost = 1;
        CheckIfArena(world);
    }

    protected override void ShootSound()
    {
        room.PlaySound(SoundID.Fire_Spear_Explode, bodyChunks[0], false, .36f + Random.value * .02f, 1.05f + Random.value * .2f);
    }

    protected override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var newBullet = new Bullet(user, firstChunk.pos + UpDir * 5f,
            (AimDir.normalized +
             (Random.insideUnitCircle * RandomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized,
            DamageStat, 4.5f + 2f * DamageStat, 15f + 30f * DamageStat, false);

        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= AimDir * 2f;
        user.bodyChunks[1].vel -= AimDir * 2f;
    }

    protected override void ShootEffects()
    {
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
}
