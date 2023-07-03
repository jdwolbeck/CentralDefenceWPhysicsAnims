using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadGroupingState : SquadState
{
    public override SquadState NextSquadState(SquadController squad)
    {
        foreach (EntityController entity in squad.SquadEntities)
        {
            if (entity.HasNavAgentDestination() || Vector3.Distance(entity.transform.position, squad.SquadLeader.transform.position) > squad.SquadRadius)
            {
                return this;
            }
        }
        return new SquadPatrollingState();
    }
    public override void HandleStateLogic(SquadController squad)
    {
        if (squad.SquadLeader != null)
        {
            foreach (EntityController entity in squad.SquadEntities)
            {
                if (entity != squad.SquadLeader)
                {
                    if (entity.currentTarget == null && Vector3.Distance(entity.transform.position, squad.SquadLeader.transform.position) > squad.SquadRadius)
                    {
                        GroupEntityOnSquad(entity, squad);
                    }
                }
            }
        }
    }
    private void GroupEntityOnSquad(EntityController entity, SquadController squad)
    {
        float randomizedDistance = Random.Range(0.5f, squad.SquadRadius);
        entity.SetNavAgentDestination(squad.SquadLeader.transform.position, randomizedDistance, 0.8f);
        //if (debugSquadController) Debug.Log("Grouping squad member: " + entity.ToString());
    }
}
