using Banspad.Entities;
using Banspad.Itemization;
using UnityEngine;

public class EntityItemsExtended : EntityItems
{
    public override bool CanWeildItem(ItemBase itemToCheck)
    {
        if (!EquipmentContainer.IsEquipmentSlotDefined(itemToCheck.ItemGroup))
        {
            Debug.Log("Equipment slot " + itemToCheck.ItemGroup + " is not defined. Passed in item: " + itemToCheck.ToString());
            return false;
        }

        if (EquipmentContainer.IsEquipmentSlotOccupied(itemToCheck.ItemGroup))
        {
            Debug.Log("Equipment slot " + itemToCheck.ItemGroup + " is occupied for item: " + itemToCheck.ToString());
            return false;
        }

        Debug.Log("Equipment slot " + itemToCheck.ItemGroup + " equipped item: " + itemToCheck.ToString());
        return true;
    }
}