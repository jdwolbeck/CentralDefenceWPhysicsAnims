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
    public GameObject Hips;
    public GameObject CurrentTarget;
    public EntityItemsExtended Items;
    public EntityType EntityType;
    public int RandomID;
    public bool IsDead;
    public float Damage;
    public float SightRange;
    public float AttackRange;
    public float AttackSpeed;

    protected List<float> timeToDamageEnemy;
    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;
    protected Rigidbody[] rigidBodies;
    protected HealthController currentTargetHC;
    protected bool animatorPresent;
    protected bool inCombat;
    protected bool isPatrolMoving;
    protected bool recentlyHitBySpear;
    protected bool inAttackAnimation;
    protected bool debugEntityController;
    protected float navDefaultSpeed;
    protected float timeSinceLastAttack;
    protected float totalMass;
    protected float standUpCooldown;
    protected float timeSinceSpear;
    protected float deathTime;
    protected float stoppingDist;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navObstacle = GetComponent<NavMeshObstacle>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();

        RandomID = UnityEngine.Random.Range(0, 100000000);
        timeToDamageEnemy = new List<float>();

        IsDead = false;
        inCombat = false;
        isPatrolMoving = false;
        recentlyHitBySpear = false;
        debugEntityController = false;
        animatorPresent = animator != null;
        deathTime = 0f;
        stoppingDist = 0f;
        standUpCooldown = 2f;
        navDefaultSpeed = navAgent.speed;

        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            totalMass += rigidBody.mass;
        }

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
        if (deathTime != 0f || IsDead)
        {
            if (deathTime + 10f < Time.time)
                Destroy(gameObject);

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
                animator.enabled = true;
                navAgent.enabled = true;

                foreach (Rigidbody rb in rigidBodies)
                    rb.isKinematic = true;
            }
        }
        if (timeToDamageEnemy.Count > 0 && Time.time > timeToDamageEnemy[0])
        {
            if (currentTargetHC != null)
                currentTargetHC.TakeDamage(gameObject, Damage);

            inAttackAnimation = false;
            timeToDamageEnemy.RemoveAt(0);
        }
        if (isPatrolMoving && (Vector3.Distance(navAgent.destination, transform.position) <= stoppingDist))
        {
            navAgent.SetDestination(transform.position);

            isPatrolMoving = false;
            stoppingDist = 0f;

            if (navAgent.speed != navDefaultSpeed)
                navAgent.speed = navDefaultSpeed;
        }
    }
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        foreach (Rigidbody rb in rigidBodies)
            rb.isKinematic = false;

        animator.enabled = false;
        navAgent.enabled = false;
        recentlyHitBySpear = true;
        timeSinceSpear = Time.time;

        float massRatio = incomingSpear.GetComponent<SpearController>().TotalMass / totalMass;
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);

        Logging.Log("Total mass of entity: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().TotalMass + " final ratio: " + massRatio, debugEntityController);
    }
    public bool MoveToAttackTarget(GameObject target)
    {
        bool inRangeOfTarget = false;

        if (target == null)
            return false;

        if (CurrentTarget != target)
            CurrentTarget = target;

        if (isPatrolMoving)
        {
            isPatrolMoving = false;
            if (navAgent.speed != navDefaultSpeed)
                navAgent.speed = navDefaultSpeed;
        }

        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) < AttackRange)
        {
            if (navAgent.enabled && navAgent.remainingDistance > 0.1f)
                navAgent.SetDestination(transform.position);

            if (CurrentTarget == null)
                Logging.Log("ERROR: Current target null for some reason on " + gameObject.ToString(), true);

            inRangeOfTarget = true;
            Attack(CurrentTarget);
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

                navAgent.SetDestination(CurrentTarget.transform.position);
            }
            else // This will help our entity continue the attack animation.
            {
                inRangeOfTarget = true;
            }
        }

        return inRangeOfTarget;
    }
    public void Attack(GameObject target)
    {
        float attackCooldown = 1 / AttackSpeed;

        if (target == null)
        {
            Logging.Log("ERROR: " + gameObject.ToString() + " just tried to attack a null target.", true);
            return;
        }

        if (target.TryGetComponent(out HealthController healthController))
        {
            Quaternion lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);

            if (Time.time > timeSinceLastAttack + attackCooldown)
            {
                if (navAgent.enabled)
                {
                    navAgent.enabled = false;
                    navObstacle.enabled = true;
                }

                inAttackAnimation = true;
                timeToDamageEnemy.Add(Time.time + attackCooldown - 0.2f);
                currentTargetHC = healthController;
                timeSinceLastAttack = Time.time;

                Logging.Log("Adding a time to attack for " + timeToDamageEnemy + " at current time " + timeSinceLastAttack, debugEntityController);
            }
        }
        else
        {
            Logging.Log("ERROR: Tried to attack something without a healthController: " + target.ToString(), true);
        }

    }
    public void ClearTarget()
    {
        Logging.Log("Current target died, removing", debugEntityController);

        inCombat = false;
        CurrentTarget = null;
        currentTargetHC = null;
    }
    public void HandleDeath()
    {
        Logging.Log("Entity custom death: " + gameObject.ToString(), debugEntityController);

        foreach (Rigidbody rb in rigidBodies)
            rb.isKinematic = false;

        navAgent.enabled = false;
        navObstacle.enabled = false;
        animator.enabled = false;
        IsDead = true;
        deathTime = Time.time;

        GetComponent<HealthController>().HealthBar.SetActive(false);

        if (EntityType == EntityType.Mob)
            WaveHandler.Instance.RemoveMobFromList(gameObject);

        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            Logging.Log("Removing entity " + ToString() + " from Squad " + squadController.ToString(), debugEntityController);

            squadController.RemoveEntityFromSquad(this);
        }

        ItemHandler.Instance.HandleMonsterItemDrop(EntityType, gameObject);
    }
    public bool GetEntityStatusinfo(EntityStatusInfo ourStateInfo)
    {
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            ourStateInfo.Position = transform.position;
            ourStateInfo.Health = GetComponent<HealthController>().CurrentHealth;

            return true;
        }
        else
        {
            Logging.Log("ERROR: unit (" + gameObject.ToString() + ": " + RandomID + ") was unable to find SquadController above it in heirarchy.", true);
        }

        return false;
    }
    public EntityController FindNearestTarget(EntityController entity)
    {
        List<SquadController> squadList = null;
        EntityController closestEntity = null;
        EntityType entityType = entity.EntityType;
        float closestDistance = 0f;

        if (entityType == EntityType.Hero || entityType == EntityType.Mercenary)
            squadList = GameHandler.Instance.MobSquads;
        else
            squadList = GameHandler.Instance.PlayerSquads;

        foreach (SquadController squadController in squadList)
        {
            foreach (EntityController enemySquadEntity in squadController.SquadEntities)
            {
                if (enemySquadEntity.IsDead) 
                    continue;

                if (closestEntity == null || Vector3.Distance(entity.transform.position, enemySquadEntity.transform.position) < closestDistance)
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
        if (!recentlyHitBySpear && !IsDead && navAgent != null)
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

            if (navAgent.speed == navDefaultSpeed)
                navAgent.speed *= moveSpeedModifier;
        }
    }
    public bool HasNavAgentDestination()
    {
        return isPatrolMoving;
    }
}
