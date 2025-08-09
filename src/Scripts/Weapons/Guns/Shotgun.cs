using RWCustom;

namespace DMD;

public class Shotgun : Gun
{
    public Shotgun(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 30;
        reloadSpeed = 64;
        fullClip = 3;
        damageStat = 1.8f;
        GunSpriteName = "Shotty";
        gunLength = 47;
        randomSpreadStat = 5f;
        angleDiff = 30;
        clipCost = 2;
        CheckIfArena(world);
    }

    public override void ShootSound()
    {
        room.PlaySound(EnumExt_Snd.AK47Shoot, bodyChunks[0], false, .5f + UnityEngine.Random.value * .03f, 1f + UnityEngine.Random.value * .07f);
        room.PlaySound(EnumExt_Snd.AK47Shoot, bodyChunks[0], false, .45f + UnityEngine.Random.value * .03f, .9f + UnityEngine.Random.value * .07f);
    }

    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        int mult = 6;
        for (int i = mult; i > 0; i--)
        {
            Bullet newBullet = new Bullet(user, firstChunk.pos + upDir * 5f, (aimDir.normalized + (UnityEngine.Random.insideUnitCircle * randomSpreadStat * (boostAccuracy ? 0.3f : 1f)) * .045f).normalized, damageStat / mult, 1.5f + 2f * damageStat / mult, 15f + 30f * damageStat / mult, false);
            room.AddObject(newBullet);
            newBullet.Fire();
        }
        user.bodyChunks[0].vel -= aimDir * 5f;
        user.bodyChunks[1].vel -= aimDir * 3f;
    }

    public override void ShootEffects()
    {
        Vector2 upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 60f, 1f, 4, Color.red));
        for (int i = 0; i < 8; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * UnityEngine.Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, UnityEngine.Random.value), null, 3, 8));
        }
    }
}