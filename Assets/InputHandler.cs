using Banspad.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    private void Start()
    {
        Debug.Log("Test my booy");
        //ItemStorageManager.Instance.CharacterInventoryUiGO.HideInventory();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            //ItemStorageManager.Instance.CharacterInventoryUiGO.ToggleInventory();
        }
    }
}
