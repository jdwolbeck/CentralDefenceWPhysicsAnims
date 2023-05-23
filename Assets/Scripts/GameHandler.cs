using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject basicUnit;
    public GameObject FallingBall;
    private GameObject spearPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (basicUnit.TryGetComponent(out Animator unitsAnimator))
        {
            //unitsAnimator.enabled = false;
        }
        spearPrefab = Resources.Load("Prefabs/Spear") as GameObject;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FallingBall.transform.position = new Vector3(-0.8941255f, 7f, -1.810738f);
            FallingBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
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
