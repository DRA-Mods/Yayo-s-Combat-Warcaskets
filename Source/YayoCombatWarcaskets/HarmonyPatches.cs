using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace YayoCombatWarcaskets
{
    internal static class HarmonyPatches
    {
        [HarmonyPatch]
        private static class PatchWarcasketDurability
        {
            private static Type warcasketType;

            [UsedImplicitly]
            private static bool Prepare()
            {
                warcasketType = AccessTools.TypeByName("VFEPirates.WarcasketDef");
                return warcasketType != null;
            }

            [UsedImplicitly]
            private static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("yayoCombat.ArmorUtility");
                return AccessTools.Method(type, "ApplyArmor");
            }

            [UsedImplicitly]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var target = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.HitPoints));
                var replacement = AccessTools.Method(typeof(PatchWarcasketDurability), nameof(GetWarcasketDurability));

                foreach (var ci in instructions)
                {
                    if (ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo method && method == target)
                    {
                        ci.opcode = OpCodes.Call;
                        ci.operand = replacement;
                        Log.Message("[Yayo's Combat Warcaskets] - Found method to patch");
                    }

                    yield return ci;
                }
            }

            private static float GetWarcasketDurability(Thing armor)
            {
                if (armor.def.GetType().IsAssignableFrom(warcasketType))
                    return armor.MaxHitPoints * YayoCombatWarcasketsMod.settings.forcedWarcasketDurabilityPercent;
                return armor.HitPoints;
            }
        }
    }
}
