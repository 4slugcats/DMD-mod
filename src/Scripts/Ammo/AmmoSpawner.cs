using DMD;
using System.Runtime.CompilerServices;

public static class AmmoSpawner
{
    public static void ApplyHooks()
    {
        On.Creature.Die += Creature_Die;

    }
    //cwt to track if a creature has spawned ammo, pretty much it
    public static readonly ConditionalWeakTable<AbstractCreature, AmmoSpawnTracker> c = new ConditionalWeakTable<AbstractCreature, AmmoSpawnTracker>();
    public class AmmoSpawnTracker
    {
        public bool AmmoGiven = false;
    }
    public static AmmoSpawnTracker GetAmmoTracker(this AbstractCreature abstractCreature) => c.GetValue(abstractCreature, _ => new AmmoSpawnTracker());
    private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
    {
        orig(self);
        if (self.killTag != null && self.killTag.realizedCreature != null && !self.abstractCreature.GetAmmoTracker().AmmoGiven)
        {
            for (int i = 0; i < self.room.PlayersInRoom.Count; i++)
            {
                if (self.killTag.realizedCreature == self.room.PlayersInRoom[i])
                {
                    self.abstractCreature.GetAmmoTracker().AmmoGiven = true;
                    Player Killer = self.room.PlayersInRoom[i];
                    // if a non-DMD player kills a creature, it triggers a smaller chance for random ammo to spawn
                    int Pickup = -1;
                    if (Killer.TryGetDMDModule(out var playerModule))
                    {
                        if (playerModule.ActiveGun.type != Enums.Guns.None)
                        {
                            Pickup = (playerModule.ActiveGun.realizedObject as Gun).AmmoType;
                        }
                    }
                    if (Random.value > (Pickup == -1 ? .1f : .35f))
                    {
                        for (int b = 0; b < Random.Range(2, 9); b++)
                        {
                            AmmoObject Ammo = new AmmoObject(new AbstractAmmo(Pickup == -1 ? Random.Range(0, 2) : Pickup, self.room.world, Enums.Objects.Ammo, null, self.abstractCreature.pos, self.room.world.game.GetNewID()));
                            self.room.AddObject(Ammo);
                            Ammo.bodyChunks[0].pos = self.mainBodyChunk.pos;

                        }
                        Debug.Log("ammo spawned");

                    }
                }
            }

        }
    }
}
