using Banspad;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EntityType
{
    Hero,
    Mercenary,
    Mob,
    EntityCount
}
public class EntityController : DamageableController
{
    public float Debug_RemainingDistance = 0f;
    public float Debug_DistanceToHero = 0f;

    public List<float> TimeToDamageEnemy;
    public EntityState CurrentState;
    public DamageableController CurrentTarget;
    public GameObject Hips;
    public NavMeshAgent NavAgent;
    public NavMeshObstacle NavObstacle;
    public EntityItemsExtended Items;
    public SquadController Squad;
    public EntityType EntityType;
    public string EntityStateString;
    public float Damage;
    public float SightRange;
    public float AttackRange;
    public float AttackSpeed;
    public float NavDefaultSpeed;
    public bool AnimationBoolInCombat;
    public bool InAttackAnimation;
    public int RandomID;

    protected Animator animator;
    protected Rigidbody[] rigidBodies;
    protected bool animatorPresent;
    protected bool recentlyHitBySpear;
    protected bool debugEntityController;
    protected float timeSinceLastAttack;
    protected float standUpCooldown;
    protected float timeSinceSpear;

    protected override void Awake()
    {
        base.Awake();

        CurrentState = new EntityState();
        animator = GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();
        NavDefaultSpeed = NavAgent.speed;
        /*if (EntityType == EntityType.Hero)
            NavAgent.avoidancePriority = Random.Range(5, 25);
        else
            NavAgent.avoidancePriority = Random.Range(30, 50);*/
        NavObstacle = GetComponent<NavMeshObstacle>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();

        RandomID = Random.Range(0, 100000000);
        TimeToDamageEnemy = new List<float>();

        AnimationBoolInCombat = false;
        recentlyHitBySpear = false;
        debugEntityController = false;
        animatorPresent = animator != null;
        standUpCooldown = 2f;

        foreach (Rigidbody rigidBody in rigidBodies)
            rigidBody.isKinematic = true;
    }
    protected virtual void Start()
    {
        HealthController.onDeath += HandleDeath;
        HealthController.CustomDeath = true;
    }
    protected virtual void Update()
    {
        if (recentlyHitBySpear && CurrentState is not EntityDeadState)
        { // Commence stand up sequence.
            if ((timeSinceSpear + standUpCooldown) <= Time.time)
            {
                transform.position = new Vector3(Hips.transform.position.x, transform.position.y, Hips.transform.position.z);
                recentlyHitBySpear = false;
                animator.enabled = true;

                if (!NavObstacle.enabled)
                    NavAgent.enabled = true;

                foreach (Rigidbody rb in rigidBodies)
                    rb.isKinematic = true;
            }

            return;
        }

        CurrentState.HandleStateLogic(this);
        CurrentState = CurrentState.NextEntityState(this);
        EntityStateString = CurrentState.ToString();

        if (animatorPresent)
        {
            float speedPercent = NavAgent.velocity.magnitude / NavAgent.speed;

            speedPercent *= NavAgent.speed / NavDefaultSpeed;

            animator.SetFloat("SpeedPercent", speedPercent, .1f, Time.deltaTime); // BlendTree Variable, local speed%, transition time between animations, deltaTime
            animator.SetBool("InCombat", AnimationBoolInCombat);
        }
    }
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        float totalMass = 10f;
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
            totalMass += rb.mass;
        }

        animator.enabled = false;
        NavAgent.enabled = false;
        recentlyHitBySpear = true;
        timeSinceSpear = Time.time;

        float massRatio = incomingSpear.GetComponent<SpearController>().TotalMass / totalMass;
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);

        Logging.Log("Total mass of entity: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().TotalMass + " final ratio: " + massRatio, debugEntityController);
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
                if (enemySquadEntity.CurrentState is EntityDeadState)
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
    public bool MoveToAttackTarget(DamageableController target)
    {
        bool inRangeOfTarget = false;

        if (target == null)
            return false;

        SetNewTarget(target);

        if (NavAgent.speed != NavDefaultSpeed)
            NavAgent.speed = NavDefaultSpeed;

        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= AttackRange)
        {
            if (NavAgent.enabled && NavAgent.remainingDistance > 0.1f)
                NavAgent.SetDestination(transform.position);

            if (CurrentTarget == null)
                Logging.Log("ERROR: Current target null for some reason on " + gameObject.ToString(), true);

            inRangeOfTarget = true;
            Attack(CurrentTarget);
        }
        else
        {
            if (!InAttackAnimation)
            {
                if (!NavAgent.enabled)
                {
                    NavObstacle.enabled = false;
                    NavAgent.enabled = true;
                }

                NavAgent.SetDestination(CurrentTarget.transform.position);
            }
            else // This will help our entity continue the attack animation.
            {
                inRangeOfTarget = true;
            }
        }

        return inRangeOfTarget;
    }
    public void Attack(DamageableController target)
    {
        float attackCooldown = 1 / AttackSpeed;

        if (target == null)
        {
            Logging.Log("ERROR: " + gameObject.ToString() + " just tried to attack a null target.", true);
            return;
        }

        Quaternion lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);

        if (Time.time > timeSinceLastAttack + attackCooldown)
        {
            if (NavAgent.enabled)
            {
                NavAgent.enabled = false;
                NavObstacle.enabled = true;
            }

            InAttackAnimation = true;
            TimeToDamageEnemy.Add(Time.time + attackCooldown - 0.2f);
            timeSinceLastAttack = Time.time;

            Logging.Log("Adding a time to attack for " + TimeToDamageEnemy + " at current time " + timeSinceLastAttack, debugEntityController);
        }
    }
    public void SetNewTarget(DamageableController target)
    {
        if (target == null || (CurrentTarget != null && CurrentTarget == target))
            return;

        ClearTarget();

        CurrentTarget = target;
        CurrentTarget.HealthController.onDeath += ClearTarget;
    }
    public void ClearTarget()
    {
        Logging.Log("Current target died, removing", debugEntityController);

        if (CurrentTarget == null)
            return;

        CurrentTarget.HealthController.onDeath -= ClearTarget;
        CurrentTarget = null;
        AnimationBoolInCombat = false;
    }
    public void HandleDeath()
    {
        Logging.Log("Entity custom death: " + gameObject.ToString(), debugEntityController);
        
        CurrentState = new EntityDeadState(this);
    }
    public void SetNavAgentDestination(Vector3 destination, float stoppingDistance=0f, float moveSpeedModifier=1f)
    {
        if (!recentlyHitBySpear && CurrentState is not EntityDeadState && NavAgent != null)
        {
            if (!NavAgent.enabled)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }

            NavAgent.SetDestination(destination);

            if (destination != transform.position)
            {
                NavAgent.stoppingDistance = stoppingDistance;
            }

            if (NavAgent.speed == NavDefaultSpeed)
                NavAgent.speed *= moveSpeedModifier;
        }
    }
    public void ClearNavAgentDestination()
    {
        NavAgent.SetDestination(transform.position);
        NavAgent.stoppingDistance = 0f; 
        
        if (NavAgent.speed != NavDefaultSpeed)
            NavAgent.speed = NavDefaultSpeed;
    }
    public bool HasNavAgentDestination()
    {
        if (NavAgent.enabled)
            return NavAgent.remainingDistance - NavAgent.stoppingDistance >= 0.1f;

        return false;
    }
    public bool GetEntityStatusinfo(EntityStatusInfo ourStateInfo)
    {
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            ourStateInfo.Position = transform.position;
            ourStateInfo.Health = HealthController.CurrentHealth;

            return true;
        }
        else
        {
            Logging.Log("ERROR: unit (" + gameObject.ToString() + ": " + RandomID + ") was unable to find SquadController above it in heirarchy.", true);
        }

        return false;
    }
}
