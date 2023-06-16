using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static MonsterDropPool;

public class ItemHandler : MonoBehaviour
{
    public static ItemHandler instance { get; private set; }
    [SerializeField] private List<GameObject> groundItems;
    private bool itemDebug = false;
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        groundItems = new List<GameObject>();
    }
    public void HandleMonsterItemDrop(UnitType unitType, GameObject unitObject)
    {
        // Figure out if the monster rolled for any kind of loot to drop
        // This will give you the generic type of item that is being dropped
        ItemTypes itemToDrop = RollForItemDrop(unitType);
        if (itemToDrop == ItemTypes.ItemTypeCount)
            return;

        // Now we can handle the logic for deciding what tier of item this is (White, rare, yellow, unique)

        // Here we handle the logic for determining which stats will be rolled onto this item

        // Get the items scriptable object and spawn it in
        GameObject itemPrefab = ResourceDictionary.instance.GetItemPreset(itemToDrop).itemPrefab;
        GameObject droppedItem = Instantiate(itemPrefab, unitObject.transform.position, Quaternion.identity);
        groundItems.Add(droppedItem);
    }
    public ItemTypes RollForItemDrop(UnitType unitType)
    {
        ItemTypes itemToDrop = ItemTypes.ItemTypeCount;
        switch (unitType)
        {
            case UnitType.Attacker:
                float itemRoll = Random.Range(0, 1f);
                float cumulativeItemRollChance = 0f;
                if (itemDebug) Debug.Log("Attacker death is rolling for item, itemRoll = " + itemRoll + " (0 helmet .2 chest .4 legs .6 gloves .8 boots 1.0");
                int itemIndex = 0;
                while (cumulativeItemRollChance <= itemRoll && itemIndex < (int)ItemTypes.ItemTypeCount)
                {
                    if (itemIndex >= monsterDropTable[(int)unitType].itemDropList.Count)
                    {
                        break;
                    }

                    DropTableItem currentItem = monsterDropTable[(int)unitType].itemDropList[itemIndex];                
                    if (cumulativeItemRollChance + currentItem.itemDropChance >= itemRoll)
                    {
                        if (itemDebug) Debug.Log("We found the item that we should drop: " + currentItem.itemType.ToString());
                        itemToDrop = currentItem.itemType;
                        break;
                    }
                    else
                    {
                        if (itemDebug) Debug.Log("Current cumulativeRoll (" + cumulativeItemRollChance + ") failed for item:" + currentItem.itemType.ToString() + " new cumulativeRoll: " + (cumulativeItemRollChance + currentItem.itemDropChance));
                        cumulativeItemRollChance += currentItem.itemDropChance;
                    }

                    itemIndex++;
                }
                break;
            case UnitType.Defender:
                break;
            default:
                break;
        }
        return itemToDrop;
    }
}
