﻿using Stellamod.UI.PopupSystem;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.QuestSystem
{
    internal class QuestPlayer : ModPlayer
    {
        private List<Quest> _activeQuests;
        private List<Quest> _completedQuests;
        private List<Quest> _rewardQuests;
        public List<Quest> ActiveQuests
        {
            get
            {
                _activeQuests ??= new List<Quest>();
                _activeQuests.Sort((x, y) => x.IsSideQuest.CompareTo(y.IsSideQuest));
                return _activeQuests;
            }
            private set
            {
                _activeQuests = value;
            }
        }

        public List<Quest> CompletedQuests
        {
            get
            {
                _completedQuests ??= new List<Quest>();
                return _completedQuests;
            }
            private set
            {
                _completedQuests = value;
            }
        }

        public List<Quest> RewardQuests
        {
            get
            {
                _rewardQuests ??= new List<Quest>();
                return _rewardQuests;
            }
            private set
            {
                _rewardQuests = value;
            }
        }

        public bool RecalculateUI { get; set; }
        public bool HasQuest(Quest quest)
        {
            return ActiveQuests.Contains(quest);
        }
        public bool CompletedQuest(Quest quest)
        {
            return CompletedQuests.Contains(quest);
        }

        public bool GiveQuest(Quest quest)
        {
            if (HasQuest(quest) || CompletedQuest(quest))
                return false;
            if (!quest.CanGiveQuest(Player))
                return false;

            ActiveQuests.Add(quest);
            quest.StartQuest(Player);
            PopupUISystem popupUISystem = ModContent.GetInstance<PopupUISystem>();
            popupUISystem.OpenUI("NewQuest");
            RecalculateUI = true;
            return true;
        }

        public void CompleteQuest(Quest quest)
        {
            if (!ActiveQuests.Contains(quest))
                return;
            if (RewardQuests.Contains(quest))
                return;
            ActiveQuests.Remove(quest);
            RewardQuests.Add(quest);
 
            PopupUISystem popupUISystem = ModContent.GetInstance<PopupUISystem>();
            popupUISystem.OpenUI("CompleteQuest");
            RecalculateUI = true;
        }

        public void CollectQuestReward(Quest quest)
        {
            if (!RewardQuests.Contains(quest))
                return;
            if (CompletedQuests.Contains(quest))
                return;
     
            CompletedQuests.Add(quest);
            RewardQuests.Remove(quest);
            quest.Reward(Player);
            RecalculateUI = true;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["activeQuests"] = ActiveQuests;
            tag["completedQuests"] = CompletedQuests;
            tag["rewardQuests"] = RewardQuests;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            ActiveQuests = tag.Get<List<Quest>>("activeQuests");
            CompletedQuests = tag.Get<List<Quest>>("completedQuests");
            RewardQuests = tag.Get<List<Quest>>("rewardQuests");
        }
    }
}
