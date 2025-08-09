using Menu.Remix.MixedUI;

namespace DMD;

public sealed partial class ModOptions
{
    public const int TAB_COUNT = 1;

    public static Color WarnRed { get; } = new(0.85f, 0.35f, 0.4f);


    public override void Initialize()
    {
        base.Initialize();

        Tabs = new OpTab[TAB_COUNT];
        var tabIndex = -1;

        InitGeneral(ref tabIndex);
    }

    private void InitGeneral(ref int tabIndex)
    {
        AddTab(ref tabIndex, "General");


        AddNewLine();

        AddTextLabel("Special thanks to the following people!", bigText: true);
        DrawTextLabels(ref Tabs[tabIndex]);


        AddNewLine();

        DrawTextLabels(ref Tabs[tabIndex]);

        AddNewLine();


        AddTextLabel("PLAYTESTERS", bigText: true);
        DrawTextLabels(ref Tabs[tabIndex]);

        AddNewLine();
        DrawTextLabels(ref Tabs[tabIndex]);


        AddNewLinesUntilEnd();
        DrawBox(ref Tabs[tabIndex]);
    }
}
