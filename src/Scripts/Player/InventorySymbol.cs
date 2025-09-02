using RWCustom;

namespace DMD;

public class InventorySymbol(InventoryHUD owner, Vector2 pos)
{
    public InventoryHUD Owner { get; } = owner;
    public ItemSymbol? ItemSymbol { get; set; }
    public WeakReference<AbstractPhysicalObject>? TargetObjectRef { get; set; }

    public Vector2 Pos { get; set; } = pos;
    public float Scale { get; set; } = 1.0f;
    public float Fade { get; set; } = 1.0f;
    public float DistFade { get; set; } = 1.0f;

    public float Flash { get; set; }

    public bool SlatedForDeletion { get; set; }

    public void UpdateIcon(AbstractPhysicalObject abstractObject)
    {
        if (TargetObjectRef is not null && TargetObjectRef.TryGetTarget(out var targetObject) && targetObject == abstractObject)
        {
            return;
        }

        TargetObjectRef = new(abstractObject);

        var iconData = new IconSymbol.IconSymbolData(CreatureTemplate.Type.StandardGroundCreature, abstractObject.type, 0);

        ItemSymbol?.RemoveSprites();
        ItemSymbol = new(iconData, Owner.HUDFContainer);

        ItemSymbol.Show(true);
        ItemSymbol.shadowSprite1.alpha = 0f;
        ItemSymbol.shadowSprite2.alpha = 0f;
    }

    public void RemoveSprites()
    {
        ItemSymbol?.RemoveSprites();
    }

    public void Update()
    {
        ItemSymbol?.Update();
    }

    public void Draw(float timeStacker)
    {
        if (SlatedForDeletion)
        {
            RemoveSprites();
            return;
        }

        if (ItemSymbol is null)
        {
            return;
        }

        if (TargetObjectRef is null || !TargetObjectRef.TryGetTarget(out var obj))
        {
            return;
        }

        ItemSymbol.Draw(timeStacker, Pos);
        ItemSymbol.symbolSprite.alpha = Fade * DistFade;

        ItemSymbol.symbolSprite.color = Color.Lerp(ItemSymbol.symbolSprite.color, Color.white, Custom.LerpMap(Flash, 2.5f, 5.0f, 0.0f, 1.0f));

        ItemSymbol.showFlash = Mathf.Lerp(ItemSymbol.showFlash, 0f, 0.1f);
        ItemSymbol.shadowSprite1.alpha = ItemSymbol.symbolSprite.alpha * 0.15f;
        ItemSymbol.shadowSprite2.alpha = ItemSymbol.symbolSprite.alpha * 0.4f;

        ItemSymbol.symbolSprite.scale = Scale;
        ItemSymbol.shadowSprite1.scale = Scale;
        ItemSymbol.shadowSprite2.scale = Scale;

        ItemSymbol.symbolSprite.scale *= Custom.LerpMap(Flash, 2.5f, 5.0f, 1.0f, 7.0f);

        ItemSymbol.shadowSprite1.element = Futile.atlasManager.GetElementWithName("dmd_hudshadow");
        ItemSymbol.shadowSprite2.element = Futile.atlasManager.GetElementWithName("dmd_hudshadow");
        //ItemSymbol.symbolSprite.element = Futile.atlasManager.GetElementWithName("dmd_hand");

        ItemSymbol.shadowSprite1.SetPosition(ItemSymbol.symbolSprite.GetPosition());
        ItemSymbol.shadowSprite2.SetPosition(ItemSymbol.symbolSprite.GetPosition());

        ItemSymbol.shadowSprite1.scale *= 0.12f;
        ItemSymbol.shadowSprite2.scale *= 0.2f;
        ItemSymbol.symbolSprite.scale *= 0.1f;

        ItemSymbol.shadowSprite1.color = Color.white;
        ItemSymbol.shadowSprite2.color = Color.black;
    }
}
