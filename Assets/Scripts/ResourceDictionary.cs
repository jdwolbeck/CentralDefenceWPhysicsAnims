using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceDictionary : MonoBehaviour
{
    public static ResourceDictionary instance;
    public List<ItemScriptable> ItemPresets { get; private set; }
    public List<Material> Materials { get; private set; }
    private Dictionary<ItemTypes, ItemScriptable> ItemPresetsDict;
    private Dictionary<string, Material> MaterialsDict;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        ItemPresets = new List<ItemScriptable>();
        ItemPresets.AddRange(Resources.LoadAll<ItemScriptable>("Presets/Items").ToList());
        ItemPresetsDict = new Dictionary<ItemTypes, ItemScriptable>();
        foreach (ItemScriptable so in ItemPresets)
        {
            ItemPresetsDict.Add(so.itemType, so);
        }

        Materials = Resources.LoadAll<Material>("Materials").ToList();
        MaterialsDict = new Dictionary<string, Material>();
        foreach (Material mat in Materials)
        {
            MaterialsDict.Add(mat.name, mat);
        }
    }
    public ItemScriptable GetItemPreset(ItemTypes presetItemType)
    {
        return ItemPresetsDict[presetItemType];
    }
    public Material GetMaterial(string matName)
    {
        return MaterialsDict[matName];
    }
}