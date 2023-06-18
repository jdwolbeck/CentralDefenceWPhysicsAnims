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
    ItemTypeCount
}
public struct DropTableItem
{
    public ItemTypes itemType;
    public float itemDropChance;
}
public struct MobDropTable
{
    public EntityType entityType;
    public List<DropTableItem> itemDropList;
}
public static class MobDropPool
{
    public static readonly List<MobDropTable> mobDropTable = new List<MobDropTable>();

    static MobDropPool()
    {
        for (int i = 0; i < (int)EntityType.EntityCount; i++)
        {
            switch ((EntityType)i)
            {
                case EntityType.Hero:
                    MobDropTable heroDropTable = new MobDropTable
                    {
                        entityType = EntityType.Hero,
                        itemDropList = new List<DropTableItem>()
                    {
                        new DropTableItem { itemType = ItemTypes.Helmet, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Chest, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Legs, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Gloves, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Boots, itemDropChance = 0.2f }
                    }
                    };
                    MobDropPool.mobDropTable.Add(heroDropTable);
                    break;
                case EntityType.Mob:
                    MobDropTable mobDropTable = new MobDropTable
                    {
                        entityType = EntityType.Mob,
                        itemDropList = new List<DropTableItem>()
                    {
                        new DropTableItem { itemType = ItemTypes.Helmet, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Chest, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Legs, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Gloves, itemDropChance = 0.2f },
                        new DropTableItem { itemType = ItemTypes.Boots, itemDropChance = 0.2f }
                    }
                    };
                    MobDropPool.mobDropTable.Add(mobDropTable);
                    break;
                case EntityType.Mercenary:
                    MobDropTable mercDropTable = new MobDropTable
                    {
                        entityType = EntityType.Mercenary,
                        itemDropList = new List<DropTableItem>()
                    {
                        new DropTableItem { itemType = ItemTypes.Helmet, itemDropChance = 0.6f },
                        new DropTableItem { itemType = ItemTypes.Chest, itemDropChance = 0.3f },
                        new DropTableItem { itemType = ItemTypes.Legs, itemDropChance = 0.05f },
                        new DropTableItem { itemType = ItemTypes.Gloves, itemDropChance = 0.025f },
                        new DropTableItem { itemType = ItemTypes.Boots, itemDropChance = 0.025f }
                    }
                    };
                    MobDropPool.mobDropTable.Add(mercDropTable);
                    break;
                default:
                    Debug.Log("ISSUE::MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.");
                    Debug.Log("ISSUE::[repeat] MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.");
                    Debug.Log("ISSUE::[repeat] MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.");
                    break;
            }
        }
    }
}