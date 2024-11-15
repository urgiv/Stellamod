﻿using Microsoft.Xna.Framework;
using Stellamod.Effects;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Lights
{
    internal class SpecialEffectsPlayer : ModPlayer
    {
        private bool _init;
        private float _vignetteOpacity;
        private float _vignetteStrength;

        private float _targetVignetteStrength;
        private float _targetVignetteOpacity;

        private Color[] _abyssPalette;
        private Color[] _alcadPalette;
        private Color[] _underworldPalette;
        private Color[] _desertPalette;
        private Color[] _witchTownPalette;
        private Color[] _rustyPalette;
        private Color[] _bloodyPalette;
        private Color[] _dungeonPalette;

        private MyPlayer MyPlayer => Player.GetModPlayer<MyPlayer>();

        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        private string DarknessVignette => "LunarVeil:DarknessVignette";

        public bool hasSpiritPendant;
        public bool hasSunGlyph;
        public float darkness;
        public float darknessCurve;
        public float whiteCurve;
        public float blackCurve;


        //Progress Variables
        public float abyssPaletteProgress;
        public float hellPaletteProgress;
        public float dungeonPaletteProgress;
        public float royalPaletteProgress;
        public float desertPaletteProgress;
        public float bloodCathedralPaletteProgress;

        private void LoadPalettes()
        {
            string rootDirectory = "Common/Lights/Palettes";
            _abyssPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Abyss");
            _alcadPalette = PalFileImporter.ReadPalette($"{rootDirectory}/RoyalCapital");
            _underworldPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Hell");
            _desertPalette = PalFileImporter.ReadPalette($"{rootDirectory}/maggot24");
            _witchTownPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Witchtown");
            _rustyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Rusty");
            _bloodyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/bloodmoon21");
            _dungeonPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Dungeon");
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSpiritPendant = false;
            hasSunGlyph = false;
            darkness = 0;
            darknessCurve = 0.79f;


            //Curve based
            float progress = (float)(Main.LocalPlayer.position.ToTileCoordinates().Y - Main.worldSurface) / 1000;
            progress = MathHelper.Clamp(progress, 0, 1);
            darknessCurve = MathHelper.Lerp(0f, darknessCurve, progress);

            whiteCurve = 0f;
            blackCurve = 1f;

            _targetVignetteOpacity = 1f;
        }

        private void TogglePaletteShader(string name, bool isActive)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (isActive)
            {
                if (!FilterManager[name].IsActive())
                {
                    FilterManager.Activate(name);
                }
            } else if (!isActive)
            {
                if (FilterManager[name].IsActive())
                {
                    FilterManager.Deactivate(name);
                }
            }
        }

        private void SpecialBiomeEffects()
        {
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            ScreenShaderData screenShaderData;
            bool abyssPaletteActive = MyPlayer.ZoneAbyss || MyPlayer.ZoneAurelus || MyPlayer.ZoneMechanics || MyPlayer.ZoneIshtar;
            if (abyssPaletteActive)
            {
                darkness += 2;
            }
            if (abyssPaletteActive)
            {
                abyssPaletteProgress += 0.01f;
            }
            else
            {
                abyssPaletteProgress -= 0.01f;
            }
            abyssPaletteProgress = MathHelper.Clamp(abyssPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteAbyss"].GetShader();
            screenShaderData.UseProgress(abyssPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteAbyss", abyssPaletteProgress != 0);

            bool hellPaletteActive = ((clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneUnderworldHeight)
                || MyPlayer.ZoneCinder || MyPlayer.ZoneDrakonic);
            if (hellPaletteActive)
            {
                darkness += 1;
            }
            if (hellPaletteActive)
            {
                hellPaletteProgress += 0.01f;
            }
            else
            {
                hellPaletteProgress -= 0.01f;
            }
            hellPaletteProgress = MathHelper.Clamp(hellPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteHell"].GetShader();
            screenShaderData.UseProgress(hellPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteHell", hellPaletteProgress != 0);

            bool royalCapitalPaletteActive = MyPlayer.ZoneAlcadzia;
            if (royalCapitalPaletteActive)
            {
                royalPaletteProgress += 0.01f;
            }
            else
            {
                royalPaletteProgress -= 0.01f;
            }
            royalPaletteProgress = MathHelper.Clamp(royalPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteRoyalCapital"].GetShader();
            screenShaderData.UseProgress(royalPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteRoyalCapital", royalPaletteProgress != 0);

            bool dungeonPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDungeon;
            if (dungeonPaletteActive)
            {
                dungeonPaletteProgress += 0.01f;
            }
            else
            {
                dungeonPaletteProgress -= 0.01f;
            }
            dungeonPaletteProgress = MathHelper.Clamp(dungeonPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteDungeon"].GetShader();
            screenShaderData.UseProgress(dungeonPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteDungeon", dungeonPaletteProgress != 0);

            bool desertPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDesert;
            if (desertPaletteActive)
            {
                desertPaletteProgress += 0.01f;
            }
            else
            {
                desertPaletteProgress -= 0.01f;
            }
            desertPaletteProgress = MathHelper.Clamp(desertPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteDesert"].GetShader();
            screenShaderData.UseProgress(desertPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteDesert", desertPaletteProgress != 0);

            bool bloodPaletteActive = MyPlayer.ZoneBloodCathedral && !Main.dayTime;
            if (bloodPaletteActive)
            {
                bloodCathedralPaletteProgress += 0.01f;
            }
            else
            {
                bloodCathedralPaletteProgress -= 0.01f;
            }
            bloodCathedralPaletteProgress = MathHelper.Clamp(bloodCathedralPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteBloodCathedral"].GetShader();
            screenShaderData.UseProgress(bloodCathedralPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteBloodCathedral", bloodCathedralPaletteProgress != 0);

            CalculateDarkness();
            TogglePaletteShader("LunarVeil:DarknessVignette", darkness != 0);

            screenShaderData = FilterManager["LunarVeil:DarknessCurve"].GetShader();
            screenShaderData.UseProgress(darknessCurve);
            screenShaderData.Shader.Parameters["blackCurve"].SetValue(blackCurve);
            screenShaderData.Shader.Parameters["whiteCurve"].SetValue(whiteCurve);
            TogglePaletteShader("LunarVeil:DarknessCurve", darknessCurve != 0);
        }
        
        private void CalculateDarkness()
        {
            if (hasSpiritPendant)
            {
                darkness -= 0.5f;
            }
            if (hasSunGlyph)
            {
                darkness -= 0.5f;
            }
            if (darkness <= 0)
            {
                darkness = 0;
            }
            _targetVignetteStrength = darkness;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            //Darkness
            if (!_init)
            {
                LoadPalettes();
                _init = true;
            }

            SpecialBiomeEffects();
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            bool useVignette = darkness != 0;
            if (useVignette)
            {
                if (!FilterManager[DarknessVignette].IsActive())
                {
                    FilterManager.Activate(DarknessVignette);
                }

                _vignetteStrength = MathHelper.Lerp(_vignetteStrength, _targetVignetteStrength, 0.1f);
                _vignetteOpacity = MathHelper.Lerp(_vignetteOpacity, _targetVignetteOpacity, 0.1f);
                var shaderData = FilterManager[DarknessVignette].GetShader();
                shaderData.UseProgress(_vignetteStrength);
                shaderData.UseOpacity(_vignetteOpacity);
            }
            else
            {
                if (_vignetteStrength != 0)
                {
                    _vignetteOpacity = MathHelper.Lerp(_targetVignetteOpacity, 0, 0.1f);
                    _vignetteStrength = MathHelper.Lerp(_vignetteStrength, 0, 0.1f);
                    var shaderData = FilterManager[DarknessVignette].GetShader();
                    shaderData.UseProgress(_vignetteStrength);
                    shaderData.UseOpacity(_vignetteOpacity);
                }
                else
                {
                    if (FilterManager[DarknessVignette].IsActive())
                    {
                        FilterManager.Deactivate(DarknessVignette);
                    }
                }
            }
        }
    }
}
