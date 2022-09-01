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
            var hasVfeAncients = false;
            var hasVfePirates = false;
            var foundMods = 0;
            const int maxMods = 3;

            foreach (var mod in LoadedModManager.RunningMods)
            {
                var id = mod.PackageId.ToLower().NoModIdSuffix();

                if (!hasYayo && id is "com.yayo.combat3" or "mlie.yayoscombat3")
                {
                    hasYayo = true;
                    if (++foundMods == maxMods) break;
                }
                else if (!hasVfeAncients && id is "vanillaexpanded.vfea")
                {
                    hasVfeAncients = true;
                    if (++foundMods == maxMods) break;
                }
                else if (!hasVfePirates && id is "oskarpotocki.vfe.pirates")
                {
                    hasVfePirates = true;
                    if (++foundMods == maxMods) break;
                }
            }

            switch (hasYayo, hasVfeAncients, hasVfePirates)
            {
                case (false, _, true):
                    Log.Warning("[Yayo's Combat Warcaskets] - no Yayo's Combat is running, having this mod enabled is pointless unless you're changing warcasket raid point cost.");
                    break;
                case (false, _, _):
                    Log.Error("[Yayo's Combat Warcaskets] - no Yayo's Combat is running, having this mod enabled is pointless.");
                    break;
                case (true, false, false):
                    Log.Error("[Yayo's Combat Warcaskets] - no supported VFE mod is running, having this mod enabled is pointless.");
                    break;
            }
        }

        public override void DoSettingsWindowContents(Rect inRect) => settings.DoSettingsWindowContents(inRect);

        public override string SettingsCategory() => "Yayo's Combat Warcaskets";
    }
}