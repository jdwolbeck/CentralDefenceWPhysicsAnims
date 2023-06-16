using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public enum UnitType
{
    Defender,
    Attacker,
    UnitCount
}
public class UnitController : MonoBehaviour
{
    public GameObject Hips;
    public bool isDead;
    public UnitType unitType;
    public float sightRange;
    public float attackRange;
    public float damage;
    public float attackSpeed;
    protected float timeSinceLastAttack;

    protected Animator animator;
    protected bool animatorPresent;
    protected NavMeshAgent navAgent;
    protected Rigidbody[] rigidBodies;
    protected float totalMass;
    protected bool inCombat;
    protected GameObject currentTarget;
    protected float standUpCooldown;
    protected bool recentlyHitBySpear;
    protected float timeSinceSpear;
    protected float deathTime = 0f;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        animatorPresent = animator != null;
        inCombat = false;
        standUpCooldown = 2f;
        recentlyHitBySpear = false;
        navAgent = GetComponent<NavMeshAgent>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        isDead = false;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            totalMass += rigidBody.mass;
        }
    }
    protected virtual void Update()
    {
        if (deathTime != 0f)
        {
            if (GetComponent<HealthController>().HealthBar.activeInHierarchy)
            {
                GetComponent<HealthController>().HealthBar.SetActive(false);
                animator.enabled = false;
                foreach (Rigidbody rb in rigidBodies)
                {
                    rb.isKinematic = false;
                }
            }
            if (deathTime + 10f < Time.time)
            {
                Destroy(gameObject);
            }
            return;
        }
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
                navAgent.enabled = true;
            }
        }
    }
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        animator.enabled = false;
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        navAgent.enabled = false;
        float massRatio = incomingSpear.GetComponent<SpearController>().totalMass / totalMass;
        //Debug.Log("Total mass of unit: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().totalMass + " final ratio: " + massRatio);
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);
        timeSinceSpear = Time.time;
        recentlyHitBySpear = true;
    }
    public bool MoveToAttackTarget(GameObject target)
    {
        bool inRangeOfTarget = false;
        bool reachedTarget = false;
        if (target == null)
            return false;
        if (currentTarget != target)
            currentTarget = target;
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange)
        {
            if (navAgent.remainingDistance > 0.1f)
            {
                navAgent.SetDestination(transform.position);
            }
            inRangeOfTarget = true;
            if (currentTarget == null)
                Debug.Log("Current target null for some reason on " + gameObject.ToString());
            Attack(currentTarget);
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
            Quaternion lookOnLook = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);
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
    public void HandleDeath()
    {
        Debug.Log("Unit custom death: " + gameObject.ToString());

        navAgent.enabled = false;
        deathTime = Time.time;
        isDead = true;

        if (unitType == UnitType.Attacker)
        {
            WaveHandler.instance.RemoveAttackerFromList(gameObject);
        }
        ItemHandler.instance.HandleMonsterItemDrop(unitType, gameObject);
    }
}
