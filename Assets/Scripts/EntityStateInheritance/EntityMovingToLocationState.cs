using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EntityMovingToLocationState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        bool hasDestination = entityController.HasNavAgentDestination(out bool isNavEnabled);
        if (entityController.CurrentTarget != null)
            return new EntityAttackingState();
        else if (!hasDestination || !isNavEnabled)
            return new EntityIdleState();

        return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        base.HandleStateLogic(entityController);

        /*if ((entityController.NavAgent.remainingDistance - entityController.NavAgent.stoppingDistance) < 0.1)
            entityController.ClearNavAgentDestination();*/
        /*entityController.CanReachTargetDestination();

        bool hasDestination = entityController.HasNavAgentDestination(out bool isNavEnabled);
        if (!hasDestination && isNavEnabled )//&& entityController.CanReachDestination(entityController.CurrentTarget.transform.position))
        {
            entityController.MoveToAttackTarget(entityController.CurrentTarget);
        }*/
    }
}
