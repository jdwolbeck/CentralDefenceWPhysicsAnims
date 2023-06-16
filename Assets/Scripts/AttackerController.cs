using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class AttackerController : UnitController
{
    private bool debug = false;

    protected override void Start()
    {
        base.Start();
        HealthController hc = GetComponent<HealthController>();
        hc.onDeath += HandleDeath;
        hc.customDeath = true;
    }
    protected virtual void Update()
    {
        base.Update();
        if (!recentlyHitBySpear && !isDead)
        {
            if (unitType == UnitType.Attacker)
            {
                HandleAttackAI();
            }
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
