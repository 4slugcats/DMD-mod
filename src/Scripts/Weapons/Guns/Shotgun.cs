using RWCustom;

namespace DMD;

public class Shotgun : Gun
{
    public Shotgun(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 30;
        ReloadSpeed = 64;
        FullClip = 3;
        DamageStat = 1.8f;
        GunSpriteName = "dmd_shotgun";
        GunLength = 47;
        RandomSpreadStat = 5f;
        PipAngleDiff = 30;
        ClipCost = 2;
        CheckIfArena(world);
    }

    public override void ShootSound()
    {
        room.PlaySound(Enums.Sounds.AKMShoot, bodyChunks[0], false, .5f + Random.value * .03f, 1f + Random.value * .07f);
        room.PlaySound(Enums.Sounds.AKMShoot, bodyChunks[0], false, .45f + Random.value * .03f, .9f + Random.value * .07f);
    }

    protected override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var mult = 6;
        for (var i = mult; i > 0; i--)
        {
            var newBullet = new Bullet(user, firstChunk.pos + UpDir * 5f, (AimDir.normalized + (Random.insideUnitCircle * RandomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, DamageStat / mult, 1.5f + 2f * DamageStat / mult, 15f + 30f * DamageStat / mult, false);
            room.AddObject(newBullet);
            newBullet.Fire();
        }
        user.bodyChunks[0].vel -= AimDir * 5f;
        user.bodyChunks[1].vel -= AimDir * 3f;
    }

    protected override void ShootEffects()
    {
        var upDir = Custom.PerpendicularVector(AimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + AimDir * 35f, 60f, 1f, 4, Color.red));
        for (var i = 0; i < 8; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + LastAimDir * 25f, AimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }
}
