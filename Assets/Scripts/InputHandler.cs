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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UiManager.Instance.ToggleInventory();
        }
    }
}
