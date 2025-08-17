using RWCustom;
using Smoke;

namespace DMD;

public class GunSmolder : PositionedSmokeEmitter
{
    public BodyChunk chunk;
    public int life;
    public PhysicalObject.Appendage.Pos appendagePos;

    public GunSmolder(Room room, Vector2 pos, BodyChunk chunk, PhysicalObject.Appendage.Pos appendagePos) : base(SmokeType.Smolder, room, pos, 2, 3f, true, 15f, 15)
    {
        this.chunk = chunk;
        this.appendagePos = appendagePos;
        life = Random.Range(50, 100);
        objectWind = 1f;
    }

    public override float ParticleLifeTime => Mathf.Lerp(50f, 100f, Random.value);

    public override int PushApartSegments => 0;

    public override bool ObjectAffectWind(PhysicalObject obj)
    {
        return chunk == null || obj != chunk.owner;
    }

    public override SmokeSystemParticle? AddParticle(Vector2 emissionPoint, Vector2 emissionForce, float lifeTime)
    {
        if (room?.PointSubmerged(emissionPoint) ?? false)
        {
            if (Random.value < 0.1f)
            {
                room.AddObject(new Bubble(emissionPoint, emissionForce, false, false));
            }
            life--;
            return null;
        }
        var smokeSystemParticle = base.AddParticle(emissionPoint, emissionForce, lifeTime);

        if (smokeSystemParticle != null)
        {
            smokeSystemParticle.life = Mathf.InverseLerp(0f, 60f, (float)life);
            smokeSystemParticle.lastLife = smokeSystemParticle.life;
        }
        return smokeSystemParticle;
    }

    public override void Update(bool eu)
    {
        if (chunk != null)
        {
            pos = chunk.pos;
            if (appendagePos != null)
            {
                pos = appendagePos.appendage.OnAppendagePosition(appendagePos);
            }
            if (chunk.owner.room != room)
            {
                Destroy();
            }
        }
        base.Update(eu);
        life--;
        if (life < 1)
        {
            Destroy();
        }
    }

    public override SmokeSystemParticle CreateParticle()
    {
        return new SmolderSegment();
    }

    public class SmolderSegment : SmokeSegment
    {
        private Color colorA;
        private Color colorB;

        public override void Update(bool eu)
        {
            base.Update(eu);
            vel.y = vel.y + 0.2f * Mathf.InverseLerp(0.9f, 1f, life);
        }

        public override float ConDist(float timeStacker)
        {
            return Mathf.Lerp(0.6f, 0.01f, Mathf.Lerp(lastLife, life, timeStacker));
        }

        public override void WindAndDrag(Room rm, ref Vector2 v, Vector2 p)
        {
            v *= Custom.LerpMap(v.magnitude, 5f, 20f, 0.97f, 0.4f);
            v.y += 0.02f;
            v += PerlinWind(p, rm);
            if (rm.readyForAI && rm.aimap.getTerrainProximity(p) < 3)
            {
                var terrainProximity = rm.aimap.getTerrainProximity(p);
                Vector2 vector = default;
                for (var i = 0; i < 8; i++)
                {
                    if (rm.aimap.getTerrainProximity(p + Custom.eightDirections[i].ToVector2() * 20f) > terrainProximity)
                    {
                        vector += Custom.eightDirections[i].ToVector2();
                    }
                }
                v += Vector2.ClampMagnitude(vector, 1f) * 0.015f;
            }
        }

        public override float MyRad(float timeStacker)
        {
            return Mathf.Lerp(5f, 1f, Mathf.Lerp(lastLife, life, timeStacker)) * Mathf.Lerp(Custom.LerpMap(Vector2.Distance(Vector2.Lerp(lastPos, pos, timeStacker), NextPos(timeStacker)), ConDist(timeStacker) / 0.03f, ConDist(timeStacker) * 500f, 3f, 0.2f, 0.2f), 1f, life) * (4f - 3f * Mathf.Pow(MyOpactiy(timeStacker), 0.5f)) * 2f;
        }

        public override float MyOpactiy(float timeStacker)
        {
            if (resting)
            {
                return 0f;
            }
            return Mathf.Pow(Mathf.InverseLerp(0f, 0.9f, Mathf.Lerp(lastLife, life, timeStacker)), 2f) * Mathf.Lerp(Custom.LerpMap(Vector2.Distance(Vector2.Lerp(lastPos, pos, timeStacker), NextPos(timeStacker)), ConDist(timeStacker) * 1.2f, ConDist(timeStacker) * 500f, 1f, 0.2f, 1.5f), 1f, Mathf.InverseLerp(0.9f, 1f, Mathf.Lerp(lastLife, life, timeStacker))) * 1.25f;
        }

        public override Color MyColor(float timeStacker)
        {
            return Color.Lerp(colorB, colorA, Mathf.Pow(Mathf.Lerp(lastLife, life, timeStacker) * 3f, 3f));
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette);
            colorA = palette.blackColor;
            colorB = Color.Lerp(palette.blackColor, palette.fogColor, 0.2f * (1f - palette.darkness));
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["SmokeTrail"];
        }
    }
}
