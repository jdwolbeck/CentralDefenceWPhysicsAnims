using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public enum TacticalAIState
{
    DEFENDING_NEXUS = 0,
    DEFENDING_ANOTHER_HERO,
    ATTACKING_STRONGHOLD
}
public class EntityStatusInfo
{
    public Vector3 Position;
    public float   Health;
}
public class SquadController : MonoBehaviour
{
    public EntityController SquadLeader;
    public List<EntityController> SquadEntities;
    public List<EntityStatusInfo> SquadEntityStatuses;
    public TacticalAIState CurrentTacticalState;
    public SquadState SquadState;
    public string SquadStateString = "";
    public float SquadRadius = 2f;

    private bool addedToSquadList;
    private bool debugSquadController = false;

    private void Start()
    {
        CurrentTacticalState = TacticalAIState.DEFENDING_NEXUS;
        SquadEntityStatuses = new List<EntityStatusInfo>();
        SquadEntities = new List<EntityController>();
        SquadState = new SquadState();
        PopulateEntityLists();
    }
    private void Update()
    {
        UpdateSquadData();
        SquadState = SquadState.NextSquadState(this);
        SquadState.HandleStateLogic(this);
        SquadStateString = SquadState.ToString();
    }
    public void AddEntityToSquad(EntityController entity)
    {
        if (entity.isDead)
            return;
        foreach (EntityController e in SquadEntities)
        {
            if (e == entity)
            {
                //Debug.Log("Squad " + ToString() + " already contains a reference to entity " + entity.ToString());
                return;
            }
        }
        SquadEntities.Add(entity);
        SquadEntityStatuses.Add(GetSquadEntityStateInfo(entity));
    }

    public void RemoveEntityFromSquad(EntityController entity)
    {
        // t is instatiated as a temporary EntityController (t =>)
        // we then set the boolean equation for finding the index, when their temp variable t (it iterates through the list EntityList and sets t to each of the indexes as it goes) we check if its this specific EntityController.
        int index = SquadEntities.FindIndex(t => t == entity);
        if (debugSquadController) Debug.Log("Index of EntityList that the entity " + entity.gameObject.ToString() + " (" + entity.randomID + "): " + index);
        SquadEntities.Remove(entity);
        SquadEntityStatuses.RemoveAt(index);
        if (entity == SquadLeader)
        {
            SetNewSquadLeader();
        }
    }
    private void UpdateSquadData()
    {
        // Handle generic Squad/Entity updates
        PopulateEntityLists();
        if (SquadLeader == null && SquadEntities.Count > 0)
            SquadLeader = SquadEntities[0];
        if (!addedToSquadList && SquadLeader != null)
        {
            GameHandler.instance.AddSquadToList(this, SquadLeader.entityType);
            addedToSquadList = true;
        }

        // Update all entities data
        int index = 0;
        foreach (EntityController entity in SquadEntities)
        {
            if (SquadEntityStatuses.Count > index)
            {
                if (!entity.GetEntityStatusinfo(SquadEntityStatuses[index]))
                {
                    if (debugSquadController) Debug.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send EntityStatusInfo...");
                }
            }
            else
            {
                Debug.Log("ERROR: tried to update EntityStatusInfo on index " + index + " when we have a EntityStatusList.Count of " + SquadEntityStatuses.Count);
            }
            index++;
        }
    }
    private void PopulateEntityLists()
    {
        foreach (EntityController entity in gameObject.GetComponentsInChildren<EntityController>())
        {
            AddEntityToSquad(entity);
        }
        SetNewSquadLeader();
    }
    private EntityStatusInfo GetSquadEntityStateInfo(EntityController entity)
    {
        EntityStatusInfo stateInfo = new EntityStatusInfo();
        if (!entity.GetEntityStatusinfo(stateInfo))
        {
            if (debugSquadController) Debug.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send SquadEntityStateInfo...");
            stateInfo.Position = Vector3.zero;
            stateInfo.Health = 1;
        }
        return stateInfo;
    }
    private void SetNewSquadLeader()
    {
        if (SquadEntities.Count > 0 && SquadLeader == null)
        {
            SquadLeader = SquadEntities[0];
        }
        else
        {
            if (debugSquadController) Debug.Log("All entities within Squad " + ToString() + " are dead or SquadLeader already declared.");
        }
    }
}