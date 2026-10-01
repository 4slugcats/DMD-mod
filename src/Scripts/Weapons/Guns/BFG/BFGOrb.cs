using RWCustom;

namespace DMD;

public class BFGOrb : UpdatableAndDeletable, IDrawable
{
    AbstractCreature owner;
    //0 default, 1 contact made, 2 explosion triggered
    int Contact = 0;
    bool WMDMODE = false;
    public BFGOrb(PhysicalObject Gun, AbstractCreature owner, Vector2 vel, Vector2 StartingPos, bool WMDMODE)
    {
        this.vel = vel * .8f * (WMDMODE ? 2f : 1f);
        Pos = StartingPos;
        lastPos = StartingPos;
        this.owner = owner;
        HitTargets = new List<AbstractCreature>();
        gun = Gun;
        this.WMDMODE = WMDMODE;
    }
    float age = 1;
    Vector2 Pos;
    Vector2 lastPos;
    Vector2 vel;
    PhysicalObject gun;
    //fizzle will destroy the orb when it reaches 1, caused by being too old or hitting a surface
    float fizzle = 0;
    public List<AbstractCreature> HitTargets;

    Color color = new Color(.1f, 1f, .3f, 1f);
    public override void Update(bool eu)
    {
        age++;
        Pos += vel * Mathf.Pow(Mathf.Lerp(1.5f, 0.3f, age / 300), 2f);
        if (room.GetTile(Pos).Solid || Contact == 2)
        {
            vel *= .8f;
            fizzle += 0.04f;
        }
        if (age > 400)
        {
            fizzle += 0.01f;
        }
        if (fizzle > 1f)
        {
            slatedForDeletetion = true;
        }
        //damage tracing code happens every 1/4 of a second to be less performance intensive
        if (age % 10 == 0)
        {
            BFGDamageScan(WMDMODE ? 1 : 0);
            if (Contact == 1)
            {
                room.AddObject(new SootMark(room, Pos, 80f, true));
                room.AddObject(new Explosion(room, gun, Pos, 7, 235f, 3.1f, 2f, 240f, 0.15f, owner.realizedCreature, 0.75f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(Pos, 340f, 1f, 15, color));
                room.AddObject(new Explosion.ExplosionLight(Pos, 315f, 1f, 11, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, Pos, 12, 30f, 9f, 5f, 120f, color));
                room.AddObject(new ShockWave(Pos, 220f, 0.045f, 5, false));
                room.PlaySound(SoundID.Bomb_Explode, Pos, 1.2f, 1.23f);
                Contact = 2;
                if (WMDMODE)
                {
                    BFGDamageScan(2);
                }
            }
            base.Update(eu);
            lastPos = Pos;
        }
    }
    //0- normal, 1-wmd op mode, 2-kills all creatures except owner
    public void BFGDamageScan(int mode)
    {
        float range;
        switch (mode)
        {
            default:
                range = Mathf.Lerp(380f, 220f, age / 150f);
                break;
            case 1:
                range = Mathf.Lerp(480f, 320f, age / 180f);
                break;
            case 2:
                range = 16000f;
                break;


        }
        for (int x = 0; x < room.abstractRoom.creatures.Count; x++)
        {


            if (room.abstractRoom.creatures[x].realizedCreature != null && room.abstractRoom.creatures[x] != owner)
            {
                if (owner == null)
                {
                    Debug.Log("owner null");
                }
                Creature real = room.abstractRoom.creatures[x].realizedCreature;

                for (int b = 0; b < real.bodyChunks.Length; b++)
                {
                    if (Custom.DistLess(Pos, real.bodyChunks[b].pos, range))
                    {
                        if (mode > 0 || room.VisualContact(Pos, real.bodyChunks[b].pos))
                        {
                            if (Custom.DistLess(Pos, real.bodyChunks[b].pos, 10f * (mode+1)) && Contact == 0)
                            {
                                Contact = 1;
                            }
                            if (!HitTargets.Contains(room.abstractRoom.creatures[x]))
                            {
                                HitTargets.Add(room.abstractRoom.creatures[x]);
                                room.AddObject(new BFGDamageBall(gun, owner, real.bodyChunks[b].pos, real.bodyChunks[b].rad, mode > 0 ? real : null));

                            }
                        }
                    }
                }

            }
        }


    }
    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[0]);
        rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[1]);
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];

        sLeaser.sprites[0] = new FSprite("dmd_bfg_orb");
        sLeaser.sprites[1] = new FSprite("Futile_White");
        sLeaser.sprites[1].shader = rCam.game.rainWorld.Shaders["FlatLight"];
        sLeaser.sprites[1].color = color;

        AddToContainer(sLeaser, rCam, null!);
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) { }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //sLeaser.sprites[0].SetElementByName("BFGOrb" + (age % 5 == 0 ? 1 : 0));
        if (fizzle >= 1f)
        {
            sLeaser.sprites[0].isVisible = false;
            sLeaser.sprites[1].isVisible = false;
        }
        Vector2 drawpos = Vector2.Lerp(Vector2.Lerp(lastPos,Pos,WMDMODE ? 0.85f : 0), Pos, timeStacker);
        sLeaser.sprites[0].x = drawpos.x - camPos.x;
        sLeaser.sprites[0].y = drawpos.y - camPos.y;
        sLeaser.sprites[1].x = drawpos.x - camPos.x;
        sLeaser.sprites[1].y = drawpos.y - camPos.y;

        if (age % 5 == 0)
        {
            sLeaser.sprites[0].rotation = Random.value * 360f;
        }
        float ScaleFlux = Mathf.Lerp(1.4f, 1.8f, Mathf.Sin(Mathf.PI * ((age % 60f) / 60f))) * (WMDMODE? 1.5f : 1f);

        sLeaser.sprites[0].scale = ScaleFlux;
        sLeaser.sprites[1].scale = ScaleFlux * 6f;
        sLeaser.sprites[1].alpha = Mathf.Lerp(Mathf.Lerp(0.2f, 0.6f, ScaleFlux * .5f), 0f, fizzle);
        sLeaser.sprites[0].alpha = Mathf.Lerp(0.9f, 0.0f, fizzle);
        //sLeaser.sprites[1].SetPosition(firstChunk.pos - camPos);
    }
}
public class BFGDamageBall : UpdatableAndDeletable, IDrawable
{
    public BFGDamageBall(PhysicalObject Gun, AbstractCreature owner, Vector2 Pos, float BCsize)
    {
        this.Pos = Pos;
        this.owner = owner;
        Gun = gun;
        BallSize = Mathf.Clamp(Mathf.Lerp(1.3f, BCsize * .3f, .5f), 2f, 6f);
    }
    public BFGDamageBall(PhysicalObject Gun, AbstractCreature owner, Vector2 Pos, float BCsize, Creature Target)
    {
        this.Pos = Pos;
        this.owner = owner;
        Gun = gun;
        BallSize = Mathf.Clamp(Mathf.Lerp(1.3f, BCsize * .3f, .5f), 2f, 6f);
        KillThisGuy = Target;
    }
    Creature KillThisGuy = null;
    float BallSize;
    PhysicalObject gun;
    AbstractCreature owner;
    float age = 1;
    Vector2 Pos;
    //fizzle will destroy the orb when it reaches 1, caused by being too old or hitting a surface
    float fizzle = 0.99f;

    Color Color = new Color(.2f, 1f, .4f);

    public override void Update(bool eu)
    {
        age++;
        if (age > 20)
        {
            fizzle += 0.04f;
        }
        else
        {
            fizzle = Mathf.Max(fizzle - 0.04f, 0f);
        }
        if (age == 20)
        {
            room.AddObject(new SootMark(room, Pos, 80f, true));
            room.AddObject(new Explosion(room, gun, Pos, 7, 85f, 3.1f, 2f, 140f, 0.05f, owner.realizedCreature, 0.75f, 40f, 0.5f));
            room.AddObject(new Explosion.ExplosionLight(Pos, 140f, 1f, 15, Color));
            room.AddObject(new Explosion.ExplosionLight(Pos, 115f, 1f, 11, new Color(1f, 1f, 1f)));
            room.AddObject(new ExplosionSpikes(room, Pos, 12, 30f, 9f, 5f, 90f, Color));
            room.AddObject(new ShockWave(Pos, 80f, 0.035f, 5, false));
            room.PlaySound(SoundID.Bomb_Explode, Pos, 0.5f, 1.5f);
            if (KillThisGuy != null)
            {
                KillThisGuy.Die();
                if (owner != null)
                {
                    KillThisGuy.killTag = owner;
                }

            }

        }
        if (fizzle > 1f)
        {
            slatedForDeletetion = true;
        }

        base.Update(eu);
    }
    bool switchSprite = true;
    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[0]);
        rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[1]);
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];

        sLeaser.sprites[0] = new FSprite("dmd_bfg_ballA");
        sLeaser.sprites[1] = new FSprite("Futile_White");
        sLeaser.sprites[1].shader = rCam.game.rainWorld.Shaders["FlatLight"];
        sLeaser.sprites[1].color = Color;

        AddToContainer(sLeaser, rCam, null!);
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) { }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //sLeaser.sprites[0].SetElementByName("BFGOrb" + (age % 5 == 0 ? 1 : 0));
        if (fizzle >= 1f)
        {
            sLeaser.sprites[0].isVisible = false;
            sLeaser.sprites[1].isVisible = false;
        }
        sLeaser.sprites[0].x = Pos.x - camPos.x;
        sLeaser.sprites[0].y = Pos.y - camPos.y;
        sLeaser.sprites[1].x = Pos.x - camPos.x;
        sLeaser.sprites[1].y = Pos.y - camPos.y;

        if (age % 5 == 0)
        {
            sLeaser.sprites[0].rotation = Random.value * 360f;
            switchSprite = !switchSprite;
        }
        float ScaleFlux = Mathf.Lerp(BallSize * .9f, BallSize, Mathf.Sin(Mathf.PI * ((age % 20f) / 20f)));

        sLeaser.sprites[0].scale = ScaleFlux;
        sLeaser.sprites[0].alpha = Mathf.Lerp(0.9f, 0.0f, fizzle);
        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("dmd_bfg_ball" + (switchSprite ? "A" : "B"));
        sLeaser.sprites[1].scale = ScaleFlux * 2.3f;
        sLeaser.sprites[1].alpha = Mathf.Lerp(Mathf.Lerp(0.4f, 0.6f, ScaleFlux), 0f, fizzle);
    }
}
