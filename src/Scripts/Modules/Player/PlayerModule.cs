namespace DMD;

public class PlayerModule
{
    public WeakReference<AbstractCreature> AbstractPlayerRef { get; }
    public Player? PlayerRef => AbstractPlayerRef.TryGetTarget(out var player) ? player.realizedCreature as Player : null;

    public PlayerModuleGraphics Graphics { get; } = new();

    public Player.InputPackage UnblockedInput { get; set; }
    public bool BlockInput { get; set; }
    public bool WasSwapLeftInput { get; set; }
    public bool WasSwapRightInput { get; set; }
    public bool WasSwapped { get; set; }


    public Vector2 AimPos { get; set; }
    public bool ReturnAimToCenter { get; set; } = true;

    public float HudFade { get; set; }
    public float HudFadeTimer { get; set; }

    public List<AbstractPhysicalObject> GunInventory { get; } = [];
    public int ActiveGunIndex { get; set; }
    public AbstractPhysicalObject ActiveGun => GunInventory[ActiveGunIndex];

    public PlayerModule(Player player)
    {
        AbstractPlayerRef = new WeakReference<AbstractCreature>(player.abstractCreature);

        CreateInventory(player);
    }

    private void CreateInventory(Player player)
    {
        GunInventory.Add(new AbstractPhysicalObject(player.abstractCreature.world, Enums.Guns.None, null!,
            player.abstractCreature.pos, player.abstractCreature.world.game.GetNewID()));

        var save = player.abstractCreature.world.game.GetMiscWorld();

        if (save is null)
        {
            return;
        }

        foreach (var gunStrId in save.UnlockedGuns)
        {
            if (!ExtEnumBase.TryParse(typeof(AbstractPhysicalObject.AbstractObjectType), gunStrId, false, out var gunType))
            {
                continue;
            }

            var gun = new AbstractPhysicalObject(player.abstractCreature.world,
                (AbstractPhysicalObject.AbstractObjectType)gunType, null!, player.abstractCreature.pos,
                player.abstractCreature.world.game.GetNewID());

            GunInventory.Add(gun);
        }
    }

    public void ShowHUD(int duration)
    {
        HudFadeTimer = duration;
    }
}
