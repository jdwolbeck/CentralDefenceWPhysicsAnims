using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class DefenderController : UnitController
{
    private bool debug = false;
    
    protected override void Start()
    {
        base.Start();
        if (unitType == UnitType.None)
        {
            Debug.Log("UnitType is not set for GO: " + gameObject.ToString());
        }
        if (unitType == UnitType.Attacker)
        {
            HealthController hc = GetComponent<HealthController>();
            hc.onDeath += HandleDeath;
            hc.customDeath = true;
        }
    }
    protected virtual void Update()
    {
        base.Update();
        if (!recentlyHitBySpear && !isDead)
        {
            if (unitType == UnitType.Attacker)
            {
                //HandleAttackAI();
            }
            if (unitType == UnitType.Defender)
            {
                HandleDefendAI();
            }
        }
    }
    /*public void HandleAttackAI()
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
    }*/
    public void HandleDefendAI()
    {
        if (currentTarget != null)
        {
            MoveToAttackTarget(currentTarget);
        }
        if (currentTarget == null)
        {
            if (!LookForTarget())
                PatrolCrystal();
        }
    }
    public bool LookForTarget()
    {
        List<GameObject> foundUnits = new List<GameObject>();
        // Utilize physics to create a sphere to check for all colliders within a sight radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider collider in colliders)
        {
            // Check for a UnitController script on a given gameObject and their parents.
            //Debug.Log("Collider found: " + collider.gameObject.ToString());
            GameObject parentGO = collider.gameObject;
            bool topLevelGO = false;
            // Do an initial check if current GO is toplevel of Attacker/Defender unit.
            UnitController uc;
            if (parentGO.TryGetComponent(out uc))
            {
                if (uc.unitType == UnitType.Attacker && !uc.isDead)
                    topLevelGO = true;
            }
            // Keep checking for UnitController script until we find one or we are at the top level of the heirarchy. 
            while (parentGO.transform.parent != null && !topLevelGO)
            {
                parentGO = parentGO.transform.parent.gameObject;
                if (parentGO.TryGetComponent(out uc))
                {
                    if (uc.unitType == UnitType.Attacker && !uc.isDead)
                        topLevelGO = true;
                }
            }
            // We found a UnitController attached to a gameObject.
            if (topLevelGO)
            {
                // Add each unique Attacker/Defender into a list
                bool unitAccountedFor = false;
                foreach (GameObject go in foundUnits)
                {
                    if (parentGO == go)
                        unitAccountedFor = true;
                }
                if (!unitAccountedFor)
                {
                    //Debug.Log("Collider " + collider.gameObject.ToString() + " is this GameObject (" + parentGO.ToString() + ") Adding to our foundUnits list.");
                    foundUnits.Add(parentGO);
                }
            }
        }
        int index = 0;
        int indexOfClosestEnemy = -1;
        if (foundUnits.Count > 0)
        {
            float closestEnemy = 999999f;
            if (Vector3.Distance(transform.position, foundUnits[index].transform.position) < closestEnemy)
            {
                closestEnemy = Vector3.Distance(transform.position, foundUnits[index].transform.position);
                indexOfClosestEnemy = index;
            }
            index++;
        }
        if (indexOfClosestEnemy != -1)
        {
            currentTarget = foundUnits[indexOfClosestEnemy];

            currentTarget.GetComponent<HealthController>().onDeath += ClearTarget;
            return true;
        }
        return false;
    }
    public void PatrolCrystal()
    {

    }
}
