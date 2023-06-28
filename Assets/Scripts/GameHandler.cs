using Banspad;
using Banspad.Managers;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance { get; private set; }
    public List<SquadController> playerSquads;
    public List<SquadController> mobSquads;
    public GameObject Hub;
    private GameObject spearPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        spearPrefab = Resources.Load("Prefabs/Spear") as GameObject;
        playerSquads = new List<SquadController>();
        mobSquads = new List<SquadController>();
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
    public void AddSquadToList(SquadController activeSquad, EntityType squadType)
    {
        if (activeSquad == null || squadType >= EntityType.EntityCount)
        {
            Debug.Log("AddSquadToList: an invalid squad was passed in.");
            return;
        }
        bool squadAccountedFor = false;
        switch (squadType)
        {
            case EntityType.Hero:
            case EntityType.Mercenary:
                foreach (SquadController pSquad in playerSquads)
                {
                    if (pSquad == activeSquad)
                    {
                        squadAccountedFor = true;
                        break;
                    }
                }
                if (!squadAccountedFor)
                    playerSquads.Add(activeSquad);
                break;
            case EntityType.Mob:
                foreach (SquadController mSquad in mobSquads)
                {
                    if (mSquad == activeSquad)
                    {
                        squadAccountedFor = true;
                        break;
                    }
                }
                if (!squadAccountedFor)
                    mobSquads.Add(activeSquad);
                break;
        }
    }
}
