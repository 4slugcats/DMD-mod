namespace DMD;

public class PlayerModule
{
    public WeakReference<AbstractCreature> AbstractPlayerRef { get; }
    public Player? PlayerRef => AbstractPlayerRef.TryGetTarget(out var player) ? player.realizedCreature as Player : null;

    public PlayerModuleGraphics Graphics { get; } = new();

    public Player.InputPackage UnblockedInput { get; set; }
    public bool BlockInput { get; set; }

    public Vector2 AimPos { get; set; }
    public bool ReturnAimToCenter { get; set; } = true;

    public float HudFade { get; set; }

    public List<AbstractPhysicalObject> GunInventory { get; } = [];
    public int? ActiveGunIndex { get; set; }
    public AbstractPhysicalObject? ActiveGun => ActiveGunIndex is not null && ActiveGunIndex < GunInventory.Count ? GunInventory[(int)ActiveGunIndex] : null;

    public PlayerModule(Player player)
    {
        AbstractPlayerRef = new WeakReference<AbstractCreature>(player.abstractCreature);

        GiveGuns(player);
    }

    private void GiveGuns(Player player)
    {
        var save = player.abstractCreature.world.game.GetMiscWorld();

        if (save is null)
        {
            // TODO: handle arena
            return;
        }

        foreach (var gunId in save.UnlockedGuns)
        {
            var gun = new Gun

            GunInventory.Add(gun);
        }
    }
}
