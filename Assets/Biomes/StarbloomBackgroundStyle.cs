﻿using Terraria.ModLoader;

namespace Stellamod.Backgrounds
{
    public class StarbloomBackgroundStyle : ModSurfaceBackgroundStyle
    {
        // Use this to keep far Backgrounds like the mountains.
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot("Stellamod/Assets/Textures/Backgrounds/Starbloomflat");

        }


        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {


            scale = 0.66f;
            parallax = 0.3;
            //b = 1400;

            return BackgroundTextureLoader.GetBackgroundSlot("Stellamod/Assets/Textures/Backgrounds/StarbloomBiomeBackground");


        }


    }
}
