using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EntityState
{
    private float timeToUpdateEntityState;
    public virtual EntityState NextEntityState(EntityController entityController)
    {
        return new EntityInitializingState();
    }
    public virtual void HandleStateLogic(EntityController entityController)
    {
        if (Time.time > timeToUpdateEntityState)
        {
            // Do some checks to set our CurrentTarget if necessary.
            HandleEnemyProximityChecks(entityController);
            HandleDefendingSquad(entityController);

            timeToUpdateEntityState = Time.time + 0.5f;
        }
    }
    private void HandleEnemyProximityChecks(EntityController entityController)
    {
        EntityController nextTarget = FindNextTarget(entityController, out bool validTargetFound);
        entityController.CanReachTarget = validTargetFound;

        if (CheckIfNewTargetRequired(entityController, nextTarget))
            entityController.SetNewTarget(nextTarget);

        if (entityController.CurrentTarget == null && entityController.EntityType == EntityType.Mob && GameHandler.Instance.Hub != null)
            entityController.SetNewTarget(GameHandler.Instance.Hub.GetComponent<DamageableController>());
    }
    private void HandleDefendingSquad(EntityController entityController)
    {
        if (entityController.CurrentTarget == null && entityController.Squad != null)
        {
            DamageableController closestTarget = null;
            float distanceToClosestTarget = 999999f;
            foreach (EntityController entity in entityController.Squad.SquadEntities)
            {
                // Find the closest squad member to help if there are no enemies around me
                if (entity.CurrentTarget != null && Vector3.Distance(entity.CurrentTarget.transform.position, entityController.transform.position) < distanceToClosestTarget)
                {
                    closestTarget = entity.CurrentTarget;
                    distanceToClosestTarget = Vector3.Distance(entity.CurrentTarget.transform.position, entityController.transform.position);
                }
            }
            if (closestTarget != null)
            {
                entityController.SetNewTarget(closestTarget);
            }
        }
    }
    private bool CheckIfNewTargetRequired(EntityController entityController, EntityController nextTarget)
    {
        if (nextTarget == null)
            return false;

        if (nextTarget.gameObject == entityController.CurrentTarget)
            return false; // No need to waste time re-aquiring the same target.

        if (Vector3.Distance(entityController.transform.position, nextTarget.transform.position) > entityController.SightRange)
            return false; // Is the closest target to us out of our entities sight range?

        if (entityController.CurrentTarget != null)
        {
            if (entityController.CurrentTarget == GameHandler.Instance.Hub)
                return true; // We want attacking units to peel off of the Nexus and attack the defending units.

            if (Vector3.Distance(entityController.transform.position, entityController.CurrentTarget.transform.position) <= entityController.AttackRange)
                return false; // Focus our current target as long as they are within attack range.
        }

        return true;
    }
    private EntityController FindNextTarget(EntityController entityController, out bool validTargetFound)
    {
        List<EntityController> entitiesInRange = entityController.FindAllTargetsInSightRange();
        EntityController nextTarget = entityController.CurrentTarget as EntityController;
        bool foundValidTarget = false;

        if (entityController.CurrentTarget == null && entitiesInRange.Count > 0)
        {
            nextTarget = entitiesInRange[0];
        }
        else if (entityController.CurrentTarget != null)
        {
            foreach (EntityController e in entitiesInRange)
            {
                if (entityController.IsPathValid(e.transform.position))
                {
                    nextTarget = e;
                    foundValidTarget = true;
                    break;
                }
            }

            if (!foundValidTarget && entitiesInRange.Count > 0)
            {
                nextTarget = entitiesInRange[0];
            }
        }

        validTargetFound = foundValidTarget;
        return nextTarget;
    }
}
