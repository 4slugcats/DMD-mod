using RWCustom;

namespace DMD;

public class BFG : Gun
{
    public BFG(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 20;
        reloadSpeed = 120;
        fullClip = 1;
        damageStat = 3f;
        GunSpriteName = "BFG9000";
        gunLength = 87;
        randomSpreadStat = 0.2f;
        clipCost = 4;
        CheckIfArena(world);
    }

    public override void ShootSound()
    {
        room.PlaySound(SoundID.Slugcat_Terrain_Impact_Medium, firstChunk.pos, 6, 0.5f);
        //SoundHelper.PlayCustomSound("AK-47Shoot", bodyChunks[0], .55f + UnityEngine.Random.value * .02f, .9f + UnityEngine.Random.value * .1f);
    }

    public override void ShootEffects()
    {
        Vector2 upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 100f, 1f, 5, Color.red));
        for (int i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * UnityEngine.Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, UnityEngine.Random.value), null, 3, 8));
        }
    }

    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        AbstractPhysicalObject pipeAPO = new AbstractPhysicalObject(room.world, EnumExt_DragonSlayer.BFGOrb, null, this.abstractPhysicalObject.pos, room.world.game.GetNewID());
        pipeAPO.RealizeInRoom();
        BFGOrb orb = pipeAPO.realizedObject as BFGOrb;

        //dont let pebbels shoot it !!
        orb.firstChunk.pos = firstChunk.pos + aimDir * 5;
    }
}