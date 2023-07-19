using Banspad;
using Banspad.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public enum EntityType
{
    Hero,
    Mercenary,
    Mob,
    EntityCount
}
public class NavMoveCommand
{
    public Vector3 Destination;
    public float StoppingDistance;
    public float MoveSpeedModifier;

    public NavMoveCommand(Vector3 destination, float stoppingDistance=0f, float moveSpeedModifier=1f)
    {
        Destination = destination;
        StoppingDistance = stoppingDistance;
        MoveSpeedModifier = moveSpeedModifier;
    }
}
public class EntityController : DamageableController
{
    public float Debug_RemainingDistance = 0f;
    public float Debug_DistanceToHero = 0f;

    public List<float> TimeToDamageEnemy;
    public List<NavMoveCommand> NextNavDestinations;
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
    public bool CanReachNavDestination;
    public bool CanReachTarget;
    public int RandomID;
    public int QueueCount;

    protected Animator animator;
    protected Rigidbody[] rigidBodies;
    protected Vector3 lastSetNavDestination;
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
        NavObstacle = GetComponent<NavMeshObstacle>();
        NavDefaultSpeed = NavAgent.speed;
        /*if (EntityType == EntityType.Hero)
            NavAgent.avoidancePriority = Random.Range(5, 25);
        else
            NavAgent.avoidancePriority = Random.Range(30, 50);*/
        rigidBodies = GetComponentsInChildren<Rigidbody>();

        RandomID = Random.Range(0, 100000000);
        TimeToDamageEnemy = new List<float>();
        NextNavDestinations = new List<NavMoveCommand>();
        lastSetNavDestination = Vector3.zero;

        AnimationBoolInCombat = false;
        recentlyHitBySpear = false;
        debugEntityController = false;
        CanReachNavDestination = true;
        animatorPresent = animator != null;
        standUpCooldown = 2f;

        foreach (Rigidbody rigidBody in rigidBodies)
            rigidBody.isKinematic = true;
    }
    protected virtual void Start()
    {
        HealthController.onDeath += HandleDeath;
        HealthController.CustomDeath = true;

        if (gameObject.name == "Mob 1")
        {
            SkinnedMeshRenderer unitMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            Material testMat = Resources.Load("Materials/TestMat") as Material;
            unitMesh.material = testMat;
            transform.position = new Vector3(35, 0, 10);
        }
    }
    protected virtual void Update()
    {
        QueueCount = NextNavDestinations.Count;
        if (NextNavDestinations.Count > 0 && NavAgent.enabled) 
        {            
            // We dont have a destination at the moment
            if (lastSetNavDestination != NextNavDestinations[0].Destination)
                SetNavAgentDestination(NextNavDestinations[0].Destination, NextNavDestinations[0].StoppingDistance, NextNavDestinations[0].MoveSpeedModifier);
            NextNavDestinations.RemoveAt(0);
        }
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
    public EntityController FindNearestTarget()
    {
        List<SquadController> squadList = null;
        EntityController closestEntity = null;
        EntityType entityType = EntityType;
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

                if (closestEntity == null || Vector3.Distance(transform.position, enemySquadEntity.transform.position) < closestDistance)
                {
                    closestEntity = enemySquadEntity;
                    closestDistance = Vector3.Distance(transform.position, closestEntity.transform.position);
                }
            }
        }

        return closestEntity;
    }
    public List<EntityController> FindAllTargetsInSightRange()
    {
        List<EntityController> targetsInSightRange = new List<EntityController>();
        List<SquadController> squadList = null;
        EntityType entityType = EntityType;

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


                if (Vector3.Distance(transform.position, enemySquadEntity.transform.position) < SightRange)
                    targetsInSightRange.Add(enemySquadEntity);
            }
        }

        return targetsInSightRange.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
    }
    public bool MoveToAttackTarget(DamageableController target)
    {
        bool inRangeOfTarget = false;

        if (target == null)
            return false;
        if (target != CurrentTarget)
            SetNewTarget(target);

        if (NavAgent.speed != NavDefaultSpeed)
            NavAgent.speed = NavDefaultSpeed;

        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= AttackRange)
        {
            if (NavAgent.enabled)
            {
                NavAgent.SetDestination(transform.position);
                transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
                NavAgent.enabled = false;
                NavObstacle.enabled = true;
            }

            if (CurrentTarget == null)
                Logging.Log("ERROR: Current target null for some reason on " + gameObject.ToString(), true);

            inRangeOfTarget = true;
            Attack(CurrentTarget);
        }
        else
        {
            if (!InAttackAnimation)
            {
                //if (IsPathValid(CurrentTarget.transform.position))
                AddDestinationToQueue(new NavMoveCommand(CurrentTarget.transform.position), true); //NavAgent.SetDestination(CurrentTarget.transform.position);
                //else
                    //CircleCurrentTarget();
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

        if (Time.time > timeSinceLastAttack + attackCooldown)
        {
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

        if (NavObstacle != null)
            NavObstacle.enabled = false;

        if (NavAgent != null)
        {
            StartCoroutine(WaitOneFrame());
            IEnumerator WaitOneFrame()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                if (!NavObstacle.enabled)
                    NavAgent.enabled = true;
            }
        }

        CurrentTarget.HealthController.onDeath -= ClearTarget;
        CurrentTarget = null;
        AnimationBoolInCombat = false; 
        CanReachNavDestination = true;
    }
    public void HandleDeath()
    {
        Logging.Log("Entity custom death: " + gameObject.ToString(), debugEntityController);
        
        CurrentState = new EntityDeadState(this);
    }
    public void SetNavAgentDestination(Vector3 destination, float stoppingDistance=0f, float moveSpeedModifier=1f)
    {
        if (!recentlyHitBySpear && CurrentState is not EntityDeadState)
        {
            NavAgent.SetDestination(destination);
            lastSetNavDestination = destination;
            NavAgent.speed = NavDefaultSpeed * moveSpeedModifier;

            if (destination != transform.position)
                NavAgent.stoppingDistance = stoppingDistance;
            else
                NavAgent.stoppingDistance = 0f;
        }            
    }
    public bool IsPathValid(Vector3 destination)
    {
        NavMeshPath navMeshPath = new NavMeshPath(); 
        if (NavAgent.enabled && NavAgent.CalculatePath(destination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            return true;

        return false;
    }
    public void ClearNavAgentDestination()
    {
        NavAgent.SetDestination(transform.position);
        NavAgent.stoppingDistance = 0f; 
        
        if (NavAgent.speed != NavDefaultSpeed)
            NavAgent.speed = NavDefaultSpeed;
    }
    public bool HasNavAgentDestination(out bool isNavEnabled)
    {
        isNavEnabled = NavAgent.enabled;
        if (isNavEnabled)
            return NavAgent.remainingDistance - NavAgent.stoppingDistance >= 0.1f;

        return false;
    }
    public void AddDestinationToQueue(NavMoveCommand moveCommand, bool isNextDestination)
    {
        if (isNextDestination)
            NextNavDestinations.Insert(0, moveCommand);
        else
            NextNavDestinations.Add(moveCommand);
    }
    public void CircleCurrentTarget(bool rotationDirection)
    {
        // Add randomized radius to each point
        if (CurrentTarget == null)
        {
            Debug.Log(ToString() + ": circling target but target is null??");
            return;
        }
        float angleModifier = Random.Range(30f, 90f);
        Vector3 nextMoveDestination;
        //float circleRadius = Random.Range(AttackRange + 2f, AttackRange + 2f);
        float circleRadius = Random.Range(AttackRange + 1.5f, AttackRange + 2f);

        //Quaternion temp = Quaternion.LookRotation(CurrentTarget.transform.position - transform.position);
        //if (gameObject.name == "Mob 1") Debug.Log(gameObject.ToString() + ": euler = " + temp.eulerAngles);
        float currentAngleToTarget = Vector3.SignedAngle(Vector3.forward, transform.position - CurrentTarget.transform.position, Vector3.up);
        float newAngle = currentAngleToTarget + angleModifier;
        if (!rotationDirection)
            newAngle -= 2 * angleModifier;
        if (newAngle > 180)
        {
            newAngle -= 2 * (newAngle - 180); // Subtract by how much we went over 180, twice. Once to fix our mistake and a second time to actually calculate the right angle
            newAngle *= -1;
        }
        else if (newAngle < -180)
        {
            newAngle -= 2 * (newAngle + 180); // Subtract by how much we went over 180, twice. Once to fix our mistake and a second time to actually calculate the right angle
            newAngle *= -1;
        }
        float unsignedAngle = newAngle;
        if (unsignedAngle < 0)
            unsignedAngle += 360f;

        //if (gameObject.name == "Mob 1") Debug.Log(gameObject.ToString() + ": euler = " + currentAngleToTarget + " newAngle = " + newAngle + " newAngleUnsigned = " + unsignedAngle);

        float x = circleRadius * Mathf.Sin(Mathf.PI * 2 * unsignedAngle / 360);
        float y = circleRadius * Mathf.Cos(Mathf.PI * 2 * unsignedAngle / 360);

        nextMoveDestination = new Vector3(x, 0, y) + CurrentTarget.transform.position;

        if (NavAgent.enabled)
        {
            NavAgent.destination = nextMoveDestination;
            NavAgent.SetDestination(nextMoveDestination);
        }
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
