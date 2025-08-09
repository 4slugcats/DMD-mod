using Noise;
using RWCustom;

namespace DMD;

public class StickyExplosion : Explosion
{
    public StickyExplosion(Room room, PhysicalObject sourceObject, Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise) : base(room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise)
    {

    }

    public override void Update(bool eu)
    {
        this.evenUpdate = eu; // this is what the base.update would do for normal explosions

        if (!this.explosionReactorsNotified)
        {
            this.explosionReactorsNotified = true;
            for (int i = 0; i < this.room.updateList.Count; i++)
            {
                if (this.room.updateList[i] is Explosion.IReactToExplosions)
                {
                    (this.room.updateList[i] as Explosion.IReactToExplosions).Explosion(this);
                }
            }
            if (this.room.waterObject != null)
            {
                this.room.waterObject.Explosion(this);
            }
            if (this.sourceObject != null)
            {
                this.room.InGameNoise(new InGameNoise(this.pos, this.backgroundNoise * 2700f, this.sourceObject, this.backgroundNoise * 6f));
            }
        }
        this.room.MakeBackgroundNoise(this.backgroundNoise);
        float num = this.rad * (0.25f + 0.75f * Mathf.Sin(Mathf.InverseLerp(0f, (float)this.lifeTime, (float)this.frame) * 3.1415927f));
        for (int j = 0; j < this.room.physicalObjects.Length; j++)
        {
            for (int k = 0; k < this.room.physicalObjects[j].Count; k++)
            {
                if (this.sourceObject != this.room.physicalObjects[j][k] && !this.room.physicalObjects[j][k].slatedForDeletetion)
                {
                    float num2 = 0f;
                    float num3 = float.MaxValue;
                    int num4 = -1;
                    for (int l = 0; l < this.room.physicalObjects[j][k].bodyChunks.Length; l++)
                    {
                        float num5 = Vector2.Distance(this.pos, this.room.physicalObjects[j][k].bodyChunks[l].pos);
                        num3 = Mathf.Min(num3, num5);
                        if (num5 < num)
                        {
                            float num6 = Mathf.InverseLerp(num, num * 0.25f, num5);
                            if (!this.room.VisualContact(this.pos, this.room.physicalObjects[j][k].bodyChunks[l].pos))
                            {
                                num6 -= 0.5f;
                            }
                            if (num6 > 0f)
                            {
                                this.room.physicalObjects[j][k].bodyChunks[l].vel += this.PushAngle(this.pos, this.room.physicalObjects[j][k].bodyChunks[l].pos) * (this.force / this.room.physicalObjects[j][k].bodyChunks[l].mass) * num6;
                                this.room.physicalObjects[j][k].bodyChunks[l].pos += this.PushAngle(this.pos, this.room.physicalObjects[j][k].bodyChunks[l].pos) * (this.force / this.room.physicalObjects[j][k].bodyChunks[l].mass) * num6 * 0.1f;
                                if (num6 > num2)
                                {
                                    num2 = num6;
                                    num4 = l;
                                }
                            }
                        }
                    }
                    if (this.room.physicalObjects[j][k] == this.killTagHolder)
                    {
                        num2 *= this.killTagHolderDmgFactor;
                    }
                    if (this.deafen > 0f && this.room.physicalObjects[j][k] is Creature)
                    {
                        (this.room.physicalObjects[j][k] as Creature).Deafen((int)Custom.LerpMap(num3, num * 1.5f * this.deafen, num * Mathf.Lerp(1f, 4f, this.deafen), 650f * this.deafen, 0f));
                    }
                    if (num4 > -1)
                    {
                        if (this.room.physicalObjects[j][k] is Creature)
                        {
                            int num7 = 0;
                            while ((float)num7 < Math.Min(Mathf.Round(num2 * this.damage * 2f), 8f))
                            {
                                Vector2 p = this.room.physicalObjects[j][k].bodyChunks[num4].pos + Custom.RNV() * this.room.physicalObjects[j][k].bodyChunks[num4].rad * UnityEngine.Random.value;
                                this.room.AddObject(new WaterDrip(p, Custom.DirVec(this.pos, p) * this.force * UnityEngine.Random.value * num2, false));
                                num7++;
                            }
                            if (this.killTagHolder != null && this.room.physicalObjects[j][k] != this.killTagHolder)
                            {
                                (this.room.physicalObjects[j][k] as Creature).SetKillTag(this.killTagHolder.abstractCreature);
                            }
                            if((this.room.physicalObjects[j][k] as Creature) is Player player && HHUtils.IsMe(player))
                                (this.room.physicalObjects[j][k] as Creature).Violence(null, null, this.room.physicalObjects[j][k].bodyChunks[num4], null, Creature.DamageType.Explosion, 0, num2 * this.stun * 0.05f);
                            else
                                (this.room.physicalObjects[j][k] as Creature).Violence(null, null, this.room.physicalObjects[j][k].bodyChunks[num4], null, Creature.DamageType.Explosion, num2 * this.damage / ((!((this.room.physicalObjects[j][k] as Creature).State is HealthState)) ? 1f : ((float)this.lifeTime)), num2 * this.stun);
                            if (this.minStun > 0f)
                            {
                                (this.room.physicalObjects[j][k] as Creature).Stun((int)(this.minStun * Mathf.InverseLerp(0f, 0.5f, num2)));
                            }
                            if ((this.room.physicalObjects[j][k] as Creature).graphicsModule != null && (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts != null)
                            {
                                for (int m = 0; m < (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts.Length; m++)
                                {
                                    if ((this.room.physicalObjects[j][k] as Creature) is Player payer && HHUtils.IsMe(payer))
                                    {
                                        (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos += this.PushAngle(this.pos, (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * this.force * 2f;
                                        (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].vel += this.PushAngle(this.pos, (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * this.force * 2f;
                                    }
                                    else
                                    {
                                        (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos += this.PushAngle(this.pos, (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * this.force * 5f;
                                        (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].vel += this.PushAngle(this.pos, (this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * this.force * 5f;
                                    }
                                    if ((this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m] is Limb)
                                    {
                                        ((this.room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m] as Limb).mode = Limb.Mode.Dangle;
                                    }
                                }
                            }
                        }
                        this.room.physicalObjects[j][k].HitByExplosion(num2, this, num4);
                    }
                }
            }
        }
        this.frame++;
        if (this.frame > this.lifeTime)
        {
            this.Destroy();
        }
    }
}