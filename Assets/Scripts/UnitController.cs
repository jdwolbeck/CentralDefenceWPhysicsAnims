using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public enum UnitType
{
    None,
    Defender,
    Attacker
}
public class UnitController : MonoBehaviour
{
    public GameObject Hips;
    public float totalMass;
    public UnitType unitType;
    public float sightRange;
    public float attackRange;
    public float damage;
    public float attackSpeed;
    private Rigidbody[] rigidBodies;
    private Animator animator;
    private NavMeshAgent navAgent;
    private GameObject currentTarget;
    private float timeSinceLastAttack;
    private float timeSinceSpear;
    private float standUpCooldown;
    private bool recentlyHitBySpear;
    private bool animatorPresent;
    private bool inCombat;
    private bool debug = false;
    private void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            totalMass += rigidBody.mass;
        }
        navAgent = GetComponent<NavMeshAgent>();
        standUpCooldown = 2f;
        recentlyHitBySpear = false;
        animator = GetComponent<Animator>();
        if (unitType == UnitType.None)
        {
            Debug.Log("UnitType is not set for GO: " + gameObject.ToString());
        }
        animatorPresent = animator != null;
        inCombat = false;
    }
    private void Update()
    {
        if (animatorPresent)
        {
            float speedPercent = navAgent.velocity.magnitude / navAgent.speed;
            animator.SetFloat("SpeedPercent", speedPercent, .1f, Time.deltaTime); // BlendTree Variable, local speed%, transition time between animations, deltaTime
            animator.SetBool("InCombat", inCombat);
        }
        if (recentlyHitBySpear)
        { // Commence stand up sequence.
            if ((timeSinceSpear + standUpCooldown) <= Time.time)
            {
                transform.position = new Vector3(Hips.transform.position.x, transform.position.y, Hips.transform.position.z);
                recentlyHitBySpear = false;
                foreach (Rigidbody rb in rigidBodies)
                {
                    rb.isKinematic = true;
                }
                animator.enabled = true;
            }
            else
            {
            }
        }
        else
        {
            if (unitType == UnitType.Attacker)
            {
                HandleAttackAI();
            }
            if (unitType == UnitType.Defender)
            {
                HandleDefendAI();
            }
        }
    }
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        animator.enabled = false;
        foreach(Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        float massRatio = incomingSpear.GetComponent<SpearController>().totalMass / totalMass;
        //Debug.Log("Total mass of unit: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().totalMass + " final ratio: " + massRatio);
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);
        timeSinceSpear = Time.time;
        recentlyHitBySpear = true;
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
    public bool MoveToAttackTarget(GameObject target)
    {
        bool inRangeOfTarget = false;
        bool reachedTarget = false;
        if (target == null)
            return false;
        if (currentTarget != target)
            currentTarget = target;
        if (navAgent.remainingDistance < attackRange && navAgent.remainingDistance > 0.1f && navAgent.speed > 0.1f)
        {
            navAgent.SetDestination(transform.position);
            reachedTarget = true;
        }
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange)
        {
            inRangeOfTarget = true;
            if (currentTarget == null)
                Debug.Log("Current target null for some reason on " + gameObject.ToString());
            Attack(currentTarget);
        }
        else if (reachedTarget)
        {
            Debug.Log("ISSUE: We stopped moving because we reached the target but the distance to the target is not < attackRange. Trying to reassign target destination.");
            navAgent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            navAgent.SetDestination(currentTarget.transform.position);
        }
        return inRangeOfTarget;
    }
    public void Attack(GameObject target)
    {
        float attackCooldown = 1 / attackSpeed;
        if (target == null)
        {
            Debug.Log(gameObject.ToString() + " just tried to attack a null target.");
            return;
        }
        if (target.TryGetComponent(out HealthController healthController))
        {
            if (Time.time > timeSinceLastAttack + attackCooldown)
            {
                healthController.TakeDamage(gameObject, damage);
                timeSinceLastAttack = Time.time;
            }
        }
        else
        {
            Debug.Log("ISSUE: Tried to attack something without a healthController: " + target.ToString());
        }

    }
    public void ClearTarget()
    {
        Debug.Log("Current target died, removing");
        currentTarget = null;
        inCombat = false;
    }
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
                if (uc.unitType == UnitType.Attacker)
                    topLevelGO = true;
            }
            // Keep checking for UnitController script until we find one or we are at the top level of the heirarchy. 
            while (parentGO.transform.parent != null && !topLevelGO)
            {
                parentGO = parentGO.transform.parent.gameObject;
                if (parentGO.TryGetComponent(out uc))
                {
                    if (uc.unitType == UnitType.Attacker)
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
            return true;
        }
        return false;
    }
    public void PatrolCrystal()
    {

    }
}
