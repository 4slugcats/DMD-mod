using RWCustom;
using UnityEngine;

namespace DMD;

// Slightly modified Slime_Cubed bullets
public class Bullet : UpdatableAndDeletable, IDrawable
{
    public PhysicalObject parent;

    public Vector2 hitPos;
    public Vector2 startPos;
    public Vector2 direction;

    public float fade;
    public float lastFade;

    public bool big = false;
    public bool firstFrame = true;
    public bool burn = false;
    public bool loud;

    public float fadeTime = 0.15f;
    public float force = 15f;
    public float damage = 2.5f;
    public float maxDist = 2500f;
    public float stun = 15f;

    public Bullet(PhysicalObject parent, Vector2 startPos, Vector2 direction, float damage, float force, float stun, bool onlyBurn)
    {
        this.parent = parent;
        this.startPos = startPos;
        this.direction = direction;
        this.damage = damage;
        this.force = force;
        this.stun = stun;
        burn = onlyBurn;
        fade = 0f;
        lastFade = 0f;
        hitPos = startPos + direction * maxDist;
        room = parent.room;
        loud = true;
    }

    public Bullet(PhysicalObject parent, Vector2 startPos, Vector2 direction, float damage, float force, float stun, bool onlyBurn, bool loud)
    {
        this.parent = parent;
        this.startPos = startPos;
        this.direction = direction;
        this.damage = damage;
        this.force = force;
        this.stun = stun;
        burn = onlyBurn;
        fade = 0f;
        lastFade = 0f;
        hitPos = startPos + direction * maxDist;
        room = parent.room;
        this.loud = loud;
    }

    public void Fire()
    {
        // Trace against world and chunks
        var maxPos = hitPos;
        hitPos = SharedPhysics.ExactTerrainRayTracePos(room, startPos, hitPos) ?? maxPos;
        var collisionResultLayer0 = SharedPhysics.TraceProjectileAgainstBodyChunks(null, room, startPos, ref hitPos, 0.1f, 0, parent, false);
        if (collisionResultLayer0.hitSomething && collisionResultLayer0.obj is Overseer)
        {
            (collisionResultLayer0.obj as Overseer).Violence(parent.firstChunk, direction * force + Vector2.up * 0.2f * force, collisionResultLayer0.chunk, collisionResultLayer0.onAppendagePos, Creature.DamageType.Stab, damage * 10f, stun);
        }
        var collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, room, startPos, ref hitPos, 0.1f, 1, parent, true);
        if (collisionResult.hitSomething)
        {
            hitPos = collisionResult.collisionPoint;
        }

        // Strike the hit object
        if (hitPos != maxPos)
        {
            Strike(collisionResult);
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        firstFrame = false;
        lastFade = fade;
        fade = Mathf.Min(fade + 0.025f / fadeTime, 1f);
        if (fade >= 1f)
        {
            Destroy();
        }
    }

    private void Strike(SharedPhysics.CollisionResult collision)
    {
        var appliedForce = direction * force + Vector2.up * 0.2f * force;
        var sourceChunk = parent.firstChunk;

        if (room.water && room.PointSubmerged(hitPos))
        {
            room.waterObject.WaterfallHitSurface(hitPos, hitPos.x, 1f);
            if (loud)
            {
                room.PlaySound(SoundID.Small_Object_Into_Water_Fast, hitPos);
            }
        }
        else if (collision.chunk == null)
        {
            if (loud)
            {
                room.PlaySound(SoundID.Bullet_Drip_Strike, hitPos, 0.75f + 0.5f * damage, 1.5f - damage);
            }
        }

        if (collision.onAppendagePos != null)
        {
            collision.onAppendagePos.appendage.ownerApps.ApplyForceOnAppendage(collision.onAppendagePos, appliedForce);
        }

        if (collision.chunk != null)
        {
            collision.chunk.vel += direction * (force / collision.chunk.mass);
        }

        if (collision.obj is Creature critter)
        {
            if (parent != null && parent is Creature)
            {
                critter.SetKillTag((parent as Creature).abstractCreature);
            }
            if (loud)
            {
                room.PlaySound(SoundID.Spear_Stick_In_Creature, hitPos, 0.75f + 0.5f * damage, 1.5f - damage);
            }

            // TODO: figure out if this is important, BurnDamage class missing
            // if (burn)
            // {
            //     BurnDamage.AddBurn(critter, damage);
            // }
            // else
            // {
                if (critter is Player)
                {
                    critter.Violence(sourceChunk, appliedForce, collision.chunk, collision.onAppendagePos, Creature.DamageType.Stab, damage * 0.3f, stun * 1.2f);
                }
                else
                {
                    critter.Violence(sourceChunk, appliedForce, collision.chunk, collision.onAppendagePos, Creature.DamageType.Stab, damage, stun);
                }

                // TODO: ditto
                // if (!(critter.State is HealthState))
                // {
                //     BurnDamage.AddBurn(critter, damage);
                // }
            //}


            if (room.world.game.IsArenaSession && room.world.game.GetArenaGameSession.GameTypeSetup.spearHitScore != 0 && sourceChunk.owner is Player ply)
            {
                var isWorthPoints = true;
                if ((collision.obj as Creature)?.State is HealthState && ((collision.obj as Creature)?.State as HealthState)?.health <= 0f)
                {
                    isWorthPoints = false;
                }
                else if ((collision.obj as Creature)?.State is not HealthState && ((collision.obj as Creature)!).State.dead)
                {
                    isWorthPoints = false;
                }

                if (isWorthPoints)
                {
                    room.world.game.GetArenaGameSession.PlayerLandSpear(ply, critter);
                }
            }
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("Futile_White", true);
        sLeaser.sprites[0].scaleX = 0.125f;
        sLeaser.sprites[0].anchorY = 0f;
        sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["BulletRain"];
        sLeaser.sprites[1] = new FSprite("RainSplash", true);
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var drawEndPos = firstFrame ? Vector2.Lerp(startPos, hitPos, timeStacker) : hitPos;
        var drawStartPos = Vector2.Lerp(startPos, hitPos, Mathf.Lerp(lastFade, fade, timeStacker));

        sLeaser.sprites[0].x = drawEndPos.x - camPos.x;
        sLeaser.sprites[0].y = drawEndPos.y - camPos.y;
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(drawEndPos, drawStartPos);
        sLeaser.sprites[0].scaleY = Vector2.Distance(drawEndPos, drawStartPos) / 16f;
        sLeaser.sprites[1].x = drawEndPos.x - camPos.x;
        sLeaser.sprites[1].y = drawEndPos.y - camPos.y;
        sLeaser.sprites[1].scale = Mathf.Sin(Mathf.Lerp(lastFade, fade, timeStacker) * Mathf.PI) * 0.4f;

        if (big)
        {
            sLeaser.sprites[1].scale *= 2f; sLeaser.sprites[0].scale *= 3f;
        }

        sLeaser.sprites[1].rotation = Random.value * 360f;
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[0]);
        rCam.ReturnFContainer("Items").AddChild(sLeaser.sprites[1]);
    }
}
