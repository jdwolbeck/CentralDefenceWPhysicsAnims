using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItemQualityColor")]
public class ScriptableItemQualityColor : ScriptableObject
{
    public ItemQualityEnum ItemQuality;
    public Color BackgroundColor;
}