using RWCustom;

namespace DMD;

public class GrenadeLauncher : Gun
{
    public GrenadeLauncher(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        FireSpeed = 20;
        ReloadSpeed = 120;
        FullClip = 4;
        DamageStat = 3f;
        GunSpriteName = "dmd_devastator";
        GunLength = 87;
        RandomSpreadStat = 0.2f;
        ClipCost = 4;
        CheckIfArena(world);
    }

    public override void ShootSound()
    {
        room.PlaySound(SoundID.Slugcat_Terrain_Impact_Medium, firstChunk.pos, 6, 0.5f);
        //SoundHelper.PlayCustomSound("AK-47Shoot", bodyChunks[0], .55f + UnityEngine.Random.value * .02f, .9f + UnityEngine.Random.value * .1f);
    }

    public virtual void ShootEffects()
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
        var pipeAPO = new AbstractPhysicalObject(room.world, Enums.Guns.Pipe, null, abstractPhysicalObject.pos, room.world.game.GetNewID());
        pipeAPO.RealizeInRoom();
        var newPipe = pipeAPO.realizedObject as Pipe;

        //dont let pebbels shoot it !!
        newPipe.firstChunk.pos = firstChunk.pos + AimDir * 5;
        newPipe.InitiateBurn();
        if (AimDir.y > -.4)
        {
            newPipe.firstChunk.vel = AimDir * 15f;
            newPipe.firstChunk.vel.y += 10;
        }
        else
        {
            newPipe.firstChunk.vel = AimDir * 8f + new Vector2((user as Player).ThrowDirection * 6, 0);
        }

        RelatedObjects.Add(newPipe);
    }

    public override void NewRoom(Room newRoom)
    {
        foreach (Pipe p in RelatedObjects)
        {
            p.Pipe_Explode(null);
        }
        base.NewRoom(newRoom);
    }

    public override void Destroy()
    {
        foreach (Pipe p in RelatedObjects)
        {
            p.Pipe_Explode(null);
        }
        base.Destroy();
    }
}
