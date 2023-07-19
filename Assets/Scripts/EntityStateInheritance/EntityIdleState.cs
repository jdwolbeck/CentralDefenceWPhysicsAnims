using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdleState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        bool hasDestination = entityController.HasNavAgentDestination(out bool isNavEnabled);
        if (entityController.CurrentTarget != null)
            return new EntityAttackingState();
        else if (hasDestination && isNavEnabled)
            return new EntityMovingToLocationState();

        return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        base.HandleStateLogic(entityController);
    }
}
