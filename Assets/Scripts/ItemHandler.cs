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
    private bool itemDebug = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    void Start()
    {
        groundItems = new List<GameObject>();
        testItems = new List<ItemBase>();
        GenerateTestItems();
    }
    public void JustinItemDropTest()
    {




        //ItemStorageManager.Instance.PickupItem(Hero.Items, testItem);
        //ItemStorageManager.Instance.DisplayEntityEquipment(Hero.Items);
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
                    Debug.Log("Dropped " + itemToDrop.ToString());
                    ItemStorageManager.Instance.PickupItem(Hero.Items, item);
                    ItemStorageManager.Instance.DisplayEntityEquipment(Hero.Items);
                    break;
                }
            }
        }
        /* OLD CODE
            // Figure out if the monster rolled for any kind of loot to drop
            // This will give you the generic type of item that is being dropped
            ItemTypes itemToDrop = RollForItemDrop(entityType);
            if (itemToDrop == ItemTypes.ItemTypeCount)
                return;

            // Now we can handle the logic for deciding what tier of item this is (White, rare, yellow, unique)

            // Here we handle the logic for determining which stats will be rolled onto this item

            // Get the items scriptable object and spawn it in
            GameObject itemPrefab = ResourceDictionary.instance.GetItemPreset(itemToDrop).itemPrefab;
            GameObject droppedItem = Instantiate(itemPrefab, entityObject.transform.position, Quaternion.identity);
            groundItems.Add(droppedItem); 
        */
    }
    public ItemGroupsEnum RollForItemDrop(EntityType entityType)
    {
        ItemGroupsEnum itemToDrop = ItemGroupsEnum.None;
        switch (entityType)
        {
            case EntityType.Mob:
                float itemRoll = UnityEngine.Random.Range(0f, 11f);
                float totalRollWeight = 11f;
                float cumulativeWeight = 0f;
                if (itemDebug) Debug.Log("Mob death is rolling for item, itemRoll = " + itemRoll + " (0 helmet .2 chest .4 legs .6 gloves .8 boots 1.0");
                int itemIndex = 0;
                while (totalRollWeight - cumulativeWeight > 0 && itemIndex < 11)
                {
                    if (itemIndex >= mobDropTable[0].itemDropList.Count)
                    {
                        break;
                    }
                    DropTableItem currentItem = mobDropTable[0].itemDropList[itemIndex];
                    if (itemRoll >= totalRollWeight - currentItem.itemDropChance)
                    {
                        if (itemDebug) Debug.Log("We found the item that we should drop: " + currentItem.itemType.ToString());
                        itemToDrop = currentItem.itemType; 
                        break;
                    }
                    else
                    {
                        if (itemDebug) Debug.Log("Current weight (" + (totalRollWeight - cumulativeWeight) + ") failed for item:" + currentItem.itemType.ToString() + " new cumulativeRoll: " + (cumulativeWeight + currentItem.itemDropChance));
                        cumulativeWeight += currentItem.itemDropChance;
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

//**********************************************************************
//****************    Enums Storage Types    ***************************
//**********************************************************************
public enum StorageTypesEnum
{
    EquipmentCharms,
    BankGems,
    BankRunes
}

//**********************************************************************
//**********************    Enums Items    *****************************
//**********************************************************************
public enum ItemGroupsEnum
{
    None = 0,
    //Equipment
    Helmet = 100,
    Chest = 200,
    Legs = 300,
    Gloves = 400,
    Boots = 500,
    Belt = 600,
    RingLeft = 700,
    RingRight = 800,
    Amulet = 900,
    OneHandWeapon = 1000,
    TwoHandWeapon = 1100,
    Shield = 1200,
    //Misc
    Charm = 10000,
    Gem = 10100,
    Rune = 10200
}
public enum ItemBaseTypesEnum
{
    //Armors
    HelmetLeather = 1,
    ArmorLeather = 1000,
    LegsLeather = 2000,
    BootsLeather = 3000,
    GlovesLeather = 4000,
    BeltLeather = 5000,
    AmuletType1 = 6000,
    RingType1 = 7000,
    //Weapon/Shields
    SwordWood = 8000,
    ShieldWood = 9000,
    //Misc
    CharmType1 = 10000,
    //Gems
    GemRough = 20000,
    GemChipped = 20010,
    GemFlawed = 20020,
    GemRegular = 20030,
    GemFlawless = 20040,
    GemSquare = 20050,
    GemFlawlessSquare = 20060,
    GemStar = 20070,
    GemFlawlessStar = 20080,
    GemOctagon = 20090,
    GemFlawlessOctagon = 20100,
    GemImperialOctagon = 20110,
    GemRoyal = 20120,
    GemMagnificent = 20130,
    GemPerfect = 20140,
    //Runes
    RuneAwp = 30000,
    RuneBix = 30001,
    RuneCom = 30002
}
public enum ItemTierEnum
{
    Weak = 0,
    Standard = 1,
    Elite = 2
}
public enum ItemQualityEnum
{
    Ethereal = 1,
    Basic = 10,
    Magic = 11,
    Rare = 12,
    Unique = 20,
    Crafted = 30,
    Runeword = 40
}
public enum ItemTextureIdEnum
{
    None = 0,
    //Armors
    HelmetLeather = 1,
    ArmorLeather = 1000,
    LegsLeather = 2000,
    BootsLeather = 3000,
    GlovesLeather = 4000,
    BeltLeather = 5000,
    AmuletType1 = 6000,
    RingType1 = 7000,
    //Weapon/Shields
    SwordWood = 8000,
    ShieldWood = 9000,
    //Misc
    CharmType1 = 10000,
    //Gems
    GemRough = 20000,
    GemChipped = 20010,
    GemFlawed = 20020,
    GemRegular = 20030,
    GemFlawless = 20040,
    GemSquare = 20050,
    GemFlawlessSquare = 20060,
    GemStar = 20070,
    GemFlawlessStar = 20080,
    GemOctagon = 20090,
    GemFlawlessOctagon = 20100,
    GemImperialOctagon = 20110,
    GemRoyal = 20120,
    GemMagnificent = 20130,
    GemPerfect = 20140,
    //Runes
    RuneAwp = 30000,
    RuneBix = 30001,
    RuneCom = 30002
}

//**********************************************************************
//****************    Enums Item Attributes   **************************
//**********************************************************************
public enum ItemAttributeTypesEnum
{
    //Attributes
    FlatStrength = 10,
    FlatDexterity = 11,
    //Defensive
    FlatLife = 100,
    FlatDefense = 101,
    //Damage
    FlatDamage = 200,
    //Utility
    AddSockets = 1000,
    //Unique Abilities
    FreezeTargetOnHit = 10000
}