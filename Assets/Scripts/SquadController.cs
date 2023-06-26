using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class UnitStateInfo
{
    public Vector3 Position;
    public float   Health;
}
public class SquadController : MonoBehaviour
{
    public EntityController SquadLeader;
    public List<EntityController> UnitList;
    public List<UnitStateInfo> UnitStateList;
    public List<float> testList;

    private const float squadRadius = 5f;

    private void Start()
    {
        UnitList = new List<EntityController>();
        UnitStateList = new List<UnitStateInfo>();
        testList = new List<float>();
        InitializeUnitLists();
    }
    private void Update()
    {
        UpdateUnitStateInfo();
    }
    private void InitializeUnitLists()
    {
        bool unitAccountedFor = false;
        int index = 0;
        foreach (EntityController unit in gameObject.GetComponentsInChildren<EntityController>())
        {
            foreach (EntityController currentUnit in UnitList)
            {
                if (currentUnit == unit)
                {
                    unitAccountedFor = true;
                }
            }
            if (!unitAccountedFor)
            {
                UnitList.Add(unit);
                UnitStateList.Add(GetSquadUnitStateInfo(unit));
                testList.Add(UnitStateList[index].Health);
            }
            index++;
        }
    }
    private void UpdateUnitStateInfo()
    {
        int index = 0;
        foreach (EntityController unit in UnitList)
        {
            if (UnitStateList.Count > index)
            {
                if (!unit.GetUnitStateInfo(UnitStateList[index]))
                {
                    Debug.Log("SquadController: unit " + unit.gameObject.ToString() + " was unable to send SquadUnitStateInfo...");
                }
                else
                {
                    testList[index] = UnitStateList[index].Health;
                }
            }
            else
            {
                Debug.Log("ERROR: tried to update UnitStateInfo on index " + index + " when we have a UnitStateList.Count of " + UnitStateList.Count);
            }
            index++;
        }
    }
    private UnitStateInfo GetSquadUnitStateInfo(EntityController unit)
    {
        UnitStateInfo stateInfo = new UnitStateInfo();
        if (!unit.GetUnitStateInfo(stateInfo))
        {
            Debug.Log("SquadController: unit " + unit.gameObject.ToString() + " was unable to send SquadUnitStateInfo...");
            stateInfo.Position = Vector3.zero;
            stateInfo.Health = 1;
        }
        return stateInfo;
    }
    private void GroupSquadOnLeader()
    {
        foreach (EntityController unit in UnitList)
        {
            if (unit != SquadLeader)
            {
                if (Vector3.Distance(unit.transform.position, SquadLeader.transform.position) > squadRadius)
                {
                    //float randomizeDistance
                }
            }
        }
    }
}
