using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovingToLocationState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        if (entityController.CurrentTarget != null && Vector3.Distance(entityController.CurrentTarget.transform.position, entityController.transform.position) <= entityController.AttackRange)
            return new EntityAttackingState();
        else if (entityController.CurrentTarget == null && !entityController.HasNavAgentDestination())
            return new EntityIdleState();

        return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        base.HandleStateLogic(entityController);

        if (entityController.NavAgent.enabled && (entityController.NavAgent.remainingDistance - entityController.NavAgent.stoppingDistance) < 0.1)
            entityController.ClearNavAgentDestination();
    }
}
