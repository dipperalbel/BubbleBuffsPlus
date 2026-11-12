## Version 5.3.1
* (Bug Fix) An item whose ability is a choice among several buffs, such as the Signet of House Vespertilio, let a routine cast more than one of them on a single charge. The choices now share the charges of the item.
* (Bug Fix) The same for any buff that only targets its caster: it no longer counts as castable when the spell slots it shares with other buffs are all taken, which could happen with spontaneous casters and arcanists.
* (Bug Fix) A routine no longer casts from an item more times than the item has charges left; the casts it leaves out are reported as failed with "no charges left on the item".

## Version 5.3.0
First version of Bubble Buffs Plus, a fork of Bubble Buffs 5.2.3.
* (Feature) Abilities of equipped items that spend charges (a cloak that casts Heroism once a day, a signet ring) are listed in the Items tab and can be part of a routine. Potions, scrolls and wands are not listed.
* (Feature) New caster option "Extend with a metamagic rod": the routine turns the caster's rod of Extend on for the buffs that ask for it and puts the rod toggles back afterwards.
* (Feature) Removed the Consumables tab, which was always empty.
* (Bug Fix) The caster options no longer fail on an ability that does not come from a spellbook.
* (Maintenance) The mod has its own id, folder and assembly (BubbleBuffsPlus) and does not load while the original Bubble Buffs is enabled.
* (Maintenance) Added build.ps1; a build no longer copies itself into the game's Mods folder unless asked to.

## Version 5.1.4
* Fixes and stuff

## Version 5.1.0
(No idea what happened in the other versions...)
* (Feature) Added ability to control the Azata Zippy Magic Secondary Buff.  This option is enabled if a character chooses the Azata Mythic Path and selects the Zippy Magic superpower.  With the option checked, every second cast of the chosen buff is free.  This allows using this mod in a controlled manner with Zippy Magic.
* (Feature) Implemented enhancements to Powerful Change / Share Transmutation if character is a Level 20 Brown-Fur Transmuter with Transmutation Supremacy
* (Feature) Prevent spending an Arcane Reservoir point when using Share Transmutation on self
* (Bug Fix) Buffs from Opposition Schools did not correctly count the number of characters that could be targeted.
* (Bug Fix) Remove fixed paths to D: in the csproj file
* (Bug Fix) Add missing \ for InputLegacyModule path in csproj file
* (Bug Fix) Add missing assets in Assets\icons folder
* (Bug Fix) Update WrathPath environment variable to WrathPathDebug in csproj file.  No need for two environment variables.
* (Maintenance) Update changelog
* (Maintenance) Increment Version Number
* (Quality of Life) Update Readme.md to be helpful for working with project


## Version 0.2.0

* Added "disband army" button
* Fixed a bug where the global map speed would not change until the map was loaded/unloaded.

## Version 0.1.0

* Added speed controls for combat, global map, and tactical battles
