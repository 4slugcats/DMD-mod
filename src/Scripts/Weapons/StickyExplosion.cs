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
        evenUpdate = eu; // this is what the base.update would do for normal explosions

        if (!explosionReactorsNotified)
        {
            explosionReactorsNotified = true;
            for (var i = 0; i < room.updateList.Count; i++)
            {
                if (room.updateList[i] is IReactToExplosions)
                {
                    (room.updateList[i] as IReactToExplosions).Explosion(this);
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
        var num = rad * (0.25f + 0.75f * Mathf.Sin(Mathf.InverseLerp(0f, (float)lifeTime, (float)frame) * 3.1415927f));
        for (var j = 0; j < room.physicalObjects.Length; j++)
        {
            for (var k = 0; k < room.physicalObjects[j].Count; k++)
            {
                if (sourceObject != room.physicalObjects[j][k] && !room.physicalObjects[j][k].slatedForDeletetion)
                {
                    var num2 = 0f;
                    var num3 = float.MaxValue;
                    var num4 = -1;
                    for (var l = 0; l < room.physicalObjects[j][k].bodyChunks.Length; l++)
                    {
                        var num5 = Vector2.Distance(pos, room.physicalObjects[j][k].bodyChunks[l].pos);
                        num3 = Mathf.Min(num3, num5);
                        if (num5 < num)
                        {
                            var num6 = Mathf.InverseLerp(num, num * 0.25f, num5);
                            if (!room.VisualContact(pos, room.physicalObjects[j][k].bodyChunks[l].pos))
                            {
                                num6 -= 0.5f;
                            }
                            if (num6 > 0f)
                            {
                                room.physicalObjects[j][k].bodyChunks[l].vel += PushAngle(pos, room.physicalObjects[j][k].bodyChunks[l].pos) * (force / room.physicalObjects[j][k].bodyChunks[l].mass) * num6;
                                room.physicalObjects[j][k].bodyChunks[l].pos += PushAngle(pos, room.physicalObjects[j][k].bodyChunks[l].pos) * (force / room.physicalObjects[j][k].bodyChunks[l].mass) * num6 * 0.1f;
                                if (num6 > num2)
                                {
                                    num2 = num6;
                                    num4 = l;
                                }
                            }
                        }
                    }
                    if (room.physicalObjects[j][k] == killTagHolder)
                    {
                        num2 *= killTagHolderDmgFactor;
                    }
                    if (deafen > 0f && room.physicalObjects[j][k] is Creature)
                    {
                        (room.physicalObjects[j][k] as Creature).Deafen((int)Custom.LerpMap(num3, num * 1.5f * deafen, num * Mathf.Lerp(1f, 4f, deafen), 650f * deafen, 0f));
                    }
                    if (num4 > -1)
                    {
                        if (room.physicalObjects[j][k] is Creature)
                        {
                            var num7 = 0;
                            while ((float)num7 < Math.Min(Mathf.Round(num2 * damage * 2f), 8f))
                            {
                                var p = room.physicalObjects[j][k].bodyChunks[num4].pos + Custom.RNV() * room.physicalObjects[j][k].bodyChunks[num4].rad * Random.value;
                                room.AddObject(new WaterDrip(p, Custom.DirVec(pos, p) * force * Random.value * num2, false));
                                num7++;
                            }
                            if (killTagHolder != null && room.physicalObjects[j][k] != killTagHolder)
                            {
                                (room.physicalObjects[j][k] as Creature).SetKillTag(killTagHolder.abstractCreature);
                            }
                            if((room.physicalObjects[j][k] as Creature) is Player player && HHUtils.IsMe(player))
                            {
                                (room.physicalObjects[j][k] as Creature).Violence(null, null, room.physicalObjects[j][k].bodyChunks[num4], null, Creature.DamageType.Explosion, 0, num2 * stun * 0.05f);
                            }
                            else
                            {
                                (room.physicalObjects[j][k] as Creature).Violence(null, null, room.physicalObjects[j][k].bodyChunks[num4], null, Creature.DamageType.Explosion, num2 * damage / ((!((room.physicalObjects[j][k] as Creature).State is HealthState)) ? 1f : ((float)lifeTime)), num2 * stun);
                            }

                            if (minStun > 0f)
                            {
                                (room.physicalObjects[j][k] as Creature).Stun((int)(minStun * Mathf.InverseLerp(0f, 0.5f, num2)));
                            }
                            if ((room.physicalObjects[j][k] as Creature).graphicsModule != null && (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts != null)
                            {
                                for (var m = 0; m < (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts.Length; m++)
                                {
                                    if ((room.physicalObjects[j][k] as Creature) is Player payer && HHUtils.IsMe(payer))
                                    {
                                        (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos += PushAngle(pos, (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * force * 2f;
                                        (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].vel += PushAngle(pos, (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * force * 2f;
                                    }
                                    else
                                    {
                                        (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos += PushAngle(pos, (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * force * 5f;
                                        (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].vel += PushAngle(pos, (room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m].pos) * num2 * force * 5f;
                                    }
                                    if ((room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m] is Limb)
                                    {
                                        ((room.physicalObjects[j][k] as Creature).graphicsModule.bodyParts[m] as Limb).mode = Limb.Mode.Dangle;
                                    }
                                }
                            }
                        }
                        room.physicalObjects[j][k].HitByExplosion(num2, this, num4);
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
