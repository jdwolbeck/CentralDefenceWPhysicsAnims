using Banspad;
using Banspad.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.Animations;

public enum EntityType
{
    Hero,
    Mercenary,
    Mob,
    EntityCount
}
public class EntityController : MonoBehaviour
{
    protected float NAVAGENT_SPEED;
    public GameObject Hips;
    public bool isDead;
    public EntityType entityType;
    public float sightRange;
    public float attackRange;
    public float damage;
    public float attackSpeed;
    public GameObject currentTarget;
    public int randomID;

    public EntityItemsExtended Items;

    protected float timeSinceLastAttack;
    protected Animator animator;
    protected bool animatorPresent;
    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;
    protected Rigidbody[] rigidBodies;
    protected float totalMass;
    protected bool inCombat;
    protected float standUpCooldown;
    protected bool recentlyHitBySpear;
    protected float timeSinceSpear;
    protected float deathTime = 0f;
    protected bool isPatrolMoving;
    protected float stoppingDist;
    protected bool inAttackAnimation;

    protected bool debugEntityController = false;

    protected HealthController currentTargetHC;
    protected List<float> timeToDamageEnemy;

    protected virtual void Start()
    {
        randomID = UnityEngine.Random.Range(0, 100000000);
        animator = GetComponent<Animator>();
        animatorPresent = animator != null;
        inCombat = false;
        standUpCooldown = 2f;
        recentlyHitBySpear = false;
        navAgent = GetComponent<NavMeshAgent>();
        NAVAGENT_SPEED = navAgent.speed;
        navObstacle = GetComponent<NavMeshObstacle>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        isDead = false;
        isPatrolMoving = false;
        stoppingDist = 0f;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            totalMass += rigidBody.mass;
        }
        timeToDamageEnemy = new List<float>();

        // Configure Equipment inventory setup
        Items = new EntityItemsExtended();
        //Equipment
        Items.EquipmentContainer = new ItemContainerEquipment(false);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.OneHandWeapon);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Shield);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Helmet);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Chest);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Legs);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Gloves);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Boots);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Belt);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.RingLeft);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.RingRight);
        Items.EquipmentContainer.DefineSingleEquipmentSlot((int)ItemGroupsEnum.Amulet);

        //Other Storage
        Items.AddNewGenericStorage((int)StorageTypesEnum.EquipmentCharms, 10, 1, new List<int>() { (int)ItemGroupsEnum.Charm });
    }
    protected virtual void Update()
    {
        if (deathTime != 0f || isDead)
        {
            if (deathTime + 10f < Time.time)
            {
                Destroy(gameObject);
            }
            return;
        }
        if (animatorPresent)
        {
            float speedPercent = navAgent.velocity.magnitude / navAgent.speed;
            if (isPatrolMoving)
                speedPercent /= 2f;
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
        if (timeToDamageEnemy.Count > 0 && Time.time > timeToDamageEnemy[0])
        {
            if (currentTargetHC != null)
            {
                currentTargetHC.TakeDamage(gameObject, damage);
            }
            inAttackAnimation = false;
            timeToDamageEnemy.RemoveAt(0);
        }
        if (isPatrolMoving && (Vector3.Distance(navAgent.destination, transform.position) <= stoppingDist))
        {
            isPatrolMoving = false;
            navAgent.SetDestination(transform.position);
            stoppingDist = 0f;
            if (navAgent.speed != NAVAGENT_SPEED)
                navAgent.speed = NAVAGENT_SPEED;
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
        if (debugEntityController) Debug.Log("Total mass of entity: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().totalMass + " final ratio: " + massRatio);
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);
        timeSinceSpear = Time.time;
        recentlyHitBySpear = true;
    }
    public bool MoveToAttackTarget(GameObject target)
    {
        bool inRangeOfTarget = false;
        if (target == null)
            return false;
        if (currentTarget != target)
            currentTarget = target;
        if (isPatrolMoving)
        {
            isPatrolMoving = false;
            if (navAgent.speed != NAVAGENT_SPEED)
                navAgent.speed = NAVAGENT_SPEED;
        }

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange)
        {
            if (navAgent.enabled && navAgent.remainingDistance > 0.1f)
            {
                navAgent.SetDestination(transform.position);
            }
            inRangeOfTarget = true;
            if (currentTarget == null)
                Debug.Log("ERROR: Current target null for some reason on " + gameObject.ToString());
            Attack(currentTarget);
        }
        else
        {
            if (!inAttackAnimation)
            {
                if (!navAgent.enabled)
                {
                    navObstacle.enabled = false;
                    navAgent.enabled = true;
                }
                navAgent.SetDestination(currentTarget.transform.position);
            }
            else // This will help our entity continue the attack animation.
                inRangeOfTarget = true;
        }
        return inRangeOfTarget;
    }
    public void Attack(GameObject target)
    {
        float attackCooldown = 1 / attackSpeed;
        if (target == null)
        {
            Debug.Log("ERROR: " + gameObject.ToString() + " just tried to attack a null target.");
            return;
        }
        if (target.TryGetComponent(out HealthController healthController))
        {
            Quaternion lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);
            if (Time.time > timeSinceLastAttack + attackCooldown)
            {
                inAttackAnimation = true;
                if (navAgent.enabled)
                {
                    navAgent.enabled = false;
                    navObstacle.enabled = true;
                }
                timeToDamageEnemy.Add(Time.time + attackCooldown - 0.2f);
                //Debug.Log("Adding a time to attack for " + timeToDamageEnemy + " at current time " + Time.time);
                currentTargetHC = healthController;
                //healthController.TakeDamage(gameObject, damage);
                timeSinceLastAttack = Time.time;
            }
        }
        else
        {
            Debug.Log("ERROR: Tried to attack something without a healthController: " + target.ToString());
        }

    }
    public void ClearTarget()
    {
        if (debugEntityController) Debug.Log("Current target died, removing");
        currentTarget = null;
        currentTargetHC = null;
        inCombat = false;
    }
    public void HandleDeath()
    {
        if (debugEntityController) Debug.Log("Entity custom death: " + gameObject.ToString());

        navAgent.enabled = false;
        navObstacle.enabled = false;
        GetComponent<HealthController>().HealthBar.SetActive(false);
        animator.enabled = false;
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        deathTime = Time.time;
        isDead = true;

        if (entityType == EntityType.Mob)
        {
            WaveHandler.instance.RemoveMobFromList(gameObject);
        }
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            if (debugEntityController) Debug.Log("Removing entity " + ToString() + " from Squad " + squadController.ToString());
            squadController.RemoveEntityFromSquad(this);
        }
        ItemHandler.instance.HandleMonsterItemDrop(entityType, gameObject);
    }
    public bool GetEntityStateInfo(EntityStateInfo ourStateInfo)
    {
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            ourStateInfo.Position = transform.position;
            ourStateInfo.Health = GetComponent<HealthController>().currentHealth;
            return true;
        }
        else
        {
            Debug.Log("ERROR: unit (" + gameObject.ToString() + ": " + randomID + ") was unable to find SquadController above it in heirarchy.");
        }
        return false;
    }
    public EntityController FindNearestTarget(EntityController entity)
    {
        EntityController closestEntity = null;
        EntityType entityType = entity.entityType;
        float closestDistance = 0f;
        List<SquadController> squadList = null;
        if (entityType == EntityType.Hero || entityType == EntityType.Mercenary)
        {
            squadList = GameHandler.instance.mobSquads;
        }
        else
        {
            squadList = GameHandler.instance.playerSquads;
        }

        foreach (SquadController squadController in squadList)
        {
            foreach (EntityController enemySquadEntity in squadController.EntityList)
            {
                if (enemySquadEntity.isDead) continue;
                if (closestEntity == null ||
                    Vector3.Distance(entity.transform.position, enemySquadEntity.transform.position) < closestDistance)
                {
                    closestEntity = enemySquadEntity;
                    closestDistance = Vector3.Distance(entity.transform.position, closestEntity.transform.position);
                }
            }
        }

        return closestEntity;
    }
    public void SetNavAgentDestination(Vector3 destination, float stoppingDistance=0f, float moveSpeedModifier=1f)
    {
        if (!recentlyHitBySpear && !isDead)
        {
            if (!navAgent.enabled)
            {
                navObstacle.enabled = false;
                navAgent.enabled = true;
            }
            navAgent.SetDestination(destination);
            if (destination != transform.position)
            {
                stoppingDist = stoppingDistance;
                isPatrolMoving = true;
            }
            if (navAgent.speed == NAVAGENT_SPEED)
                navAgent.speed *= moveSpeedModifier;
        }
    }
    public bool HasNavAgentDestination()
    {
        return isPatrolMoving;
    }
}
