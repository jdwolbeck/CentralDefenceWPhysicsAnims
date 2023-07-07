using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadEngagingEnemyState : SquadState
{
    public override SquadState NextSquadState(SquadController squad)
    {
        foreach (EntityController entity in squad.SquadEntities)
        {
            if (entity.CurrentTarget != null)
                return this;
        }
        return new SquadGroupingState();
    }
    public override void HandleStateLogic(SquadController squad) { }
}
