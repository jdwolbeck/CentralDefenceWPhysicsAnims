using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityState
{
    public virtual EntityState NextEntityState(EntityController entityController)
    {
        return new EntityInitializingState();
    }
    public virtual void HandleStateLogic(EntityController entityController)
    {
        HandleEnemyProximityChecks(entityController);
        HandleDefendingSquad(entityController);
    }
    private void HandleEnemyProximityChecks(EntityController entityController)
    {
        EntityController closestEntity = entityController.FindNearestTarget(entityController);

        if (CheckIfNewTargetRequired(entityController, closestEntity))
            entityController.SetNewTarget(closestEntity);

        if (entityController.CurrentTarget != null)
        {
            if (entityController.CurrentTarget.GetComponent<EntityController>()?.CurrentState is EntityDeadState)
                entityController.ClearTarget();

            entityController.AnimationBoolInCombat = entityController.MoveToAttackTarget(entityController.CurrentTarget);
        }
        else
        {
            entityController.AnimationBoolInCombat = false;

            if (!entityController.NavAgent.enabled)
            {
                entityController.NavObstacle.enabled = false;
                entityController.NavAgent.enabled = true;
            }

            if (entityController.EntityType == EntityType.Mob)
                entityController.SetNewTarget(GameHandler.Instance.Hub.GetComponent<DamageableController>());
        }
    }
    private bool CheckIfNewTargetRequired(EntityController entityController, EntityController closestEntity)
    {
        if (closestEntity == null)
            return false;

        if (closestEntity.gameObject == entityController.CurrentTarget)
            return false; // No need to waste time re-aquiring the same target.

        if (Vector3.Distance(entityController.transform.position, closestEntity.transform.position) > entityController.SightRange)
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
    public void HandleDefendingSquad(EntityController entityController)
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
}
