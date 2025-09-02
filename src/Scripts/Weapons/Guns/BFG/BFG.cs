using RWCustom;

namespace DMD;

public class BFG : Gun
{
    public BFG(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 20;
        ReloadSpeed = 120;
        FullClip = 1;
        DamageStat = 3f;
        GunSpriteName = "dmd_bfg";
        GunLength = 87;
        RandomSpreadStat = 0.2f;
        ClipCost = 4;
        CheckIfArena(world);
    }

    protected override void ShootSound()
    {
        room.PlaySound(SoundID.Slugcat_Terrain_Impact_Medium, firstChunk.pos, 6, 0.5f);
        //SoundHelper.PlayCustomSound("AK-47Shoot", bodyChunks[0], .55f + UnityEngine.Random.value * .02f, .9f + UnityEngine.Random.value * .1f);
    }

    protected override void ShootEffects()
    {
        var upDir = Custom.PerpendicularVector(AimDir);

        if (upDir.y < 0)
        {
            upDir *= -1f;
        }

        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + AimDir * 35f, 100f, 1f, 5, Color.red));

        for (var i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + LastAimDir * 25f, AimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }

    protected override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var bfgOrbApo = new AbstractPhysicalObject(room.world, Enums.Guns.Projectiles.BFGOrb, null, abstractPhysicalObject.pos, room.world.game.GetNewID());

        bfgOrbApo.RealizeInRoom();

        var orb = (BFGOrb)bfgOrbApo.realizedObject;

        //dont let pebbels shoot it !!
        orb.firstChunk.pos = firstChunk.pos + AimDir * 5;
        orb.firstChunk.vel = AimDir * 5.0f;
    }
}
