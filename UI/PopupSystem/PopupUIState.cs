﻿using Terraria.UI;

namespace Stellamod.UI.PopupSystem
{
    internal class PopupUIState : UIState
    {
        public PopupUI popupUI;
        public PopupUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            popupUI = new PopupUI();
            Append(popupUI);
        }
    }
}
