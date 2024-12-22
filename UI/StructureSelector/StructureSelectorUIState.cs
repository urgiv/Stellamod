﻿using Terraria.UI;

namespace Stellamod.UI.StructureSelector
{
    internal class StructureSelectorUIState : UIState
    {
        public StructureSelectorUI ui;
        public StructureSelectorUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new StructureSelectorUI();
            Append(ui);
        }
    }
}
