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
        InitSwapInput(ref tabIndex);
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

    private void InitSwapInput(ref int tabIndex)
    {
        AddTab(ref tabIndex, "Swap Input");

        AddDragger(SwapTriggerPlayer);
        DrawDraggers(ref Tabs[tabIndex], offsetX: 150.0f);

        AddNewLine();
        AddAndDrawLargeDivider(ref Tabs[tabIndex]);
        AddNewLine(-1);

        AddNewLine(4);

        AddAndDrawKeybinder(SwapLeftKeybind, ref Tabs[tabIndex]);
        AddAndDrawKeybinder(SwapRightKeybind, ref Tabs[tabIndex]);

        AddNewLine(-1);
        AddAndDrawLargeDivider(ref Tabs[tabIndex]);
        AddNewLine(2.5f);

        AddAndDrawKeybinder(SwapKeybindKeyboard, ref Tabs[tabIndex]);
        AddAndDrawKeybinder(SwapKeybindPlayer1, ref Tabs[tabIndex]);
        AddAndDrawKeybinder(SwapKeybindPlayer2, ref Tabs[tabIndex]);
        AddAndDrawKeybinder(SwapKeybindPlayer3, ref Tabs[tabIndex]);
        AddAndDrawKeybinder(SwapKeybindPlayer4, ref Tabs[tabIndex]);

        AddNewLine(-1.5f);

        DrawBox(ref Tabs[tabIndex]);
    }
}
