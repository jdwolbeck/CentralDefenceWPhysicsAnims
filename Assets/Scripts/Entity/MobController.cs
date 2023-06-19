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
        if (currentTarget == null)
        {
            currentTarget = GameHandler.instance.Hub;
            if (currentTarget != null && currentTarget.TryGetComponent(out HealthController healthController))
            {
                healthController.onDeath += ClearTarget;
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
        else
        {
            tempInCombat = MoveToAttackTarget(currentTarget);
        }
        inCombat = tempInCombat;
    }
}
