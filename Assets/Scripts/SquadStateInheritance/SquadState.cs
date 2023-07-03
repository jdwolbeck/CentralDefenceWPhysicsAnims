using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadState
{
    public virtual SquadState NextSquadState(SquadController squad)
    {
        return new SquadGroupingState();
    }
    public virtual void HandleStateLogic(SquadController squad) { }
}
