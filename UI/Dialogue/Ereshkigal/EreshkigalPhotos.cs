﻿namespace Stellamod.UI.Dialogue
{
    internal class EreshkigalPhotos : Dialogue
    {
        //The number of steps in this dialogue
        public override int Length => 4;

        public override void Next(int index)
        {
            base.Next(index);

            //This starts the dialogue
            switch (index)
            {
                case 0:
                    //Set the texture of the portrait
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/EreshkigalDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("EreshTalk1"));
                    break;

                case 1:
                    //Set the texture of the portrait

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("EreshTalk2"));
                    break;

                case 2:
                    //Set the texture of the portrait

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("EreshTalk3"));
                    break;

                case 3:
                    //Set the texture of the portrait

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("EreshTalk4"));
                    break;
            }
        }

        public override void Update(int index)
        {
            base.Update(index);
            //If you want stuff to happen while they're talking you can do it here ig
            //But that might not be a good idea since you can just speed through dialogues
        }

        public override void Complete()
        {

            //Do something when the dialogue is completely finished


            base.Complete();
        }




    }
}
