using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdleState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        if (entityController.CurrentTarget != null || entityController.HasNavAgentDestination())
            return new EntityMovingToLocationState();

        return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        base.HandleStateLogic(entityController);
    }
}
