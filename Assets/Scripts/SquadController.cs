using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class EntityStateInfo
{
    public Vector3 Position;
    public float   Health;
}
public enum TacticalAIState
{
    DEFENDING_NEXUS = 0,
    DEFENDING_ANOTHER_HERO,
    ATTACKING_STRONGHOLD
}
public class SquadController : MonoBehaviour
{
    public EntityController SquadLeader;
    public List<EntityController> EntityList;
    public List<EntityStateInfo> EntityStateList;
    public TacticalAIState curTacticalState;

    private const float squadRadius = 2f;
    private const float groupingCooldown = 0.5f;
    private float lastUpdateTime;

    private float patrolRadius;
    private float squadStateCooldown = 1f;
    private float lastSquadStateUpdateTime;
    private bool addedToSquadList;
    private bool debugSquadController = false;

    private void Start()
    {
        curTacticalState = TacticalAIState.DEFENDING_NEXUS;
        EntityStateList = new List<EntityStateInfo>();
        EntityList = new List<EntityController>();
        PopulateEntityLists();
        patrolRadius = Random.Range(4f, 11f);

    }
    private void Update()
    {
        UpdateSquadState();
        UpdateEntityStateInfo();
        UpdateSquadGrouping();
        UpdateTacticalAI();
    }
    public void AddEntityToSquad(EntityController entity)
    {
        if (entity.isDead)
            return;
        foreach (EntityController e in EntityList)
        {
            if (e == entity)
            {
                //Debug.Log("Squad " + ToString() + " already contains a reference to entity " + entity.ToString());
                return;
            }
        }
        EntityList.Add(entity);
        EntityStateList.Add(GetSquadEntityStateInfo(entity));
    }

    public void RemoveEntityFromSquad(EntityController entity)
    {
        // t is instatiated as a temporary EntityController (t =>)
        // we then set the boolean equation for finding the index, when their temp variable t (it iterates through the list EntityList and sets t to each of the indexes as it goes) we check if its this specific EntityController.
        int index = EntityList.FindIndex(t => t == entity);
        if (debugSquadController) Debug.Log("Index of EntityList that the entity " + entity.gameObject.ToString() + " (" + entity.randomID + "): " + index);
        EntityList.Remove(entity);
        EntityStateList.RemoveAt(index);
        if (entity == SquadLeader)
        {
            SetNewSquadLeader();
        }
    }
    private void PopulateEntityLists()
    {
        foreach (EntityController entity in gameObject.GetComponentsInChildren<EntityController>())
        {
            AddEntityToSquad(entity);
        }
    }
    private void UpdateSquadState()
    {
        if (Time.time >= lastSquadStateUpdateTime + squadStateCooldown)
        {
            if (SquadLeader == null && EntityList.Count > 0)
                SquadLeader = EntityList[0];
            if (!addedToSquadList && SquadLeader != null)
            {
                GameHandler.instance.AddSquadToList(this, SquadLeader.entityType);
                addedToSquadList = true;
            }
            PopulateEntityLists();
            lastSquadStateUpdateTime = Time.time;
        }
    }
    private void UpdateEntityStateInfo()
    {
        int index = 0;
        foreach (EntityController entity in EntityList)
        {
            if (EntityStateList.Count > index)
            {
                if (!entity.GetEntityStateInfo(EntityStateList[index]))
                {
                    if (debugSquadController) Debug.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send SquadEntityStateInfo...");
                }
            }
            else
            {
                Debug.Log("ERROR: tried to update EntityStateInfo on index " + index + " when we have a EntityStateList.Count of " + EntityStateList.Count);
            }
            index++;
        }
    }
    private EntityStateInfo GetSquadEntityStateInfo(EntityController entity)
    {
        EntityStateInfo stateInfo = new EntityStateInfo();
        if (!entity.GetEntityStateInfo(stateInfo))
        {
            if (debugSquadController) Debug.Log("SquadController: entity " + entity.gameObject.ToString() + " was unable to send SquadEntityStateInfo...");
            stateInfo.Position = Vector3.zero;
            stateInfo.Health = 1;
        }
        return stateInfo;
    }
    private void UpdateSquadGrouping()
    {
        if (Time.time > lastUpdateTime + groupingCooldown && SquadLeader != null)
        {
            foreach (EntityController entity in EntityList)
            {
                if (entity != SquadLeader)
                {
                    if (entity.currentTarget == null && Vector3.Distance(entity.transform.position, SquadLeader.transform.position) > squadRadius)
                    {
                        GroupEntityOnSquad(entity);
                    }
                }
            }
            lastUpdateTime = Time.time;
        }
    }
    private void GroupEntityOnSquad(EntityController entity)
    {
        float randomizedDistance = Random.Range(0.5f, squadRadius);
        entity.SetNavAgentDestination(SquadLeader.transform.position, randomizedDistance, 0.8f);
        //if (debugSquadController) Debug.Log("Grouping squad member: " + entity.ToString());
    }
    private void UpdateTacticalAI()
    {
        switch (curTacticalState)
        {
            case TacticalAIState.DEFENDING_NEXUS:
                PatrolNexus();
                break;
            case TacticalAIState.DEFENDING_ANOTHER_HERO:
                break;
            case TacticalAIState.ATTACKING_STRONGHOLD:
                break;
        }
    }
    private void PatrolNexus()
    {
        if (SquadLeader != null && SquadLeader.currentTarget == null && !SquadLeader.HasNavAgentDestination() && GameHandler.instance.Hub != null)
        {
            // Determine positioning of our SquadLeader
            Vector3 locationRelativeToNexus = SquadLeader.transform.position - GameHandler.instance.Hub.transform.position;
            Vector2 squadQuadrantRelToNexus = new Vector2(locationRelativeToNexus.x > 0 ? 1 : -1, locationRelativeToNexus.z > 0 ? 1 : -1);
            // Find random position in the next quadrant of the nexus
            Vector2 squadsNextQuadrant;
            if (squadQuadrantRelToNexus == new Vector2(1, 1))
                squadsNextQuadrant = new Vector2(1, -1);
            else if (squadQuadrantRelToNexus == new Vector2(1, -1))
                squadsNextQuadrant = new Vector2(-1, -1);
            else if (squadQuadrantRelToNexus == new Vector2(-1, -1))
                squadsNextQuadrant = new Vector2(-1, 1);
            else
                squadsNextQuadrant = new Vector2(1, 1);

            // Set destination to this quadrant
            float randomizedDestination = Random.Range(0, patrolRadius);
            randomizedDestination *= squadsNextQuadrant.x;
            Vector3 nextPatrolDestination = 
                new Vector3(randomizedDestination, 0, squadsNextQuadrant.y * Mathf.Sqrt((patrolRadius*patrolRadius) - (randomizedDestination* randomizedDestination)));

            SquadLeader.SetNavAgentDestination(nextPatrolDestination, 1f, .5f);
        }
    }
    private void SetNewSquadLeader()
    {
        if (EntityList.Count > 0)
        {
            SquadLeader = EntityList[0];
        }
        else
        {
            if (debugSquadController) Debug.Log("All entities within Squad " + ToString() + " are dead.");
        }
    }
}
