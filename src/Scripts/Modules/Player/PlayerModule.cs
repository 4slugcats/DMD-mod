namespace DMD;

public partial class PlayerModule
{
    public WeakReference<AbstractCreature> AbstractPlayerRef { get; }
    public Player? PlayerRef => AbstractPlayerRef.TryGetTarget(out var player) ? player.realizedCreature as Player : null;

    public PlayerModuleGraphics Graphics { get; } = new();

    public PlayerModule(Player self)
    {
        AbstractPlayerRef = new(self.abstractCreature);
    }
}
