using DMD;
using RWCustom;
using System;
using UnityEngine;
public class AmmoObject : PhysicalObject, IDrawable
{
    public AbstractAmmo AbstractAmmo;
    public int AmmoType;
    public int SpawnJumpTicks;
	public AmmoObject(AbstractPhysicalObject abObj) : base(abObj)
	{
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 6f, 0.5f);
        bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.3f;
        surfaceFriction = 0.92f;
        collisionLayer = 0;
        waterFriction = 0.95f;
        buoyancy = 0.7f;
        AbstractAmmo = abObj as AbstractAmmo;
        AmmoType = AbstractAmmo.AmmoType;
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
        //Debug.Log(bodyChunks[0].pos);
        if (SpawnJumpTicks > 0)
        {
            SpawnJumpTicks--;
            bodyChunks[0].vel += new Vector2(1 - Mathf.Sin(Random.value * Mathf.PI), Random.Range(0.6f, 0.8f)) * 3;
        }
    }
    public void SpawnJump()
    {
        SpawnJumpTicks = 7;
    }
    public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20", false);
        AddToContainer(sLeaser, rCam, null);
    }

    public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 DrawPos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        sLeaser.sprites[0].x = DrawPos.x - camPos.x;
        sLeaser.sprites[0].y = DrawPos.y - camPos.y;
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public virtual void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

    public virtual void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Items");
        }

        for (int num = 0; sLeaser.sprites.Length > num; num++)
        {
            sLeaser.sprites[num].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[num]);
        }
    }

}
public class AbstractAmmo : AbstractPhysicalObject
{
	public int AmmoType;

	public AbstractAmmo(int AmmoType, World World, AbstractObjectType Type, PhysicalObject Ammo, WorldCoordinate Pos, EntityID ID) : base(World,Type,Ammo,Pos,ID)
	{ 
		this.AmmoType = AmmoType;
	}
    public override void Realize()
    {
		realizedObject = new AmmoObject(this);
    }

}
