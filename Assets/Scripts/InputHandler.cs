using Banspad.Managers;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            UiManager.Instance.ToggleInventory();
    }
}
