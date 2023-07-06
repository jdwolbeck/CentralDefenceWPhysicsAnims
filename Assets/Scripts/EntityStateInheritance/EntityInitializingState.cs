using Banspad.Storage;
using Banspad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class EntityInitializingState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        return new EntityIdleState();
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        // Configure Equipment inventory setup
        entityController.Items = new EntityItemsExtended();

        //Equipment
        entityController.Items.EquipmentContainer = new ItemContainerEquipment(false);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.OneHandWeapon);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Shield);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Helmet);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Chest);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Legs);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Gloves);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Boots);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Belt);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.RingLeft);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.RingRight);
        entityController.Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Amulet);

        //Other Storage
        entityController.Items.AddNewGenericStorage((int)StorageTypesEnum.EquipmentCharms, 10, 1, new List<int>() { (int)ItemGroupsEnum.Charm });
    }
}
