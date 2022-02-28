using HarmonyLib;
using RimWorld;
using Verse;

namespace YayoCombatWarcaskets
{
    public static class HarmonyManualPatches
    {
        public static bool WarcasketCostInitialized { get; private set; } = false;
        public static bool BulletproofInitialized { get; private set; } = false;

        public static void ToggleWarcasketPointChange()
        {
            var targetMethod = AccessTools.PropertyGetter(typeof(PawnGenOption), nameof(PawnGenOption.Cost));
            var ourMethod = AccessTools.Method(typeof(HarmonyManualPatches), nameof(PostCostCalculation));

            if (targetMethod == null || ourMethod == null) return;

            if (WarcasketCostInitialized)
                YayoCombatWarcasketsMod.Harmony.Unpatch(targetMethod, ourMethod);
            else
                YayoCombatWarcasketsMod.Harmony.Patch(targetMethod, postfix: new HarmonyMethod(ourMethod));
            WarcasketCostInitialized = !WarcasketCostInitialized;
        }

        public static void ToggleBulletproof()
        {
            var type = AccessTools.TypeByName("yayoCombat.ArmorUtility");
            var targetMethod = AccessTools.Method(type, "GetPostArmorDamage");

            type = AccessTools.TypeByName("VFEAncients.PowerWorker_Blunt");
            var changeType = AccessTools.Method(type, "ChangeType");

            if (targetMethod == null || changeType == null) return;

            if (BulletproofInitialized)
                YayoCombatWarcasketsMod.Harmony.Unpatch(targetMethod, changeType);
            else
                YayoCombatWarcasketsMod.Harmony.Patch(targetMethod, postfix: new HarmonyMethod(changeType));

            BulletproofInitialized = !BulletproofInitialized;
        }

        private static void PostCostCalculation(PawnKindDef ___kind, ref float __result)
        {
            if (___kind.apparelTags == null)
                return;

            if (___kind.apparelTags.Any(x => x.Contains("Warcasket")))
                __result *= YayoCombatWarcasketsMod.settings.warcasketSpawnCostPercent;
        }
    }
}
