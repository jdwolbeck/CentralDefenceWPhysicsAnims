using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class HeroController : EntityController
{
    protected override void Awake()
    {
        base.Awake();

        /*HealthController hc = GetComponent<HealthController>();
        hc.onDeath += HandleDeath;
        hc.CustomDeath = true;*/
    }
    protected override void Update()
    {
        base.Update();

        //if (!recentlyHitBySpear && CurrentState is not EntityDeadState)
            //HandleDefendAI();
    }
    public void HandleDefendAI()
    {
        if (CurrentTarget != null && Vector3.Distance(transform.position, CurrentTarget.transform.position) > AttackRange)
        {
            EntityController closestEntity = FindNearestTarget(this);

            if (closestEntity != null && closestEntity.gameObject != CurrentTarget && Vector3.Distance(transform.position, closestEntity.transform.position) <= SightRange)
                CurrentTarget = closestEntity.gameObject;
        }
        if (CurrentTarget != null)
        {
            if (CurrentTarget.GetComponent<EntityController>().CurrentState is EntityDeadState)
                ClearTarget();

            AnimationBoolInCombat = MoveToAttackTarget(CurrentTarget);
        }
        else
        {
            AnimationBoolInCombat = false;

            if (!NavAgent.enabled)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }

            LookForTarget();
        }
    }
    public bool LookForTarget()
    {
        List<GameObject> foundEntities = new List<GameObject>();

        // Utilize physics to create a sphere to check for all colliders within a sight radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, SightRange);

        foreach (Collider collider in colliders)
        {
            // Check for a EntityController script on a given gameObject and their parents.
            GameObject parentGO = collider.gameObject;
            bool topLevelGO = false;
            EntityController uc;

            // Do an initial check if current GO is toplevel of Mob/Hero entity.
            if (parentGO.TryGetComponent(out uc))
            {
                if (uc.EntityType == EntityType.Mob && uc.CurrentState is not EntityDeadState)
                    topLevelGO = true;
            }

            // Keep checking for EntityController script until we find one or we are at the top level of the heirarchy. 
            while (parentGO.transform.parent != null && !topLevelGO)
            {
                parentGO = parentGO.transform.parent.gameObject;
                if (parentGO.TryGetComponent(out uc))
                {
                    if (uc.EntityType == EntityType.Mob && uc.CurrentState is not EntityDeadState)
                        topLevelGO = true;
                }
            }

            // We found a EntityController attached to a gameObject.
            if (topLevelGO)
            {
                // Add each unique entity into a list
                bool entityAccountedFor = false;

                foreach (GameObject go in foundEntities)
                {
                    if (parentGO == go)
                        entityAccountedFor = true;
                }

                if (!entityAccountedFor)
                    foundEntities.Add(parentGO);
            }
        }

        int index = 0;
        int indexOfClosestEnemy = -1;

        if (foundEntities.Count > 0)
        {
            float closestEnemy = 999999f;

            if (Vector3.Distance(transform.position, foundEntities[index].transform.position) < closestEnemy)
            {
                closestEnemy = Vector3.Distance(transform.position, foundEntities[index].transform.position);
                indexOfClosestEnemy = index;
            }

            index++;
        }

        if (indexOfClosestEnemy != -1)
        {
            CurrentTarget = foundEntities[indexOfClosestEnemy];
            CurrentTarget.GetComponent<HealthController>().onDeath += ClearTarget;

            return true;
        }

        return false;
    }
}
