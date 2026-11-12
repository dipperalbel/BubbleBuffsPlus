using Kingmaker.Blueprints;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BubbleBuffs {

    /// <summary>
    /// Metamagic rods of Extend.
    ///
    /// In the game a rod is a toggle: while it is on, every suitable spell its owner casts is
    /// extended and spends one charge of the rod. A buff routine left to itself therefore
    /// spends the charges on whichever spells happen to be cast first. This class lets the
    /// routine decide: the rod is on for the casts that ask for it and off for the others.
    /// </summary>
    internal static class ExtendRods {

        /// <summary>
        /// The caster's Extend rods that can extend the spell, weakest rod first, so that a
        /// greater rod is kept for the spells only it can extend.
        /// </summary>
        public static IEnumerable<ActivatableAbility> For(UnitEntityData caster, AbilityData spell) {
            // The game spends a charge on a spell that is already extended and gives nothing for it.
            if (spell.HasMetamagic(Metamagic.Extend))
                return Enumerable.Empty<ActivatableAbility>();

            var cast = AsCast(spell);
            return caster.ActivatableAbilities.RawFacts
                .Select(ability => (ability, mechanics: Mechanics(ability)))
                .Where(rod => rod.mechanics != null && rod.mechanics.IsSuitableAbility(cast))
                .OrderBy(rod => rod.mechanics.MaxSpellLevel)
                .Select(rod => rod.ability);
        }

        /// <summary>True if the caster owns a rod that can extend the spell, charged or not.</summary>
        public static bool CanExtend(UnitEntityData caster, AbilityData spell) => For(caster, spell).Any();

        /// <summary>
        /// Picks the rod for one cast of a routine and takes one of its charges from
        /// <paramref name="chargesLeft"/>, which holds what the casts planned so far have left
        /// in each rod. Null if no suitable rod has a charge left.
        /// </summary>
        public static ActivatableAbility Reserve(UnitEntityData caster, AbilityData spell, Dictionary<ActivatableAbility, int> chargesLeft) {
            foreach (var rod in For(caster, spell)) {
                if (!chargesLeft.TryGetValue(rod, out int left))
                    left = rod.SourceItem?.Charges ?? 0;

                if (left > 0) {
                    chargesLeft[rod] = left - 1;
                    return rod;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the caster's Extend rods for one cast: <paramref name="rod"/> on (none if null),
        /// the others off. The rod has to be on when the cast rule is created, which is when
        /// Extend is applied to the spell, and when the rule triggers, which is when the charge
        /// is spent. Dispose the result to put every rod toggle back as it was.
        /// </summary>
        public static IDisposable Hold(UnitEntityData caster, ActivatableAbility rod) => new HeldRods(caster, rod);

        /// <summary>
        /// The ability a routine really casts, which is what the rod has to suit: for a touch
        /// spell its touch delivery, otherwise the spell itself.
        /// </summary>
        private static AbilityData AsCast(AbilityData spell) {
            var touch = spell.Blueprint.GetComponent<AbilityEffectStickyTouch>();
            return touch ? new AbilityData(spell, touch.TouchDeliveryAbility) : spell;
        }

        private static MetamagicRodMechanics Mechanics(ActivatableAbility ability) {
            var mechanics = ability.Blueprint.Buff?.GetComponent<MetamagicRodMechanics>();
            return mechanics != null && mechanics.Metamagic == Metamagic.Extend ? mechanics : null;
        }

        private sealed class HeldRods : IDisposable {
            // Every rod toggle, not only Extend: the game allows one active rod at a time, so
            // turning a rod on can turn another kind of rod off.
            private readonly List<(ActivatableAbility rod, bool wasOn)> before;

            public HeldRods(UnitEntityData caster, ActivatableAbility rod) {
                before = caster.ActivatableAbilities.RawFacts
                    .Where(ability => ability.Blueprint.Group == ActivatableAbilityGroup.MetamagicRod)
                    .Select(ability => (ability, ability.IsOn))
                    .ToList();

                foreach (var (other, _) in before) {
                    if (other != rod && Mechanics(other) != null)
                        other.IsOn = false;
                }

                if (rod != null)
                    rod.IsOn = true;
            }

            public void Dispose() {
                // Off first: turning one rod on while another is still on makes the game turn one of them off.
                foreach (var (rod, wasOn) in before.Where(entry => !entry.wasOn))
                    rod.IsOn = false;
                foreach (var (rod, wasOn) in before.Where(entry => entry.wasOn))
                    rod.IsOn = true;
            }
        }
    }
}
