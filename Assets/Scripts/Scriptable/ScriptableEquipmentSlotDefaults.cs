using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableEquipmentSlotDefaults")]
public class ScriptableEquipmentSlotDefaults : ScriptableObject
{
    public ItemGroupsEnum ItemGroup;
    public Sprite DefaultImage;
    public int SlotWidth;
    public int SlotHeight;
}