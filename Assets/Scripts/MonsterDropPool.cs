using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    Helmet,
    Chest,
    Legs,
    Gloves,
    Boots,
    Belt,
    Rings,
    Amulet,
    Weapon,
    Shield,
    ItemTypeCount
}
public struct DropTableItem
{
    public ItemTypes itemType;
    public float itemDropChance;
}
public struct MonsterDropTable
{
    public UnitType unitType;
    public List<DropTableItem> itemDropList;
}
public static class MonsterDropPool
{
    public static readonly List<MonsterDropTable> monsterDropTable = new List<MonsterDropTable>();

    static MonsterDropPool()
    {
        MonsterDropTable defenderDropTable = new MonsterDropTable
        {
            unitType = UnitType.Defender,
            itemDropList = new List<DropTableItem>()
            {
                new DropTableItem { itemType = ItemTypes.Helmet, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Chest, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Legs, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Gloves, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Boots, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Belt, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Rings, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Amulet, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Weapon, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Shield, itemDropChance = 0.0f }
            }
        };
        monsterDropTable.Add(defenderDropTable);
        MonsterDropTable attackerDropTable = new MonsterDropTable
        {
            unitType = UnitType.Attacker,
            itemDropList = new List<DropTableItem>()
            {
                new DropTableItem { itemType = ItemTypes.Helmet, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Chest, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Legs, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Gloves, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Boots, itemDropChance = 0.2f },
                new DropTableItem { itemType = ItemTypes.Belt, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Rings, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Amulet, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Weapon, itemDropChance = 0.0f },
                new DropTableItem { itemType = ItemTypes.Shield, itemDropChance = 0.0f }
            }
        };
        monsterDropTable.Add(attackerDropTable);
    }
}