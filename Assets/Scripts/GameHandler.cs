using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance { get; private set; }
    public GameObject Hub;
    private GameObject spearPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        spearPrefab = Resources.Load("Prefabs/Spear") as GameObject;
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

            if (destinationPoint != Vector3.zero)
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
