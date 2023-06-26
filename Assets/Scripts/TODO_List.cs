public class TODO_List
{
    /* TODO List for all planned features/game concepts
     * 
     * GAME FEATURES
     * Generic game concepts:
     *  - Central crystal needs to be defended from charging waves of enemies
     *  - To obtain loot the player will need to enter in areas/zones similar to Melvor where a certain type of monster will be
     *      - Ex: Goblin village: wave1 (5 goblins) wave2 (7 goblins, 2 goblin brutes) wave3 (3 goblin brutes, 3 goblin archers)
     *  - Combat system will be a combat octagon (more dynamic than combat triangle)
     *      - Damage type ideas: Stab, Crush, Slash, Pierce, Fire, Earth, Water, Air, Shadow, Light
     *  - Can have up to your full party of 5 heros.
     * 
     * Zone/Dungeon:
     *  - Each zone has its own crystal with its own set of upgrades.
     *      - Could lock unique drops behind a certain crystal level.
     *      - Each time you lose a dungeon your crystal loses xp
     *      - individual crystal levels could provide things like +stats, +goldfind +magicfind
     *  - Each zone will have a Boss which will have to lured out by killing the stronghold for that area
     *      - Stronghold will be a defined area that each waves main force will come from.
     *      - Could have additional mini-strongholds around for new spawn points that will lure out mini-bosses (warchiefs UwU)
     *      - When a stronghold is first entered by hero units, a responding enemy militia will spawn to defend the base.
     *      - If the player does not provoke and defeat the stronghold before all the enemy waves have been released the stronghold will be locked down and not allowed to be fought again.
     *  - Cone shaped map with pre-defined road where all spawned monsters roughly follow on their way to your nexus.
     *  - Have tiers with the spawner at the end of the tiers zone.
     *  - The spawner will be defended by extra monsters that just sit and guard the spawners.
     *  - Killing a monster spawner will activate the next tiers spawner. Keep pushing until you reach the boss tier.
     *  - Goblin Zone:
     *      - Tier 1 monsters Level 1
     *      - Tier 2 monsters Level 50
     *      - Tier 3 monsters Level 70
     *  - Additional monsters throw around the map that will attack when within sight range.
     *      - Monsters that are around a spawner when it is attacked will rush to its aid.
     * 
     * Crystal Upgrades:
     *  - Auto-restart a given dungeon (cost gold, your units only pickup 50% of the items on the floor after boss)
     *  - Initially the crystal only holds x amount of items and gold; Upgrade to have it hold more and more items and gold
     * 
     * Hero Unit:
     *  - Spartan (javelin/sword/shield), Barbarian (dual-wield), Archer (bow)
     *  - Each hero would at least have normal attack and powerful attack that could knockdown enemies
     *  - Full set of diablo-like equiment, charm inventory, skill tree, activatable skills, attributes
     *  - Comes with squad of similar but weaker units
     *  - All squad units will have the same set of equipment but can be upgraded (similar to d2 mercenaries)
     *  
     * Loot:
     *  - Diablo-like items and stats (Helmet, chest, legs, boots, gloves, ring/ammy, charm inventory, runes, gems)
     *  - Equipment could have two sets of stats, one for when equip by your hero and another 'mercenary' stat set for all your squad units to use.
     *  - Quality tiers of equipment: White, Uncommon, Rare, Unique, Ethereal? Like super unique
     *  - Every type of monster will have a unique drop table (goblin can drop bronze spear, bow, eth runes)
     *  - Killing the monsters within the zone will have a chance to drop this loot
     *      - The players units will have to (automatically) walk over to obtain the loot and add it to the crystals wave inventory (which can be raked in at the end of the wave at the cost of ending the raid early)
     *  - Loot found during a wave can be extracted by hero but all other loot/gold/exp will be forfeited from this wave.
     *      - Could have some type of fortification system that would have your heros build up some type of defense around the crystal to extract before the final wave. (Could take a wave to setup and retreat)
     *  - Crystal will hold loot found during the wave as well as gold and exp.
     *  
     * Quality of Life:
     *  - DPS/Toughness %change comparison when looking at puting on a new item to your hero
     *  - DPS dummy.
     * 
     * Blender:
     *  - Character model for all classes
     *  - Monster models
     *  - Terrain objects to make each new area unique
     *  - Item models:
     *      - Armor (both equip and on floor)
     *      - Jewlery (on floor)
     *      - Gold (on floor)
     *      - Weapons (both equip and on floor)
     *  - Additional character animations for running, getting up, attacking with weapons, casting spells (abilities)
     * 
     * Experimental:
     *  - Co-op:
     *      - Local coop on something like steam could help eachother attack a region where the monsters are doubled and health/damage increased by x% (individual drops)
     *          - Individual Drops, Zone traveling UI disables for the Guest player. Host player will initiate all activities. (Maybe Guest needs to Ready up)
     *      - PvP a little arena to test out your might
     *      - Steam friend leaderboards
     *  - Solo Training:
     *      - No nexus and you just control a single character. Explore a map killing stuff more of an arcade mode.
     *  - Additional difficulties:
     *      - Hardcore evolution: 
     *          - Each time you defeat a Boss/Mini-Boss in a zone they remember how you beat them and adapt (nemesis system).
     *          - This intelligence will slowly be forgotten over X amount of time passed.
     *
     * UI:
     * Inventories:
     *  - Crystal Inventory
     *      - Holds obtained items, Gold and Exp for the current zone/waves
     *      - Bonus Tab Idea 1: Could build in a Charms inventory tab that applies stat buffs to all characters in that region. Maybe only certain stats will work in a crystal like %gold find or %magic find
     *      - Bonus Tab Idea 2: Could utilize runes to power-up the Crystal any apply buffs to all characters. (Maybe this consumes the runes and locks in a bonus to the slot)
     *      - Bonus Tab Idea 3: Could consume junk charms/gems/runes to power-up the crystal
     *  - Player Inventory
     *      - Reserved tab for Gems. When a gem is picked up it gets auto placed into its reserved slot.
     *      - Reserved tab for Runes. When a rune is picked up it gets auto placed into its reserved slot.
     *      - Reserved tab for Other Resources (Salvage Mats, Zone specific Mats). When a resource is picked up it gets auto placed into its reserved slot.     
     *      - Infinite item tabs.
     *      - Auto-sort button
     *      - Right clicking a tab lets you set an icon and pick a "Type". This type will allow you to mark a tab as "Charms".
     *          - Maybe we allow free typing a type otherwise preset tab Types: (Each Class name), (Each Item Slot name ie Charms/Ring/Amulet/Sword/Boots etc), (Each item Quality name ie White/Sockets/Magic/Rare/Unique), Early Game, Mid Game, End Game, Magic Find, Gold Find, Runewords
     *      - Tab filter in the UI. Can filter by tab Type (Filtering will hide the other tabs)
     *      - Tab search in the UI. Can search for an item. Any tab containing an item that satisfies the search is kept and other tabs are hidden
     *          - Could create a whole search language with some documentation in the UI. Example: "Nagelring (att:MF% 25-30)" or "[Ring] (att:DamageReduction% 0-)"
     *  - Top of UI Has all the heros. Click each hero to focus on their Equipment/Inventories.
     *  - Focused Hero display their Equipment slots or their Charm inventory
     *      - Two tabs at the top. First and default tab is the Hero's equiment. Second tab is the Hero's charms.
     * 
     * Cube/Crafting Inventory:
     *  - Need a place in the UI preferably within the main Inventory UI to drag items in and convert gems/runes up
     *  - The cube could be used to do crafting. Crafting ideas:
     *      - Add random sockets to item
     *      - Re-roll item attributes
     *      - Re-roll attribute rolls
     *      - Remove sockets (Returning the socketed items to you)
     *      - (Maybe a separate UI for this) Re-roll attribute line (Ber rune re-rolls line 1, Jah rune re-rolls line 2 etc)
     * 
     * Item Compare Display:
     *  - Bottom of the Compare Show 3 things
     *      - (+-) Damage Per Second (Needs to be an in-depth DPS calculation function that can be run for each item)
     *      - (+-) Effective Health (Factors Life, Defence, Resistances etc) (Needs to be an in-depth EHP calculation function that can be run for each item)
     *      - (+-) Display list of Lost/Gained Stats
     *      - (+-) Display list of Lost/Gained Unique special abilities (Make this stand out)
     * 
     * Hero Stats Display:
     *  - Display Hero Level and Exp
     *  - Display in the UI core stats such as: DPS, EHP, Resitances, Life, Mana?     
     *  - Similar to D4 give section that is a dynamic list of all stats and their totals
     */




    /*
     * Things to do next:
     * Gameplay options:
     *  - Work on only building up a dungeon/zone first (Goblin village for current scene)
     *    - Squad based AI
     *      - Option1
     *       - Each squad member must communicate with daddy hero whow ill be a sort of "SquadManager"
     *       - All squad managers and enemy scripts should also speak with eachother with some type of standard message that will define their current location/team (at a minimum)
     *      - Option2 (preferred)
     *       - Each entity on scene will use its UnitManager script to communicate with the Gamehandler, which will contain a list of all entities and a list of their most up-to-date location/health information.
     *       - All entities should be able to reference the Gamehandlers list to determine closest enemy to them or priorty targeting.
     *    - Overarching AI for defending Nexus/other Hero or for attacking Goblin base.
     *    - Put in placeholder Goblin stronghold and develop system for how the boss will be spawned
     *  - Combat system will be more of a combat octogon with elements (fire, water, earth, etc, shadow, light, physical)
     *  - Many different types of heros that can be recuited from some tavern type area (shitty 1-3 star heros can be bought and if they die in the battle field they are dead for good -- possibly human vs immortals)
     *  - Each hero (no matter the tier) will come with a squad of similar units, a hero skillset, and items for hero (diablo-like items).
     */
}
