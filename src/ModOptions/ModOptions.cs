namespace DMD;

public sealed partial class ModOptions : OptionsTemplate
{
    public static ModOptions Instance { get; } = new();

    public static void RegisterOI()
    {
        if (MachineConnector.GetRegisteredOI(Plugin.MOD_ID) != Instance)
        {
            MachineConnector.SetRegisteredOI(Plugin.MOD_ID, Instance);
        }
    }


    // SWAP
    public static Configurable<KeyCode> SwapLeftKeybind { get; } = Instance.config.Bind(nameof(SwapLeftKeybind), KeyCode.A, new ConfigurableInfo(
        "Keybind to swap to the stored pearl to the left. Limited to Player 1.", null, "", "Swap Left"));

    public static Configurable<KeyCode> SwapRightKeybind { get; } = Instance.config.Bind(nameof(SwapRightKeybind), KeyCode.D, new ConfigurableInfo(
        "Keybind to swap to the stored pearl to the right. Limited to Player 1.", null, "", "Swap Right"));


    public static Configurable<KeyCode> SwapKeybindKeyboard { get; } = Instance.config.Bind(nameof(SwapKeybindKeyboard), KeyCode.LeftAlt, new ConfigurableInfo(
        "Keybind for Keyboard.", null, "", "Keyboard"));

    public static Configurable<KeyCode> SwapKeybindPlayer1 { get; } = Instance.config.Bind(nameof(SwapKeybindPlayer1), KeyCode.Joystick1Button3, new ConfigurableInfo(
        "Keybind for Player 1.", null, "", "Player 1"));

    public static Configurable<KeyCode> SwapKeybindPlayer2 { get; } = Instance.config.Bind(nameof(SwapKeybindPlayer2), KeyCode.Joystick2Button3, new ConfigurableInfo(
        "Keybind for Player 2.", null, "", "Player 2"));

    public static Configurable<KeyCode> SwapKeybindPlayer3 { get; } = Instance.config.Bind(nameof(SwapKeybindPlayer3), KeyCode.Joystick3Button3, new ConfigurableInfo(
        "Keybind for Player 3.", null, "", "Player 3"));

    public static Configurable<KeyCode> SwapKeybindPlayer4 { get; } = Instance.config.Bind(nameof(SwapKeybindPlayer4), KeyCode.Joystick4Button3, new ConfigurableInfo(
        "Keybind for Player 4.", null, "", "Player 4"));


    public static Configurable<int> SwapTriggerPlayer { get; } = Instance.config.Bind(nameof(SwapTriggerPlayer), 1, new ConfigurableInfo(
        "Which player controller trigger swapping will apply to. 0 disables trigger swapping. Negative player numbers invert the triggers. Hold and drag up or down to change.",
        new ConfigAcceptableRange<int>(-4, 4), "",
        "Trigger Player"));
}
