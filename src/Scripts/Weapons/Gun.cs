using RWCustom;

namespace DMD;

public abstract class Gun : Weapon
{
    protected Creature Owner { get; set; } = null!;
    public GunSmolder Smolder { get; set; } = null!;
    private bool SmokeFromShot { get; set; }

    public Vector2 AimDir { get; set; }
    protected Vector2 LastAimDir { get; set; }

    protected int FireDelay { get; set; }
    private bool JustShot { get; set; }
    private int TimeFromLastShotAttempt { get; set; }
    private bool IsTriggerReleased { get; set; } = true;

    public bool IsFlipped { get; set; }
    public bool AutoFlip { get; set; }

    protected int OwnerAge { get; set; }

    protected int FirstPipAngle { get; set; }
    protected int PipAngleDiff { get; set; } = 24;

    // Stats
    protected string GunSpriteName { get; set; } = "";

    protected int GunLength { get; set; }
    protected bool Automatic { get; set; }
    protected int FireSpeed { get; set; }

    protected int FullClip { get; set; }
    public int ClipCost { get; set; }

    private int ReloadTime { get; set; }
    protected int ReloadSpeed { get; set; }

    protected float DamageStat { get; set; }
    protected float RandomSpreadStat { get; set; }

    protected List<PhysicalObject> RelatedObjects { get; } = [];

    public Vector2 UpDir
    {
        get
        {
            var dir = Custom.PerpendicularVector(AimDir);
            if (dir.y < 0)
            {
                dir *= -1f;
            }
            return dir;
        }
    }

    protected int Clip
    {
        get => abstractPhysicalObject.ID.number;
        set => abstractPhysicalObject.ID.number = value;
    }

    public Gun(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
        bodyChunkConnections = [];

        AimDir = Vector2.one;

        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.4f;
        surfaceFriction = 0.4f;
        collisionLayer = 2;
        waterFriction = 0.98f;
        buoyancy = 0.4f;

        AimDir = AimDir with { x = (Random.value < 0 ? 1f : -1f) };
        AimDir = AimDir with { y = (Random.value * 2f - 1f) * .2f };

        LastAimDir = AimDir;
        AutoFlip = true;
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        abstractPhysicalObject.ID.number = FullClip;
        firstChunk.pos = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);
        firstChunk.lastPos = firstChunk.pos;
    }

    public override void Update(bool eu)
    {
        // TODO
        // if (firstupdate && owner is Player p && HHUtils.IsMe(p))
        // {
        //     Clip = HHUtils.inventories.GetOrCreateValue(p).ownedBullets[TypeToIndex(abstractPhysicalObject.type) % 4];
        // }

        JustShot = false;
        AimDir.Normalize();
        LastAimDir = AimDir;
        if (AutoFlip)
        {
            if (IsFlipped && AimDir.x < -.5)
            {
                IsFlipped = false;
            }
            else if (!IsFlipped && AimDir.x > .5)
            {
                IsFlipped = true;
            }
        }
        if (ReloadTime > 0)
        {
            if (ReloadTime == ReloadSpeed - 7)
            {
                room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, firstChunk.pos + AimDir * 25f, 0.5f, 1.25f);
            }
            if (ReloadTime % 11 == 1)
            {
                room.PlaySound(SoundID.Seed_Cob_Pop, firstChunk.pos + AimDir * 25f, 1f, .875f);
            }
            if (ReloadTime == 1)
            {
                room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, firstChunk.pos + AimDir * 25f, 0.8f, 1.4f);
                Clip = FullClip;

                // TODO
                // if (HHUtils.IsMe(owner))
                // {
                //     HHUtils.inventories.GetOrCreateValue(owner as Player).ownedClips -= clipCost;
                // }
            }
            ReloadTime--;

        }
        if (FireDelay > 0)
        {
            FireDelay--;
            if (SmokeFromShot)
            {
                if (Smolder == null)
                {
                    Smolder = new GunSmolder(room, firstChunk.pos + UpDir * 5f + AimDir * (GunLength / 2f), null, null);
                    room.AddObject(Smolder);
                }
                Smolder.life = 100;
                for (var i = 0; i < 3; i++)
                {
                    Smolder.AddParticle(Smolder.pos + UpDir * 5f + AimDir * GunLength / 2f, AimDir * (10f + 30f * Random.value) + Random.insideUnitCircle * 14f, 30f);
                }
            }
        }
        if (Smolder != null)
        {
            Smolder.pos = firstChunk.pos + UpDir * 5f + AimDir * (GunLength / 2f);
            if (Smolder.slatedForDeletetion)
            {
                Smolder = null;
            }
        }

        IsTriggerReleased = TimeFromLastShotAttempt > 1;
        TimeFromLastShotAttempt++;

        if (mode == Mode.Free)
        {
            Owner = null;
        }
        if (Owner != null)
        {
            OwnerAge++;
        }

        base.Update(eu);
    }

    public override void Grabbed(Creature.Grasp grasp)
    {
        //Debug.Log("gun grabbed");
        if (grasp?.grabber == null)
        {
            return;
        }

        //Debug.Log("grabber isnt null");
        Owner = grasp.grabber;
        //Debug.Log("grabber is " + owner.GetType().Name);
        ChangeMode(Mode.Carried);
        base.Grabbed(grasp);

        // if (HHUtils.IsMe(owner) && TypeToIndex(abstractPhysicalObject.type) > 3 && !HHUtils.inventories.GetOrCreateValue(owner as Player).ownedGuns[TypeToIndex(abstractPhysicalObject.type) - 4])
        // {
        //     HHUtils.inventories.GetOrCreateValue(owner as Player).Upgrade(this);
        // }
    }


    public void TryShoot(PhysicalObject user, Vector2 fireDir, bool wantsandcanreload)
    {
        // if(user is Player p && HHUtils.IsMe(p) && HHUtils.inventories.GetOrCreateValue(p).active)
        // {
        //     return;
        // }

        Vector2 targetCoord;
        var right = fireDir.x > 0;
        var pos = firstChunk.pos;
        var recordDist = float.PositiveInfinity;
        BodyChunk? recordChunk = null;

        //Debug.Log("values set");

        foreach (var testObject in room.physicalObjects[1].Where(x => x is Creature c && !(c == user || c.dead))) //1 represents the main collision layer
        {
            foreach (var chunk in testObject.bodyChunks)
            {
                if (Mathf.Abs(chunk.pos.y - firstChunk.pos.y) <= 30 && (chunk.pos.x > firstChunk.pos.x == right) && (Mathf.Abs(chunk.pos.x - pos.x) < recordDist))
                {
                    recordDist = Mathf.Abs(chunk.pos.x - pos.x);
                    recordChunk = chunk;
                }
            }
        }

        if (recordChunk != null)
        {
            targetCoord = recordChunk.pos - firstChunk.pos;
        }
        else
        {
            targetCoord = fireDir;
        }

        TimeFromLastShotAttempt = 0;

        if (FireDelay == 0 && Clip == 0 && !JustShot && (ReloadTime == 0 || ReloadTime == ReloadSpeed))
        {
            SmokeFromShot = false;
            FireDelay = FireSpeed;
            Reload(wantsandcanreload);
        }
        if (FireDelay == 0 && Clip > 0)
        {
            Shoot(user, targetCoord);
        }
    }

    protected virtual void Shoot(PhysicalObject user, Vector2 fireDir)
    {
        Clip--;

        SmokeFromShot = true;
        FireDelay = FireSpeed;
        JustShot = true;
        AimDir = fireDir.normalized;

        if (user is Player pla && pla.input[0].y < -.35f)
        {
            AimDir = new Vector2(0, -1);
        }

        var boostAccuracy = user is Player p && (p.animation == Player.AnimationIndex.Flip || p.bodyMode == Player.BodyModeIndex.Crawl);

        var upDir = Custom.PerpendicularVector(AimDir);

        if (upDir.y < 0)
        {
            upDir *= -1f;
        }

        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos + upDir * 5f + AimDir * 35f, 75f, 1f, 5, Color.white));

        for (var i = 0; i < 3; i++)
        {
            room.AddObject(new Spark(firstChunk.pos + upDir * 5f + LastAimDir * 25f, AimDir * 50f * Random.value + Custom.RNV() * 1.5f, Color.Lerp(Color.white, Color.yellow, Random.value), null, 3, 8));
        }

        ShootSound();
        ShootEffects();

        SummonProjectile(user, boostAccuracy);

        room.AddObject(new Spark(firstChunk.pos + upDir * 5f - LastAimDir * 5f, upDir * 8f + Random.insideUnitCircle * 3f, Color.yellow, null, 60, 120));

        if (Clip == 0 && Automatic)
        {
            FireDelay = 20;
        }
    }

    protected abstract void ShootEffects();

    public void Reload(bool smart)
    {
        var costsatisfied = false;

        // if (Owner is Player p && smart)
        // {
        //     Debug.Log(ChugBaseHunter.inventories[p] != null);
        //      if (HHUtils.inventories.GetOrCreateValue(p).CanFeedGun(this))
        //      {
        //          Debug.Log(HHUtils.inventories.GetOrCreateValue(p) != null);
        //          costsatisfied = true;
        //      }
        // }

        if ((smart && costsatisfied) || Owner is Scavenger || Owner == null)
        {
            ReloadTime = ReloadSpeed;
        }
        else if (IsTriggerReleased)
        {
            room.PlaySound(SoundID.Rock_Hit_Wall, firstChunk.pos, .9f, 1.35f);
            firstChunk.pos -= AimDir * 5f;
            firstChunk.lastPos = firstChunk.pos;
        }
    }

    protected abstract void SummonProjectile(PhysicalObject user, bool boostAccuracy);

    protected void CheckIfArena(World world)
    {
        if (world.game.IsArenaSession)
        {
            Clip = FullClip;
        }
    }


    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1 + FullClip];

        sLeaser.sprites[0] = new FSprite(GunSpriteName)
        {
            anchorY = 0.5f, //.8
        };

        for (var i = 1; i <= FullClip; i++)
        {
            sLeaser.sprites[i] = new FSprite("pixel")
            {
                scale = 4,
                isVisible = false
            };
        }
        FirstPipAngle = 0 + (PipAngleDiff / 2 * (FullClip - 1));

        AddToContainer(sLeaser, rCam, null!);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Items");
        }
        var HUDcont = rCam.ReturnFContainer("HUD");

        sLeaser.sprites[0].RemoveFromContainer();
        newContatiner.AddChild(sLeaser.sprites[0]);
        for (var i = 1; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            HUDcont.AddChild(sLeaser.sprites[i]);
        }
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        // TODO
        //sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName(GunSpriteName + (Clip == 0 ? "NoMag" : ""));
        sLeaser.sprites[0].x = Mathf.Lerp(firstChunk.lastPos.x, firstChunk.pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(firstChunk.lastPos.y, firstChunk.pos.y, timeStacker) - camPos.y;
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), Vector3.Slerp(LastAimDir, AimDir, timeStacker)) - 90f;
        if (mode == Mode.OnBack)
        {
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            var perpV = Custom.PerpendicularVector(v);
            sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), perpV);
            sLeaser.sprites[0].scaleY = -1f;
        }
        else
        {
            sLeaser.sprites[0].scaleY = (IsFlipped ? 1f : -1f);
        }


        if (Owner != null && Owner is Player p && mode == Mode.Carried)
        {
            for (var i = 1; i <= FullClip; i++)
            {
                sLeaser.sprites[i].isVisible = i <= Clip; // TODO
                sLeaser.sprites[i].SetPosition(Custom.DegToVec((FirstPipAngle - (PipAngleDiff * (i - 1)))) * 30 + Owner.firstChunk.pos + new Vector2(0, 20) - camPos);
            }
        }
        else
        {
            for (var i = 1; i <= FullClip; i++)
            {
                sLeaser.sprites[i].isVisible = false;
            }
        }

        for (var i = 1; i <= FullClip; i++)
        {
            sLeaser.sprites[i].alpha = OwnerAge / 20f;
        }


        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) { }


    public virtual void ShootSound() { }
}
