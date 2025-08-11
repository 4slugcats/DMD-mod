using RWCustom;

namespace DMD;

public class Derringer : Gun
{
    public Derringer(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 1;
        reloadSpeed = 50;
        fullClip = 2;
        damageStat = 0.35f;
        GunSpriteName = "Derringer"; //does more damage to players
        gunLength = 20;
        randomSpreadStat = 0.8f;
        angleDiff = 40;
        CheckIfArena(world);
    }

    public override void Shoot(PhysicalObject user, Vector2 fireDir)
    {
        base.Shoot(user, fireDir);
        if (user is Scavenger)
        {
            fireDelay = 15;
        }
    }

    public override void ShootSound()
    {
        room.PlaySound(EnumExt_Snd.AK47Shoot, bodyChunks[0], false, .38f + Random.value * .03f, 1.1f + Random.value * .2f);
    }

    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var newBullet = new Bullet(user, firstChunk.pos + upDir * 5f, (aimDir.normalized + (Random.insideUnitCircle * randomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, damageStat, 4.5f + 2f * damageStat, 15f + 30f * damageStat, false);
        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= aimDir * 3f;
        user.bodyChunks[1].vel -= aimDir * 3f;
    }

    public override void ShootEffects()
    {
        var upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 75f, 1f, 5, Color.white));
        for (var i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }
}
