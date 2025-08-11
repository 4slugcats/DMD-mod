using RWCustom;

namespace DMD;

public class GrenadeLauncher : Gun
{
    public GrenadeLauncher(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        fireSpeed = 20;
        reloadSpeed = 120;
        fullClip = 4;
        damageStat = 3f;
        GunSpriteName = "RocketLauncher";
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
        var upDir = Custom.PerpendicularVector(aimDir);
        if (upDir.y < 0)
        {
            upDir *= -1f;
        }
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + aimDir * 35f, 100f, 1f, 5, Color.red));
        for (var i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + lastAimDir * 25f, aimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }
    }

    public override void SummonProjectile(PhysicalObject user, bool boostAccuracy)
    {
        var pipeAPO = new AbstractPhysicalObject(room.world, EnumExt_DragonSlayer.Pipe, null, abstractPhysicalObject.pos, room.world.game.GetNewID());
        pipeAPO.RealizeInRoom();
        var newPipe = pipeAPO.realizedObject as Pipe;

        //dont let pebbels shoot it !!
        newPipe.firstChunk.pos = firstChunk.pos + aimDir * 5;
        newPipe.InitiateBurn();
        if (aimDir.y > -.4)
        {
            newPipe.firstChunk.vel = aimDir * 15f;
            newPipe.firstChunk.vel.y += 10;
        }
        else
        {
            newPipe.firstChunk.vel = aimDir * 8f + new Vector2((user as Player).ThrowDirection * 6, 0);
        }

        relatedObjects.Add(newPipe);
    }

    public override void NewRoom(Room newRoom)
    {
        foreach (Pipe p in relatedObjects)
        {
            p.Pipe_Explode(null);
        }
        base.NewRoom(newRoom);
    }

    public override void Destroy()
    {
        foreach (Pipe p in relatedObjects)
        {
            p.Pipe_Explode(null);
        }
        base.Destroy();
    }
}
