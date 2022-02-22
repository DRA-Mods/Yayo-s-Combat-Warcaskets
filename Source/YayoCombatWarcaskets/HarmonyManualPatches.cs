using HarmonyLib;

namespace YayoCombatWarcaskets
{
    public static class HarmonyManualPatches
    {
        private static bool initialized = false;

        public static void ToggleBulletproof(Harmony harmony)
        {
            var type = AccessTools.TypeByName("yayoCombat.ArmorUtility");
            var targetMethod = AccessTools.Method(type, "GetPostArmorDamage");

            type = AccessTools.TypeByName("VFEAncients.PowerWorker_Blunt");
            var changeType = AccessTools.Method(type, "ChangeType");

            if (targetMethod == null || changeType == null) return;

            if (initialized)
                harmony.Unpatch(targetMethod, changeType);
            else 
                harmony.Patch(targetMethod, postfix: new HarmonyMethod(changeType));

            initialized = !initialized;
        }
    }
}
