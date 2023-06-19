using Banspad.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystemExtended : ResourceSystem
{
    public static new ResourceSystemExtended Instance;
    public List<ScriptableEquipmentSlotDefaults> EquipmentDefaultsPresets { get; private set; }
    public List<ScriptableItemQualityColor> ItemQualityColorPresets { get; private set; }
    public List<ScriptableItemTexture> ItemTexturePresets { get; private set; }
    private Dictionary<ItemGroupsEnum, ScriptableEquipmentSlotDefaults> _EquipmentSlotDefaults;
    private Dictionary<ItemTextureIdEnum, ScriptableItemTexture> _ItemTexturesById;

    //Unity functions
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        AssembleResources();
    }
    void Start()
    {

    }
    void Update()
    {

    }

    //Private functions
    private void AssembleResources()
    {
        EquipmentDefaultsPresets = Resources.LoadAll<ScriptableEquipmentSlotDefaults>("Presets/EquipmentDefaults").ToList();
        ItemQualityColorPresets = Resources.LoadAll<ScriptableItemQualityColor>("Presets/ItemQuality").ToList();
        ItemTexturePresets = Resources.LoadAll<ScriptableItemTexture>("Presets/ItemTextures").ToList();

        _EquipmentSlotDefaults = EquipmentDefaultsPresets.ToDictionary(r => r.ItemGroup, r => r);
        _ItemTexturesById = ItemTexturePresets.ToDictionary(r => r.ItemTextureId, r => r);

        foreach (var i in ItemQualityColorPresets)
            DefineEquipmentSlotColorForItemQuality((int)i.ItemQuality, i.BackgroundColor);
    }

    //Getters
    public ScriptableItemTexture GetItemTextureScriptable(ItemTextureIdEnum t) => _ItemTexturesById[t];
    public Sprite GetItemTexture(ItemTextureIdEnum t) => _ItemTexturesById[t].ItemImage;
    public ScriptableEquipmentSlotDefaults GetEquipmentSlotDefaults(ItemGroupsEnum t) => _EquipmentSlotDefaults[t];
    //Getter overrides
    public override Sprite GetTextureForId(int itemTextureIdEnum) => GetItemTexture((ItemTextureIdEnum)itemTextureIdEnum);
    public override Sprite GetEquipmentSlotDefaultImage(int t) => _EquipmentSlotDefaults[(ItemGroupsEnum)t].DefaultImage;
}