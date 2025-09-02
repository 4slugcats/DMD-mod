using HUD;
using RWCustom;
using System.Runtime.CompilerServices;
using Vector2 = UnityEngine.Vector2;

namespace DMD;

public class InventoryHUD(HUD.HUD hud, FContainer fContainer) : HudPart(hud)
{
    public FContainer HUDFContainer { get; } = fContainer;

    public static ConditionalWeakTable<AbstractPhysicalObject, InventorySymbol> Symbols { get; } = new();
    public static ConditionalWeakTable<AbstractCreature, FSprite> InventoryCircles { get; } = new();

    public List<InventorySymbol> AllSymbols { get; } = [];
    public List<FSprite> AllHUDCircles { get; } = [];

    public bool HardSetPos { get; set; }

    public override void Draw(float timeStacker)
    {
        if (hud.rainWorld.processManager.currentMainLoop is not RainWorldGame game)
        {
            return;
        }

        var allModules = game.GetAllDMDModules();

        if (allModules.All(x => x.HudFade < 0.001f))
        {
            HardSetPos = true;
            return;
        }

        foreach (var playerModule in allModules)
        {
            var player = playerModule.PlayerRef;

            if (player?.abstractCreature?.world is null)
            {
                continue;
            }

            if (player.abstractCreature.Room is null)
            {
                continue;
            }

            var cameras = player.abstractCreature.world.game.cameras;
            var rCam = cameras.First();

            var playerChunkPos = Vector2.Lerp(player.firstChunk.lastPos, player.firstChunk.pos, timeStacker);
            var playerPos = player.abstractCreature.world.RoomToWorldPos(playerChunkPos, player.abstractCreature.Room.index);
            var roomPos = player.abstractCreature.world.RoomToWorldPos(rCam.pos, rCam.room.abstractRoom.index);

            var truePos = playerPos - roomPos;

            var activeGunIndex = playerModule.ActiveGunIndex;

            // Make inventory pearls movement independent of framerate
            var lerpFac = 10.0f * Time.deltaTime;

            // Reset position when an inventory is shown
            if (HardSetPos)
            {
                lerpFac = 1.0f;
                HardSetPos = false;
            }

            if (!InventoryCircles.TryGetValue(player.abstractCreature, out var circle))
            {
                circle = new FSprite("dmd_hudcircle")
                {
                    alpha = 0.0f,
                };

                InventoryCircles.Add(player.abstractCreature, circle);
                AllHUDCircles.Add(circle);
            }

            if (circle.container != HUDFContainer)
            {
                HUDFContainer.AddChild(circle);
            }

            for (var i = 0; i < playerModule.GunInventory.Count; i++)
            {
                var abstractObject = playerModule.GunInventory[i];
                var diff = i - activeGunIndex;

                var isActive = playerModule.ActiveGun == abstractObject;

                if (!Symbols.TryGetValue(abstractObject, out var symbol) || !AllSymbols.Contains(symbol))
                {
                    continue;
                }

                symbol.DistFade = isActive ? 1.0f : 0.8f;

                var origin = truePos;
                var angle = (diff) * Mathf.PI * 2.0f / playerModule.GunInventory.Count + Mathf.Deg2Rad * 90.0f;

                var radius = Custom.LerpMap(playerModule.HudFade, 0.5f, 1.0f, 65.0f, 80.0f);
                var invPos = new Vector2(origin.x + Mathf.Cos(angle) * radius, origin.y + Mathf.Sin(angle) * radius);

                symbol.Pos = Custom.Dist(symbol.Pos, invPos) > 300.0f ? invPos : Vector2.Lerp(symbol.Pos, invPos, lerpFac);
                symbol.Scale = isActive ? 1.2f : 0.8f;
            }

            circle.SetPosition(Custom.Dist(circle.GetPosition(), truePos) > 300.0f ? truePos : Vector2.Lerp(circle.GetPosition(), truePos, lerpFac));
            circle.scale = Custom.LerpMap(playerModule.HudFade, 0.0f, 1.0f, 0.75f, 1.05f);
            circle.alpha = player.room is null ? 0.0f : Custom.LerpMap(playerModule.HudFade, 0.5f, 1.0f, 0.0f, 0.4f);
        }

        for (var i = AllSymbols.Count - 1; i >= 0; i--)
        {
            var symbol = AllSymbols[i];
            
            if (symbol is null || symbol.SlatedForDeletion)
            {
                AllSymbols.RemoveAt(i);
            }

            symbol?.Draw(timeStacker);
        }
    }

    public override void ClearSprites()
    {
        foreach (var x in AllHUDCircles)
        {
            x.RemoveFromContainer();
        }

        AllHUDCircles.Clear();

        foreach (var x in AllSymbols)
        {
            x.SlatedForDeletion = true;
            x.RemoveSprites();
        }

        AllSymbols.Clear();
    }

    public override void Update()
    {
        if (hud.rainWorld.processManager.currentMainLoop is not RainWorldGame game)
        {
            return;
        }

        var allModules = game.GetAllDMDModules();

        if (allModules.All(x => x.HudFade < 0.001f))
        {
            HardSetPos = true;
            return;
        }

        List<InventorySymbol> updatedSymbols = [];

        foreach (var playerModule in allModules)
        {
            foreach (var item in playerModule.GunInventory)
            {
                UpdateSymbol(item, playerModule, updatedSymbols);
            }
        }

        var symbolsToClear = AllSymbols.Except(updatedSymbols);

        foreach (var symbol in symbolsToClear)
        {
            symbol.SlatedForDeletion = true;
        }
    }

    public void UpdateSymbol(AbstractPhysicalObject abstractObject, PlayerModule playerModule, List<InventorySymbol> updatedSymbols)
    {
        if (!Symbols.TryGetValue(abstractObject, out var symbol) || !AllSymbols.Contains(symbol))
        {
            if (symbol is not null)
            {
                Symbols.Remove(abstractObject);
            }

            symbol = new InventorySymbol(this, Vector2.zero);
            Symbols.Add(abstractObject, symbol);
            AllSymbols.Add(symbol);
        }

        if (updatedSymbols.Contains(symbol))
        {
            return;
        }

        symbol.UpdateIcon(abstractObject);
        symbol.Update();

        symbol.Fade = playerModule.PlayerRef?.room is null ? 0.0f : playerModule.HudFade;

        updatedSymbols.Add(symbol);
    }
}
