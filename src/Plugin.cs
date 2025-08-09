global using UnityEngine;
global using Random = UnityEngine.Random;

using BepInEx;
using BepInEx.Logging;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: IgnoresAccessChecksTo("Assembly-CSharp")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
#pragma warning restore CS0618 // Type or member is obsolete

namespace DMD;

[BepInDependency("slime-cubed.slugbase")] // SlugBase

[BepInDependency("com.dual.improved-input-config", BepInDependency.DependencyFlags.SoftDependency)] // Improved Input Config

[BepInPlugin(MOD_ID, MOD_ID, "1.5.2")]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "dmd";

    public static string ModName { get; set; } = "";
    public static string Version { get; set; }= "";
    public static string Author { get; set; } = "";

    public new static ManualLogSource Logger { get; private set; } = null!;

    public void OnEnable()
    {
        Logger = base.Logger;
        Hooks.ApplyInitHooks();
    }
}
