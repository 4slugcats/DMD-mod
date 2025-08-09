using RWCustom;

namespace DMD;

public class AK47 : Gun
{
    public AK47(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 6;
        reloadSpeed = 35;
        fullClip = 15;
        damageStat = 0.3f;
        automatic = true;
        GunSpriteName = "AK-47";
        gunLength = 60;
        randomSpreadStat = 1.4f;
        angleDiff = 19;
        clipCost = 1;
        CheckIfArena(world);
    }

    public override void ShootSound()
    {
        room.PlaySound(EnumExt_Snd.AK47Shoot, bodyChunks[0], false, .36f + UnityEngine.Random.value * .02f, 1.05f + UnityEngine.Random.value * .2f);
    }
z
    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        Bullet newBullet = new Bullet(user, user.abstractPhysicalObject.world, firstChunk.pos + upDir * 5f, (aimDir.normalized + (UnityEngine.Random.insideUnitCircle * randomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, damageStat, 4.5f + 2f * damageStat, 15f + 30f * damageStat, false);
        room.AddObject(newBullet);
        newBullet.Fire();
        user.bodyChunks[0].vel -= aimDir * 2f;
        user.bodyChunks[1].vel -= aimDir * 2f;
    }

    public override void ShootEffects()
    {
        Vector2 upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 60f, 1f, 4, Color.yellow));
        for (int i = 0; i < 2; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * UnityEngine.Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, UnityEngine.Random.value), null, 3, 8));
        }
    }
}
