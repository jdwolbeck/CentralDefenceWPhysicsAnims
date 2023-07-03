using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadPatrollingState : SquadState
{
    private float patrolRadius = Random.Range(8f, 12f);

    public override SquadState NextSquadState(SquadController squad)
    {
        foreach (EntityController entity in squad.SquadEntities)
        {
            if (!entity.HasNavAgentDestination() && Vector3.Distance(entity.transform.position, squad.SquadLeader.transform.position) <= squad.SquadRadius)
            {
                return this;
            }
        }
        return new SquadGroupingState();
    }
    public override void HandleStateLogic(SquadController squad)
    {
        if (squad.SquadLeader != null && squad.SquadLeader.currentTarget == null && !squad.SquadLeader.HasNavAgentDestination() && GameHandler.instance.Hub != null)
        {
            // Determine positioning of our SquadLeader
            Vector3 locationRelativeToNexus = squad.SquadLeader.transform.position - GameHandler.instance.Hub.transform.position;
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
                new Vector3(randomizedDestination, 0, squadsNextQuadrant.y * Mathf.Sqrt((patrolRadius * patrolRadius) - (randomizedDestination * randomizedDestination)));

            foreach (EntityController entity in squad.SquadEntities)
            {
                if (entity == squad.SquadLeader)
                {
                    entity.SetNavAgentDestination(nextPatrolDestination, 1f, .5f);
                }
                else
                {
                    Vector3 entityPosOffset = entity.transform.position - squad.SquadLeader.transform.position;
                    entity.SetNavAgentDestination(nextPatrolDestination + entityPosOffset, 1f, .5f);
                }
            }
        }
    }
}