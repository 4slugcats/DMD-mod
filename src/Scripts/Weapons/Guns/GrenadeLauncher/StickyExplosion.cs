using Noise;
using RWCustom;

namespace DMD;

public class StickyExplosion(Room room, PhysicalObject sourceObject, Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    : Explosion(room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise)
{
    public override void Update(bool eu)
    {
        evenUpdate = eu; // this is what the base.update would do for normal explosions

        if (!explosionReactorsNotified)
        {
            explosionReactorsNotified = true;
            for (var i = 0; i < room.updateList.Count; i++)
            {
                if (room.updateList[i] is IReactToExplosions)
                {
                    ((IReactToExplosions)room.updateList[i]).Explosion(this);
                }
            }

            if (room.waterObject != null)
            {
                room.waterObject.Explosion(this);
            }

            if (sourceObject != null)
            {
                room.InGameNoise(new InGameNoise(pos, backgroundNoise * 2700f, sourceObject, backgroundNoise * 6f));
            }
        }

        room.MakeBackgroundNoise(backgroundNoise);
        var num = rad * (0.25f + 0.75f * Mathf.Sin(Mathf.InverseLerp(0f, lifeTime, frame) * 3.1415927f));
        foreach (var t in room.physicalObjects)
        {
            foreach (var t1 in t)
            {
                if (sourceObject != t1 && !t1.slatedForDeletetion)
                {
                    var num2 = 0f;
                    var num3 = float.MaxValue;
                    var num4 = -1;

                    for (var l = 0; l < t1.bodyChunks.Length; l++)
                    {
                        var num5 = Vector2.Distance(pos, t1.bodyChunks[l].pos);
                        num3 = Mathf.Min(num3, num5);
                        if (num5 < num)
                        {
                            var num6 = Mathf.InverseLerp(num, num * 0.25f, num5);
                            if (!room.VisualContact(pos, t1.bodyChunks[l].pos))
                            {
                                num6 -= 0.5f;
                            }

                            if (num6 > 0f)
                            {
                                t1.bodyChunks[l].vel += PushAngle(pos, t1.bodyChunks[l].pos) * (force / t1.bodyChunks[l].mass) * num6;
                                t1.bodyChunks[l].pos += PushAngle(pos, t1.bodyChunks[l].pos) * (force / t1.bodyChunks[l].mass) * num6 * 0.1f;
                                if (num6 > num2)
                                {
                                    num2 = num6;
                                    num4 = l;
                                }
                            }
                        }
                    }

                    if (t1 == killTagHolder)
                    {
                        num2 *= killTagHolderDmgFactor;
                    }

                    if (deafen > 0f && t1 is Creature)
                    {
                        ((Creature)t1).Deafen((int)Custom.LerpMap(num3, num * 1.5f * deafen, num * Mathf.Lerp(1f, 4f, deafen), 650f * deafen, 0f));
                    }

                    if (num4 > -1)
                    {
                        if (t1 is Creature)
                        {
                            var num7 = 0;
                            while (num7 < Math.Min(Mathf.Round(num2 * damage * 2f), 8f))
                            {
                                var p = t1.bodyChunks[num4].pos + Custom.RNV() * t1.bodyChunks[num4].rad * Random.value;
                                room.AddObject(new WaterDrip(p, Custom.DirVec(pos, p) * force * Random.value * num2, false));
                                num7++;
                            }

                            if (killTagHolder != null && t1 != killTagHolder)
                            {
                                ((Creature)t1).SetKillTag(killTagHolder.abstractCreature);
                            }

                            if ((Creature)t1 is Player player && player.IsDMD())
                            {
                                ((Creature)t1).Violence(null, null, t1.bodyChunks[num4], null, Creature.DamageType.Explosion, 0, num2 * stun * 0.05f);
                            }
                            else
                            {
                                ((Creature)t1).Violence(null, null, t1.bodyChunks[num4], null, Creature.DamageType.Explosion, num2 * damage / (!(((Creature)t1).State is HealthState) ? 1f : lifeTime), num2 * stun);
                            }

                            if (minStun > 0f)
                            {
                                ((Creature)t1).Stun((int)(minStun * Mathf.InverseLerp(0f, 0.5f, num2)));
                            }

                            if (((Creature)t1).graphicsModule != null && ((Creature)t1).graphicsModule.bodyParts != null)
                            {
                                for (var m = 0; m < ((Creature)t1).graphicsModule.bodyParts.Length; m++)
                                {
                                    var creature = (Creature)t1;

                                    if (creature is Player p && p.IsDMD())
                                    {
                                        creature.graphicsModule.bodyParts[m].pos += PushAngle(pos, creature.graphicsModule.bodyParts[m].pos) * num2 * force * 2f;
                                        creature.graphicsModule.bodyParts[m].vel += PushAngle(pos, creature.graphicsModule.bodyParts[m].pos) * num2 * force * 2f;
                                    }
                                    else
                                    {
                                        creature.graphicsModule.bodyParts[m].pos += PushAngle(pos, creature.graphicsModule.bodyParts[m].pos) * num2 * force * 5f;
                                        creature.graphicsModule.bodyParts[m].vel += PushAngle(pos, creature.graphicsModule.bodyParts[m].pos) * num2 * force * 5f;
                                    }

                                    if (creature.graphicsModule.bodyParts[m] is Limb limb)
                                    {
                                        limb.mode = Limb.Mode.Dangle;
                                    }
                                }
                            }
                        }

                        t1.HitByExplosion(num2, this, num4);
                    }
                }
            }
        }

        frame++;
        if (frame > lifeTime)
        {
            Destroy();
        }
    }
}
