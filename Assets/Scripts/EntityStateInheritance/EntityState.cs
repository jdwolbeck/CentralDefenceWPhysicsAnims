using System.Collections;
using System.Collections.Generic;
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
    }
    private void HandleEnemyProximityChecks(EntityController entityController)
    {
        EntityController closestEntity = entityController.FindNearestTarget(entityController);

        if (CheckIfNewTargetRequired(entityController, closestEntity))
        {
            entityController.CurrentTarget = closestEntity.gameObject;

            if (entityController.CurrentTarget.TryGetComponent(out HealthController currentTargetHC))
                currentTargetHC.onDeath += entityController.ClearTarget;
        }

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
                entityController.CurrentTarget = GameHandler.Instance.Hub;
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
}
