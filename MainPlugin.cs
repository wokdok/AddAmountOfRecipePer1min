using BepInEx;
using HarmonyLib;

namespace AddAmountOfRecipePer1min
{

    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInProcess("DSPGAME.exe")]
    public class MainPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.wokdok.plugins.addamountofrecipeper1min";
        public const string ModName = "AddAmountOfRecipePer1min";
        public const string ModVersion = "1.0.0";

        public void Awake()
        {
            var harmony = new Harmony("com.wokdok.plugins.addamountofrecipeper1min.patch");
            harmony.PatchAll(typeof(Patch_UIItemTip_SetTip));
            harmony.PatchAll(typeof(Patch_UIItemTip_OnDisable));
        }
    }
}
