using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Body)]
    public class ElagentBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Astolfo Breasts");
            /* Tooltip.SetDefault("Nya~"
				+ "\nYummy!" +
				"\n+20 Health"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Orange; // The rarity of the item
            Item.defense = 4; // The amount of defense the item will give when equipped
        }
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
        }
    }
}
