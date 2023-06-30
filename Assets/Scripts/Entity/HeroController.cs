using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class HeroController : EntityController
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
            HandleDefendAI();
        }
    }
    public void HandleDefendAI()
    {
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange)
        {
            EntityController closestEntity = FindNearestTarget(this);
            if (closestEntity != null && closestEntity.gameObject != currentTarget && Vector3.Distance(transform.position, closestEntity.transform.position) <= sightRange)
            {
                currentTarget = closestEntity.gameObject;
            }
        }
        if (currentTarget != null)
        {
            if (currentTarget.GetComponent<EntityController>().isDead)
            {
                ClearTarget();
            }
            inCombat = MoveToAttackTarget(currentTarget);
        }
        else
        {
            inCombat = false;
            if (!navAgent.enabled)
            {
                navObstacle.enabled = false;
                navAgent.enabled = true;
            }
            LookForTarget();
        }
    }
    public bool LookForTarget()
    {
        List<GameObject> foundEntities = new List<GameObject>();
        // Utilize physics to create a sphere to check for all colliders within a sight radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider collider in colliders)
        {
            // Check for a EntityController script on a given gameObject and their parents.
            //Debug.Log("Collider found: " + collider.gameObject.ToString());
            GameObject parentGO = collider.gameObject;
            bool topLevelGO = false;
            // Do an initial check if current GO is toplevel of Mob/Hero entity.
            EntityController uc;
            if (parentGO.TryGetComponent(out uc))
            {
                if (uc.entityType == EntityType.Mob && !uc.isDead)
                    topLevelGO = true;
            }
            // Keep checking for EntityController script until we find one or we are at the top level of the heirarchy. 
            while (parentGO.transform.parent != null && !topLevelGO)
            {
                parentGO = parentGO.transform.parent.gameObject;
                if (parentGO.TryGetComponent(out uc))
                {
                    if (uc.entityType == EntityType.Mob && !uc.isDead)
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
                {
                    //Debug.Log("Collider " + collider.gameObject.ToString() + " is this GameObject (" + parentGO.ToString() + ") Adding to our foundEntities list.");
                    foundEntities.Add(parentGO);
                }
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
            currentTarget = foundEntities[indexOfClosestEnemy];

            currentTarget.GetComponent<HealthController>().onDeath += ClearTarget;
            return true;
        }
        return false;
    }
}
