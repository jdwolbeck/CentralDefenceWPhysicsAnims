using Banspad;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }
    public List<SquadController> PlayerSquads;
    public List<SquadController> MobSquads;
    public GameObject Hub;

    private GameObject spearPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        PlayerSquads = new List<SquadController>();
        MobSquads = new List<SquadController>();
        spearPrefab = Resources.Load("Prefabs/Spear") as GameObject;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 destinationPoint = Vector3.zero;

            if(Physics.Raycast(ray, out RaycastHit hit))
                destinationPoint = hit.point;

            if (Input.GetKey(KeyCode.LeftShift) && destinationPoint != Vector3.zero)
            {
                GameObject spear;
                Rigidbody[] spearRbs;
                Vector3 spearSpawnPosition = Camera.main.transform.position + (-2 * Camera.main.transform.forward);
                Quaternion spearRotation = Quaternion.FromToRotation(Vector3.forward, destinationPoint - spearSpawnPosition);

                spear = Instantiate(spearPrefab, spearSpawnPosition, spearRotation);

                spear.GetComponent<SpearController>().Damage = 1f;
                spearRbs = spear.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rb in spearRbs)
                    rb.AddForce(30 * spear.transform.forward, ForceMode.VelocityChange);
            }
        }
    }
    public void AddSquadToList(SquadController activeSquad, EntityType squadType)
    {
        bool squadAccountedFor = false;

        if (activeSquad == null || squadType >= EntityType.EntityCount)
        {
            Logging.Log("AddSquadToList: an invalid squad was passed in.", true);

            return;
        }

        switch (squadType)
        {
            case EntityType.Hero:
            case EntityType.Mercenary:
                foreach (SquadController pSquad in PlayerSquads)
                {
                    if (pSquad == activeSquad)
                    {
                        squadAccountedFor = true;
                        break;
                    }
                }

                if (!squadAccountedFor)
                    PlayerSquads.Add(activeSquad);

                break;
            case EntityType.Mob:
                foreach (SquadController mSquad in MobSquads)
                {
                    if (mSquad == activeSquad)
                    {
                        squadAccountedFor = true;
                        break;
                    }
                }

                if (!squadAccountedFor)
                    MobSquads.Add(activeSquad);

                break;
        }
    }
}
