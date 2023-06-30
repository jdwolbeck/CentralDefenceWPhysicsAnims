using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class MobController : EntityController
{
    //private bool debug = false;

    protected override void Start()
    {
        base.Start();
        HealthController hc = GetComponent<HealthController>();
        hc.onDeath += HandleDeath;
        hc.customDeath = true;
    }
    protected override void Update()
    {
        base.Update();
        if (!recentlyHitBySpear && !isDead)
        {
            HandleAttackAI();
        }
    }
    public void HandleAttackAI()
    {
        bool tempInCombat = false;
        EntityController closestEntity = FindNearestTarget(this);
        if (closestEntity != null && closestEntity.gameObject != currentTarget && Vector3.Distance(transform.position, closestEntity.transform.position) <= sightRange)
        {
            currentTarget = closestEntity.gameObject;
            if (currentTarget.TryGetComponent(out currentTargetHC))
            {
                currentTargetHC.onDeath += ClearTarget;
            }
        }
        if (currentTarget == null)
        {
            currentTarget = GameHandler.instance.Hub;
            if (currentTarget.TryGetComponent(out currentTargetHC))
            {
                currentTargetHC.onDeath += ClearTarget;
            }
        }
        if (currentTarget != null)
        {
            if (currentTarget.TryGetComponent(out EntityController ec))
            {
                if (ec.isDead)
                    ClearTarget();
            }
            tempInCombat = MoveToAttackTarget(currentTarget);
        }
        inCombat = tempInCombat;
    }
}
