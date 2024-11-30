﻿using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Common.QuestSystem.Quests
{
    internal class MysteriousPlacesV : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ItemID.Wood, 10);
            IsSideQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            return questPlayer.HasFinishedQuest(QuestLoader.GetInstance<MysteriousPlacesIV>());
        }
        public override void StartQuest(Player player)
        {
            base.StartQuest(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneIlluria;
        }

        public override void QuestIntroDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "SirestiasPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "MysteriousPlacesV";
        }
    }
}
