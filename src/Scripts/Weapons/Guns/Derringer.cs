using RWCustom;

namespace DMD;

public class Derringer : Gun
{
    public Derringer(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 1;
        ReloadSpeed = 50;
        FullClip = 2;
        DamageStat = 0.35f;
        GunSpriteName = "dmd_deagle"; //does more damage to players
        GunLength = 20;
        RandomSpreadStat = 0.8f;
        PipAngleDiff = 40;
        CheckIfArena(world);
    }

    protected override void Shoot(PhysicalObject user, Vector2 fireDir)
    {
        base.Shoot(user, fireDir);
        if (user is Scavenger)
        {
            FireDelay = 15;
        }
    }

    protected override void ShootSound()
    {
        room.PlaySound(Enums.Sounds.AKMShoot, bodyChunks[0], false, .38f + Random.value * .03f, 1.1f + Random.value * .2f);
    }

    protected override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var newBullet = new Bullet(user, firstChunk.pos + UpDir * 5f, (AimDir.normalized + (Random.insideUnitCircle * RandomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, DamageStat, 4.5f + 2f * DamageStat, 15f + 30f * DamageStat, false);
        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= AimDir * 3f;
        user.bodyChunks[1].vel -= AimDir * 3f;
    }

    protected override void ShootEffects()
    {
        var upDir = Custom.PerpendicularVector(AimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + AimDir * 35f, 75f, 1f, 5, Color.white));
        for (var i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + LastAimDir * 25f, AimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }
}
