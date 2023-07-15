using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadPatrollingState : SquadState
{
    private readonly float patrolRadius = Random.Range(8f, 12f);

    public override SquadState NextSquadState(SquadController squad)
    {
        foreach (EntityController entity in squad.SquadEntities)
        {
            if (entity.CurrentTarget != null)
                return new SquadEngagingEnemyState();
            else if (Vector3.Distance(entity.transform.position, squad.SquadLeader.transform.position) > squad.SquadRadius)
                return new SquadGroupingState();
        }

        return this;
    }
    public override void HandleStateLogic(SquadController squad)
    {
        if (squad.SquadLeader != null && squad.SquadLeader.CurrentTarget == null && GameHandler.Instance.Hub != null)
        {
            // Determine positioning of our SquadLeader
            Vector3 locationRelativeToNexus = squad.SquadLeader.transform.position - GameHandler.Instance.Hub.transform.position;
            Vector3 nextPatrolDestination;
            Vector2 squadQuadrantRelToNexus = new Vector2(locationRelativeToNexus.x > 0 ? 1 : -1, locationRelativeToNexus.z > 0 ? 1 : -1);
            Vector2 squadsNextQuadrant;
            float randomizedDestination = Random.Range(0, patrolRadius);

            // Find random position in the next quadrant of the nexus
            if (squadQuadrantRelToNexus == new Vector2(1, 1))
                squadsNextQuadrant = new Vector2(1, -1);
            else if (squadQuadrantRelToNexus == new Vector2(1, -1))
                squadsNextQuadrant = new Vector2(-1, -1);
            else if (squadQuadrantRelToNexus == new Vector2(-1, -1))
                squadsNextQuadrant = new Vector2(-1, 1);
            else
                squadsNextQuadrant = new Vector2(1, 1);

            // Set destination to this quadrant
            randomizedDestination *= squadsNextQuadrant.x;
            nextPatrolDestination = new Vector3(randomizedDestination, 0, squadsNextQuadrant.y * Mathf.Sqrt((patrolRadius * patrolRadius) - (randomizedDestination * randomizedDestination)));

            foreach (EntityController entity in squad.SquadEntities)
            {
                if (entity.HasNavAgentDestination(out bool isNavEnabled))
                    continue;

                if (isNavEnabled)
                {
                    if (entity == squad.SquadLeader)
                    {
                        entity.AddDestinationToQueue(new NavMoveCommand(nextPatrolDestination, 1f, 0.5f), false); //entity.SetNavAgentDestination(nextPatrolDestination, 1f, .5f);
                    }
                    else
                    {
                        Vector3 entityPosOffset = entity.transform.position - squad.SquadLeader.transform.position;
                        entity.AddDestinationToQueue(new NavMoveCommand(nextPatrolDestination + entityPosOffset, 1f, 0.5f), false); //entity.SetNavAgentDestination(nextPatrolDestination + entityPosOffset, 1f, .5f);
                    }
                }
            }
        }
    }
}