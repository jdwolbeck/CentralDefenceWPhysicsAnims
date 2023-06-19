using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem")]
public class ItemScriptable : ScriptableObject
{
    public GameObject itemPrefab;
    public ItemTypes itemType;
}
