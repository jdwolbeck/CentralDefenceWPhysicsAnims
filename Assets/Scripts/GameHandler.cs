using Banspad.Managers;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance { get; private set; }
    public GameObject Hub;
    private GameObject spearPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

    }
    void Start()
    {
        spearPrefab = Resources.Load("Prefabs/Spear") as GameObject;

        //Define all account wide storgae objects
        ItemStorageManager.Instance.DefineBank(13, 6);
        ItemStorageManager.Instance.AddNewBankTab();
        ItemStorageManager.Instance.AddNewGenericStorage((int)StorageTypesEnum.BankGems, 7, 15, new List<int>() { (int)ItemGroupsEnum.Gem });
        ItemStorageManager.Instance.AddNewGenericStorage((int)StorageTypesEnum.BankRunes, 7, 15, new List<int>() { (int)ItemGroupsEnum.Rune });
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 destinationPoint = Vector3.zero;
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                destinationPoint = hit.point;
            }

            if (Input.GetKey(KeyCode.LeftShift) && destinationPoint != Vector3.zero)
            {
                Vector3 spearSpawnPosition = Camera.main.transform.position + (-2 * Camera.main.transform.forward);
                Quaternion spearRotation = Quaternion.FromToRotation(Vector3.forward, destinationPoint - spearSpawnPosition);
                GameObject spear = Instantiate(spearPrefab, spearSpawnPosition, spearRotation);
                spear.GetComponent<SpearController>().damage = 1f;
                //spear.GetComponentInChildren<Rigidbody>().AddForce(30 * spear.transform.forward, ForceMode.VelocityChange);
                Rigidbody[] spearRbs = spear.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in spearRbs)
                {
                    rb.AddForce(30 * spear.transform.forward, ForceMode.VelocityChange);
                }
            }
        }
    }
}
