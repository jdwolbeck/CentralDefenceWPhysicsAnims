using Banspad;
using Banspad.Itemization;
using Banspad.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static MobDropPool;

public class ItemHandler : MonoBehaviour
{
    public EntityController Hero;
    public static ItemHandler instance { get; private set; }
    [SerializeField] private List<GameObject> groundItems;
    [SerializeField] private List<ItemBase> testItems;
    private bool isUIInitialized = false;
    private bool itemDebug = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    private void Start()
    {
        groundItems = new List<GameObject>();
        testItems = new List<ItemBase>();
        GenerateTestItems();       
    }
    private void Update()
    {
        if (!isUIInitialized && Hero != null && Hero.Items != null)
        {
            UiManager.Instance.DisplayEntityEquipment(Hero.Items);
            isUIInitialized = true;
        }
    }
    public void HandleMonsterItemDrop(EntityType entityType, GameObject entityObject)
    {
        ItemGroupsEnum itemToDrop = RollForItemDrop(entityType);
        if (itemToDrop != ItemGroupsEnum.None)
        {
            foreach (ItemBase item in testItems)
            {
                if (item.ItemGroup == (int)itemToDrop)
                {
                    if (itemDebug) Debug.Log("Dropped " + itemToDrop.ToString());
                    ItemStorageManagerExtended.Instance.PickupItem(Hero.Items, item);
                    break;
                }
            }
        }
    }
    public ItemGroupsEnum RollForItemDrop(EntityType entityType)
    {
        ItemGroupsEnum itemToDrop = ItemGroupsEnum.None;
        switch (entityType)
        {
            case EntityType.Mob:
                float itemRoll = UnityEngine.Random.Range(0f, 11f);
                float currentRollWeight = 11f;
                if (itemDebug) Debug.Log("Mob death is rolling for item, itemRoll = " + itemRoll);
                int itemIndex = 0;
                while (currentRollWeight > 0 && itemIndex < 11)
                {
                    if (itemIndex >= mobDropTable[0].itemDropList.Count)
                    {
                        break;
                    }
                    DropTableItem currentItem = mobDropTable[0].itemDropList[itemIndex];
                    if (itemRoll >= currentRollWeight - currentItem.itemDropChance)
                    {
                        if (itemDebug) Debug.Log("We found the item that we should drop: " + currentItem.itemType.ToString());
                        itemToDrop = currentItem.itemType; 
                        break;
                    }
                    else
                    {
                        if (itemDebug) Debug.Log("Current weight (" + currentRollWeight + ") failed for item:" + currentItem.itemType.ToString() + " new weight: " + (currentRollWeight - currentItem.itemDropChance));
                        currentRollWeight -= currentItem.itemDropChance;
                    }
                    itemIndex++;
                }
                break;
            case EntityType.Hero:
            case EntityType.Mercenary:
            default:
                break;
        }
        return itemToDrop;
    }
    public void GenerateTestItems()
    {
        // Define a couple test items we can give to our Hero
        // Basic Helmet
        List<ItemAttribute> attribute = new List<ItemAttribute>();
        ItemAttribute tmp = new ItemAttribute();
        tmp.AttributeAmount = 5;
        tmp.AttributeType = (int)ItemAttributeTypesEnum.FlatLife;
        tmp.SetSortValue();
        attribute.Add(tmp);
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Helmet, ItemBaseTypesEnum.HelmetLeather, ItemTierEnum.Standard, 
                                        ItemQualityEnum.Basic, ItemTextureIdEnum.HelmetLeather, attribute, false));
        //Magic Chest
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Chest, ItemBaseTypesEnum.ArmorLeather, ItemTierEnum.Standard,
                                        ItemQualityEnum.Magic, ItemTextureIdEnum.ArmorLeather, null, true));
        // Rare Legs
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Legs, ItemBaseTypesEnum.LegsLeather, ItemTierEnum.Standard,
                                        ItemQualityEnum.Rare, ItemTextureIdEnum.LegsLeather, null, true));
        // Unique Boots
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Boots, ItemBaseTypesEnum.BootsLeather, ItemTierEnum.Standard,
                                        ItemQualityEnum.Unique, ItemTextureIdEnum.BootsLeather, null, true));
        // Basic Gloves
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Gloves, ItemBaseTypesEnum.GlovesLeather, ItemTierEnum.Standard,
                                        ItemQualityEnum.Basic, ItemTextureIdEnum.GlovesLeather, null, true));
        // Rare Belt
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Belt, ItemBaseTypesEnum.BeltLeather, ItemTierEnum.Standard,
                                        ItemQualityEnum.Rare, ItemTextureIdEnum.BeltLeather, null, true));
        // Unique Sword
        testItems.Add(GenerateTestItem(ItemGroupsEnum.OneHandWeapon, ItemBaseTypesEnum.SwordWood, ItemTierEnum.Standard,
                                        ItemQualityEnum.Unique, ItemTextureIdEnum.SwordWood, null, true));
        // Basic Shield
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Shield, ItemBaseTypesEnum.ShieldWood, ItemTierEnum.Standard,
                                        ItemQualityEnum.Basic, ItemTextureIdEnum.ShieldWood, null, true));
        // Rare Amulet
        testItems.Add(GenerateTestItem(ItemGroupsEnum.Amulet, ItemBaseTypesEnum.AmuletType1, ItemTierEnum.Standard,
                                        ItemQualityEnum.Rare, ItemTextureIdEnum.AmuletType1, null, true));
        // Magic Ring L
        testItems.Add(GenerateTestItem(ItemGroupsEnum.RingLeft, ItemBaseTypesEnum.RingType1, ItemTierEnum.Standard,
                                        ItemQualityEnum.Magic, ItemTextureIdEnum.RingType1, null, true));
        // Unique Right R
        testItems.Add(GenerateTestItem(ItemGroupsEnum.RingRight, ItemBaseTypesEnum.RingType1, ItemTierEnum.Standard,
                                        ItemQualityEnum.Unique, ItemTextureIdEnum.RingType1, null, true));


    }
    public ItemBase GenerateTestItem(ItemGroupsEnum itemGroup, ItemBaseTypesEnum itemBaseType, ItemTierEnum itemTier, ItemQualityEnum itemQuality, 
                                     ItemTextureIdEnum itemTextureId, List<ItemAttribute> itemAttributes, bool rndAttributes)
    {
        ItemBase testItem = new ItemBase();

        testItem.ItemGroup = (int)itemGroup;
        testItem.ItemBaseType = (int)itemBaseType;
        testItem.ItemTier = (int)itemTier;
        testItem.ItemQuality = (int)itemQuality;
        testItem.ItemTextureId = (int)itemTextureId;

        if (!rndAttributes && itemAttributes != null)
        {
            foreach(ItemAttribute itemAttribute in itemAttributes)
            {
                testItem.ItemAttributes.Add(itemAttribute);
            }
        }
        else
        { // Create some random attributes
            int attributeCount = 1;
            switch (itemQuality)
            {
                case (ItemQualityEnum.Basic):
                    attributeCount = 2;
                    break;
                case (ItemQualityEnum.Magic):
                    attributeCount = 3;
                    break;
                case (ItemQualityEnum.Rare):
                    attributeCount = 4;
                    break;
                case (ItemQualityEnum.Unique):
                    attributeCount = 5;
                    break;
                default:
                    break;
            }

            ItemAttribute attribute = new ItemAttribute();
            for (int index = 0; index < attributeCount; index++)
            {
                attribute.AttributeAmount = UnityEngine.Random.Range(1, 11);

                int randAttribute = UnityEngine.Random.Range(0, 7);
                if (randAttribute == 0) randAttribute = (int)ItemAttributeTypesEnum.FlatStrength;
                else if (randAttribute == 1) randAttribute = (int)ItemAttributeTypesEnum.FlatDexterity;
                else if (randAttribute == 2) randAttribute = (int)ItemAttributeTypesEnum.FlatLife;
                else if (randAttribute == 3) randAttribute = (int)ItemAttributeTypesEnum.FlatDefense;
                else if (randAttribute == 4) randAttribute = (int)ItemAttributeTypesEnum.FlatDamage;
                else if (randAttribute == 5) randAttribute = (int)ItemAttributeTypesEnum.AddSockets;
                else randAttribute = (int)ItemAttributeTypesEnum.FreezeTargetOnHit;
                attribute.AttributeType = randAttribute;

                attribute.SetSortValue();
                testItem.ItemAttributes.Add(attribute);
            }
        }

        ScriptableEquipmentSlotDefaults slotDefaults = ResourceSystemExtended.Instance.GetEquipmentSlotDefaults((ItemGroupsEnum)testItem.ItemGroup);
        testItem.SlotWidth = slotDefaults.SlotWidth;
        testItem.SlotHeight = slotDefaults.SlotHeight;

        return testItem;
    }
}
