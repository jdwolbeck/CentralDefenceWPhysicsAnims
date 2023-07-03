using Banspad;
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
    public ItemGroupsEnum itemType;
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
                case EntityType.Mob:
                    MobDropTable mobDropTable = new MobDropTable
                    {
                        entityType = EntityType.Mob,
                        itemDropList = new List<DropTableItem>()
                        {
                            new DropTableItem { itemType = ItemGroupsEnum.Helmet, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Chest, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Legs, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Gloves, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Boots, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Belt, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Amulet, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.RingLeft, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.RingRight, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.OneHandWeapon, itemDropChance = 1f },
                            new DropTableItem { itemType = ItemGroupsEnum.Shield, itemDropChance = 1f }
                        }
                    };

                    MobDropPool.mobDropTable.Add(mobDropTable);
                    break;
                case EntityType.Mercenary:
                    break;
                case EntityType.Hero:
                    break;
                default:
                    Logging.Log("ISSUE::MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.", true);
                    Logging.Log("ISSUE::[repeat] MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.", true);
                    Logging.Log("ISSUE::[repeat] MobDropPool does not have an entry for EntityType of " + i + " please fill in date for this entry.", true);
                    break;
            }
        }
    }
}