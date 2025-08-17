namespace DMD;

public static class World_Hooks
{
    public static void ApplyHooks()
    {
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObjectOnRealize;
    }

    private static void AbstractPhysicalObjectOnRealize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
    {
        if (self.realizedObject is null)
        {
            if (self.type == Enums.Guns.AKM)
            {
                self.realizedObject = new AKM(self, self.world);
            }
            else if (self.type == Enums.Guns.Shotgun)
            {
                self.realizedObject = new Shotgun(self, self.world);
            }
        }
        orig(self);
    }
}
