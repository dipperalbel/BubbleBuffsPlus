# Bubble Buffs Plus

A fork of [Bubble Buffs](https://github.com/factubsio/BubbleBuffs) (on Nexus Mods: [Bubbles Ultimate Buff Bot Lite Extra Simple](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/195)), the mod for Pathfinder: Wrath of the Righteous that adds buff routines to the spellbook screen. You decide once who casts which buff on whom, then cast them all with one button.

Bubble Buffs Plus does everything the original does and adds the changes below. Download it from [Nexus Mods](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/1268).

## What is different from Bubble Buffs

- **Abilities of equipped items that spend charges.** A cloak that casts Heroism once a day, a signet ring and similar items are listed in the **Items** tab and can be part of a routine. The original leaves these abilities out. An item that recharges when the party rests counts for all its charges; an item that does not recharge counts for the charges it has left. Potions, scrolls and wands are not listed.
- **Metamagic rod of Extend.** In the popout with the options of a caster there is a new one, **Extend with a metamagic rod**. It can be ticked when the caster owns a rod of Extend that suits the spell. The routine turns the rod on for the buffs you ticked and puts the rod toggles back as they were when it is done. With several rods it takes the weakest one that suits the spell, so that a greater rod is kept for the spells only it can extend. When the rods run out of charges, the buff is cast without one. While a routine runs, the caster's Extend rods are off for the buffs you did not tick, so no charge is spent on them.
- **No Consumables tab.** The tab was always empty, so it is gone.
- **A mod of its own.** Bubble Buffs Plus has its own id and folder (`BubbleBuffsPlus`). It replaces Bubble Buffs and does not load while the original is enabled.

## Installation

1. Install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21), version 0.23.0 or later, and set it up for Wrath of the Righteous.
2. If you have Bubble Buffs, switch it off in Unity Mod Manager or remove it.
3. Install the Bubble Buffs Plus zip with Unity Mod Manager, or extract it so that the files are in `<game folder>\Mods\BubbleBuffsPlus\`.

To keep the buff setups you made with Bubble Buffs, copy the `bubblebuff-*.json` files from `<game folder>\Mods\BubbleBuffs\UserSettings\` to `<game folder>\Mods\BubbleBuffsPlus\UserSettings\`. There is one file for each playthrough.

## Building

You need the game installed and the .NET SDK (the project is built here with version 9).

```powershell
.\build.ps1            # builds and writes dist\BubbleBuffsPlus-<version>.zip
.\build.ps1 -Install   # also copies the build into the game's Mods folder (close the game first)
```

`build.ps1` looks for the game in the default Steam folder. For another location, put a `GamePath.props` file next to `BubbleBuffs.sln`:

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <WrathInstallDir>D:\Games\Pathfinder Second Adventure</WrathInstallDir>
  </PropertyGroup>
</Project>
```

The version comes from `BubbleBuffs\Info.json`. The [original README](https://github.com/factubsio/BubbleBuffs#debugging) explains how to attach a debugger to the game.

## Credits

Bubble Buffs is the work of bubbles ([factubsio](https://github.com/factubsio)) and of the people who contributed to it; this fork only adds the changes listed above. The acknowledgments of the original:

- @Balkoth for Buffbot, the direct inspiration for Bubble Buffs
- @Vek17, whose project the mod was started from
- the Discord members who tested the early versions, and the Pathfinder: Wrath of the Righteous Discord channel members

Fork by Alberto Bellumat.

## License

MIT, as the original. See [LICENSE](LICENSE).
