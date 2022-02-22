using UnityEngine;
using Verse;

namespace YayoCombatWarcaskets
{
    public class YayoCombatWarcasketsSettings : ModSettings
    {
        public float forcedWarcasketDurabilityPercent = 1f;

        public bool patchBulletproof = true;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref forcedWarcasketDurabilityPercent, nameof(forcedWarcasketDurabilityPercent), 1f);
            Scribe_Values.Look(ref patchBulletproof, nameof(patchBulletproof), true);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.ColumnWidth = 270f;

            var text = forcedWarcasketDurabilityPercent switch
            {
                <= 0.0f => "WarcasketArmorStrengthAir",
                <= 0.2f => "WarcasketArmorStrengthPaper",
                <= 0.5f => "WarcasketArmorStrengthWood",
                <= 0.7f => "WarcasketArmorStrengthSteel",
                <= 1.0f => "WarcasketArmorStrengthSkySteel",
                <= 2.0f => "WarcasketArmorStrengthPlasteel",
                <= 3.0f => "WarcasketArmorStrengthArchotech",
                <= 4.0f => "WarcasketArmorStrengthAlphaPoly",
                _ => "WarcasketArmorStrengthBetaPoly",
            };
            listing.Label($"{"WarcasketArmorStrength".Translate()} {forcedWarcasketDurabilityPercent * 100:f0}%");
            listing.Label(text.Translate());
            forcedWarcasketDurabilityPercent = GenMath.RoundTo(listing.Slider(forcedWarcasketDurabilityPercent, 0f, 5f), 0.05f);

            var temp = patchBulletproof;
            listing.CheckboxLabeled("WarcasketPatchBulletproof".Translate(), ref patchBulletproof, "WarcasketPatchBulletproofTooltip".Translate());
            if (temp != patchBulletproof)
                HarmonyManualPatches.ToggleBulletproof(YayoCombatWarcasketsMod.Harmony);

            listing.End();
        }
    }
}
