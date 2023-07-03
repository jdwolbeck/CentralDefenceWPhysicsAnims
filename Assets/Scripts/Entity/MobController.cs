using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class MobController : EntityController
{
    protected override void Awake()
    {
        base.Awake();

        HealthController hc = GetComponent<HealthController>();
        hc.onDeath += HandleDeath;
        hc.CustomDeath = true;
    }
    protected override void Update()
    {
        base.Update();

        if (!recentlyHitBySpear && !IsDead)
            HandleAttackAI();
    }
    public void HandleAttackAI()
    {
        bool tempInCombat = false;

        EntityController closestEntity = FindNearestTarget(this);

        if (closestEntity != null && closestEntity.gameObject != CurrentTarget && Vector3.Distance(transform.position, closestEntity.transform.position) <= SightRange)
        {
            CurrentTarget = closestEntity.gameObject;

            if (CurrentTarget.TryGetComponent(out currentTargetHC))
                currentTargetHC.onDeath += ClearTarget;
        }

        if (CurrentTarget == null)
        {
            CurrentTarget = GameHandler.Instance.Hub;

            if (CurrentTarget.TryGetComponent(out currentTargetHC))
                currentTargetHC.onDeath += ClearTarget;
        }

        if (CurrentTarget != null)
        {
            if (CurrentTarget.TryGetComponent(out EntityController ec))
            {
                if (ec.IsDead)
                    ClearTarget();
            }

            tempInCombat = MoveToAttackTarget(CurrentTarget);
        }

        inCombat = tempInCombat;
    }
}
