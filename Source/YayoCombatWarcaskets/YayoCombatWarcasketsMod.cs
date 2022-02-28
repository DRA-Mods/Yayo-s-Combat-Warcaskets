using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace YayoCombatWarcaskets
{
    [UsedImplicitly]
    public class YayoCombatWarcasketsMod : Mod
    {
        private static Harmony harmony;
        internal static Harmony Harmony => harmony ??= new Harmony("Dra.YayoCombatWarcaskets");
        public static YayoCombatWarcasketsSettings settings;

        public YayoCombatWarcasketsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<YayoCombatWarcasketsSettings>();

            Harmony.PatchAll();
            if (!Mathf.Approximately(settings.warcasketSpawnCostPercent, 1f))
                HarmonyManualPatches.ToggleWarcasketPointChange();
            if (settings.patchBulletproof)
                HarmonyManualPatches.ToggleBulletproof();

            var hasYayo = false;
            var hasVfe = false;

            foreach (var mod in LoadedModManager.RunningMods)
            {
                var id = mod.PackageId.ToLower().NoModIdSuffix();

                if (!hasYayo && id is "com.yayo.combat3")
                {
                    hasYayo = true;
                    if (hasVfe) break;
                }
                else if (!hasVfe && id is "oskarpotocki.vfe.pirates" or "vanillaexpanded.vfea")
                {
                    hasVfe = true;
                    if (hasYayo) break;
                }
            }

            if (!hasYayo)
                Log.Error("[Yayo's Combat Warcaskets] - no Yayo's Combat is running, having this mod enabled is pointless.");
            else if (!hasVfe)
                Log.Error("[Yayo's Combat Warcaskets] - no supported mod is running, having this mod enabled is pointless.");
        }

        public override void DoSettingsWindowContents(Rect inRect) => settings.DoSettingsWindowContents(inRect);

        public override string SettingsCategory() => "Yayo's Combat Warcaskets";
    }
}