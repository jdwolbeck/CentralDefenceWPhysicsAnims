using Banspad;
using System.Collections.Generic;
using UnityEngine;

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
    public SquadState SquadState;
    public EntityController SquadLeader;
    public List<EntityController> SquadEntities;
    public List<EntityStatusInfo> SquadEntityStatuses;
    public TacticalAIState CurrentTacticalState;
    public string SquadStateString;
    public float SquadRadius;

    private bool addedToSquadList;
    private bool debugSquadController;

    private void Start()
    {
        SquadState = new SquadState();
        SquadEntities = new List<EntityController>();
        SquadEntityStatuses = new List<EntityStatusInfo>();

        SquadRadius = 2f;
        CurrentTacticalState = TacticalAIState.DEFENDING_NEXUS;
        addedToSquadList = false;
        debugSquadController = false;

        PopulateEntityLists();
    }
    private void Update()
    {
        UpdateSquadData();

        SquadState = SquadState.NextSquadState(this);
        SquadStateString = SquadState.ToString();
        SquadState.HandleStateLogic(this);
    }
    public void AddEntityToSquad(EntityController entity)
    {
        if (entity.IsDead)
            return;

        foreach (EntityController e in SquadEntities)
        {
            if (e == entity)
                return;
        }

        SquadEntities.Add(entity);
        SquadEntityStatuses.Add(GetSquadEntityStateInfo(entity));
    }

    public void RemoveEntityFromSquad(EntityController entity)
    {
        // t is instatiated as a temporary EntityController (t =>)
        // we then set the boolean equation for finding the index, when their temp variable t (it iterates through the list EntityList and sets t to each of the indexes as it goes) we check if its this specific EntityController.
        int index = SquadEntities.FindIndex(t => t == entity);

        Logging.Log("Index of EntityList that the entity " + entity.gameObject.ToString() + " (" + entity.RandomID + "): " + index, debugSquadController);
        SquadEntities.Remove(entity);
        SquadEntityStatuses.RemoveAt(index);

        if (entity == SquadLeader)
            SetNewSquadLeader();
    }
    private void UpdateSquadData()
    {
        // Handle generic Squad/Entity updates
        PopulateEntityLists();

        if (SquadLeader == null && SquadEntities.Count > 0)
            SquadLeader = SquadEntities[0];

        if (!addedToSquadList && SquadLeader != null)
        {
            GameHandler.Instance.AddSquadToList(this, SquadLeader.EntityType);
            addedToSquadList = true;
        }

        // Update all entities data
        int index = 0;
        foreach (EntityController entity in SquadEntities)
        {
            if (SquadEntityStatuses.Count > index)
            {
                if (!entity.GetEntityStatusinfo(SquadEntityStatuses[index]))
                    Logging.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send EntityStatusInfo...", debugSquadController);
            }
            else
            {
                Logging.Log("ERROR: tried to update EntityStatusInfo on index " + index + " when we have a EntityStatusList.Count of " + SquadEntityStatuses.Count, true);
            }

            index++;
        }
    }
    private void PopulateEntityLists()
    {
        foreach (EntityController entity in gameObject.GetComponentsInChildren<EntityController>())
            AddEntityToSquad(entity);

        SetNewSquadLeader();
    }
    private EntityStatusInfo GetSquadEntityStateInfo(EntityController entity)
    {
        EntityStatusInfo stateInfo = new EntityStatusInfo();

        if (!entity.GetEntityStatusinfo(stateInfo))
        {
            Logging.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send SquadEntityStateInfo...", debugSquadController);

            stateInfo.Health = 1;
            stateInfo.Position = Vector3.zero;
        }

        return stateInfo;
    }
    private void SetNewSquadLeader()
    {
        if (SquadEntities.Count > 0 && SquadLeader == null)
            SquadLeader = SquadEntities[0];
    }
}