﻿
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Assets.Biomes
{
    public class AcidBiome : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Terors") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Nightmares");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/AcidWaterStyle");

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/MarrowSurfaceBackgroundStyle");

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAcid;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = false;
    }
}