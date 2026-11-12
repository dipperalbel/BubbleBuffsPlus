using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;

namespace BubbleBuffs {

    /// <summary>
    /// Abilities that come from an equipped item and spend its charges, such as a cloak that
    /// casts Heroism once a day. The original mod left every ability of this kind out.
    /// </summary>
    internal static class ItemCharges {

        /// <summary>
        /// True for worn equipment that spends charges. Potions, scrolls and wands also spend
        /// charges but are consumables: they stay out of the buff lists.
        /// </summary>
        public static bool IsChargedEquipment(ItemEntity item) {
            if (item == null || !item.IsSpendCharges)
                return false;

            return item.Blueprint is not BlueprintItemEquipmentUsable usable || usable.Type == UsableItemType.Other;
        }

        /// <summary>
        /// How many casts the planner may count on. An item that recharges on rest counts for
        /// its full charges, the way a spellbook counts for its full slots: the plan describes a
        /// rested party, and the cast itself checks what is left. An item that does not
        /// recharge counts for the charges it still has.
        /// </summary>
        public static int PlannedCasts(ItemEntity item) {
            if (item.Blueprint is BlueprintItemEquipment equipment && equipment.RestoreChargesOnRest)
                return equipment.Charges;

            return item.Charges;
        }
    }
}
