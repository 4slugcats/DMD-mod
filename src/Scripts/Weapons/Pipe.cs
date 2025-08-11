using Noise;
using RWCustom;
using Smoke;

namespace DMD;

class Pipe : ScavengerBomb
{
    public Pipe(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        bounce = .3f;
        gravity = .8f;
        rotationSpeed = (Random.value * .5f) / 10;
    }
    bool Stuck = false;
    Vector2 stuckPos;

    public void Pipe_InitiateBurn()
    {
        if (burn == 0f)
        {
            burn = 21400000f;
            firstChunk.vel += Custom.RNV() * Random.value * 6f;
        }
        else
        {
            burn = Mathf.Min(burn, 4);
        }
    }

    public void Pipe_Explode(BodyChunk hitChunk)
    {
        if (slatedForDeletetion)
        {
            return;
        }
        var vector = Vector2.Lerp(firstChunk.pos, firstChunk.lastPos, 0.35f);
        room.AddObject(new SootMark(room, vector, 60f, true));
        room.AddObject(new StickyExplosion(room, this, vector, 7, 200f, 5.2f, 1.4f, 200f, 0.2f, thrownBy, 0.7f, 160f, 1f));
        room.AddObject(new Explosion.ExplosionLight(vector, 230f, .7f, 5, explodeColor));
        room.AddObject(new Explosion.ExplosionLight(vector, 180f, .7f, 3, new Color(1f, 1f, 1f)));
        room.AddObject(new ExplosionSpikes(room, vector, 18, 30f, 9f, 7f, 170f, explodeColor));
        room.AddObject(new ShockWave(vector, 280f, 0.045f, 5));
        for (var i = 0; i < 25; i++)
        {
            var a = Custom.RNV();
            if (room.GetTile(vector + a * 20f).Solid)
            {
                if (!room.GetTile(vector - a * 20f).Solid)
                {
                    a *= -1f;
                }
                else
                {
                    a = Custom.RNV();
                }
            }
            for (var j = 0; j < 3; j++)
            {
                room.AddObject(new Spark(vector + a * Mathf.Lerp(30f, 60f, Random.value), 2 * a * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 20f * Random.value, Color.Lerp(explodeColor, new Color(1f, 1f, 1f), Random.value), null, 11, 28));
            }
            for (var k = 0; k < 6; k++)
            {
                room.AddObject(new CollectToken.TokenSpark(vector + Custom.RNV() * Mathf.Lerp(30f, 60f, Random.value), 2 * Custom.RNV() * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 15f * Random.value, explodeColor, false));
            }
            room.AddObject(new Explosion.FlashingSmoke(vector + a * 40f * Random.value, 2 * a * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + 0.05f * Random.value, new Color(1f, 1f, 1f), explodeColor, Random.Range(3, 11)));
        }
        if (smoke != null)
        {
            for (var k = 0; k < 8; k++)
            {
                smoke.EmitWithMyLifeTime(vector + Custom.RNV(), Custom.RNV() * Random.value * 17f);
            }
        }
        for (var l = 0; l < 6; l++)
        {
            room.AddObject(new BombFragment(vector, Custom.DegToVec(((float)l + Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, Random.value)));
        }
        room.ScreenMovement(new Vector2?(vector), default(Vector2), 1.3f);
        for (var m = 0; m < abstractPhysicalObject.stuckObjects.Count; m++)
        {
            abstractPhysicalObject.stuckObjects[m].Deactivate();
        }
        room.PlaySound(SoundID.Bomb_Explode, vector, .8f, 1.1f + .3f * Random.value);
        room.InGameNoise(new InGameNoise(vector, 18000f, this, 1f));
        var flag = hitChunk != null;
        for (var n = 0; n < 5; n++)
        {
            if (room.GetTile(vector + Custom.fourDirectionsAndZero[n].ToVector2() * 20f).Solid)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            if (smoke == null)
            {
                smoke = new BombSmoke(room, vector, null, explodeColor);
                room.AddObject(smoke);
            }
            if (hitChunk != null)
            {
                smoke.chunk = hitChunk;
            }
            else
            {
                smoke.chunk = null;
                smoke.fadeIn = 1f;
            }
            smoke.pos = vector;
            smoke.stationary = true;
            smoke.DisconnectSmoke();
        }
        else if (smoke != null)
        {
            smoke.Destroy();
        }
        Destroy();
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        if (firstContact)
        {
            if (speed * bodyChunks[chunk].mass > 7f)
            {
                room.ScreenMovement(new Vector2?(bodyChunks[chunk].pos), Custom.IntVector2ToVector2(direction) * speed * bodyChunks[chunk].mass * 0.1f, Mathf.Max((speed * bodyChunks[chunk].mass - 30f) / 50f, 0f));
            }
            if (speed > 4f && speed * bodyChunks[chunk].loudness * Mathf.Lerp(bodyChunks[chunk].mass, 1f, 0.5f) > 0.5f)
            {
                room.InGameNoise(new InGameNoise(bodyChunks[chunk].pos + IntVector2.ToVector2(direction) * bodyChunks[chunk].rad * 0.9f, Mathf.Lerp(350f, Mathf.Lerp(100f, 1500f, Mathf.InverseLerp(0.5f, 20f, speed * bodyChunks[chunk].loudness * Mathf.Lerp(bodyChunks[chunk].mass, 1f, 0.5f))), 0.5f), this, 4f));
            }
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (Stuck)
        {
            ChangeCollisionLayer(2);
            firstChunk.rad = 0;
            CollideWithTerrain = false;
            gravity = 0;
            firstChunk.vel = Vector2.zero;
            firstChunk.pos = stuckPos;
            rotationSpeed = 0;
        }

        if (firstChunk.contactPoint != null && firstChunk.contactPoint != new IntVector2(0, 0))
        {
            Stuck = true;
            stuckPos = firstChunk.pos;
        }
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj == null)
        {
            return false;
        }
        ChangeMode(Mode.Free);
        firstChunk.vel = firstChunk.vel * -0.2f;
        SetRandomSpin();
        if (result.obj is Creature)
        {
            (result.obj as Creature).Violence(firstChunk, new Vector2?(firstChunk.vel * firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, 0.1f, 10f);
            room.PlaySound(SoundID.Rock_Hit_Creature, firstChunk);
        }
        else if (result.chunk != null)
        {
            result.chunk.vel += firstChunk.vel * firstChunk.mass / result.chunk.mass;
        }
        else if (result.onAppendagePos != null)
        {
            (result.obj as IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, firstChunk.vel * firstChunk.mass);
        }
        if (!ignited)
        {
            InitiateBurn();
        }

        return true;
    }

    public override void WeaponDeflect(Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        firstChunk.pos = Vector2.Lerp(firstChunk.pos, inbetweenPos, 0.5f);
        vibrate = 20;
        ChangeMode(Mode.Free);
        firstChunk.vel = deflectDir * bounceSpeed * 0.5f;
        if (!ignited)
        {
            InitiateBurn();
        }

        SetRandomSpin();
    }

    //public override void HitByExplosion(float hitFac, Explosion explosion, int hitChunk)
    //{

    //}

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Sticky", true);
        sLeaser.sprites[0].color = Color.white;
        sLeaser.sprites[0].scale = 0.6f;

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var position = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        if (vibrate > 0)
        {
            position += Custom.DegToVec(Random.value * 360f) * 2f * Random.value;
        }

        Vector2 rotation = Vector3.Slerp(lastRotation, this.rotation, timeStacker);
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rotation);
        sLeaser.sprites[0].SetPosition(position - camPos);
        sLeaser.sprites[0].scale = 0.4f;

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

}
