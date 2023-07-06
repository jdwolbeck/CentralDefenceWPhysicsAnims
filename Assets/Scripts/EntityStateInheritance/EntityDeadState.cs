using Banspad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityDeadState : EntityState
{
    private float deathTime;
    public EntityDeadState(EntityController entityController)
    {
        Rigidbody[] rigidbodies = entityController.gameObject.GetComponentsInChildren<Rigidbody>();
        Animator animator = entityController.gameObject.GetComponent<Animator>();
        NavMeshAgent navAgent = entityController.gameObject.GetComponent<NavMeshAgent>();
        NavMeshObstacle navObstacle = entityController.gameObject.GetComponent<NavMeshObstacle>();

        foreach (Rigidbody rb in rigidbodies)
            rb.isKinematic = false;

        navAgent.enabled = false;
        navObstacle.enabled = false;
        animator.enabled = false;
        entityController.EntityHC.HealthBar.SetActive(false);
        deathTime = Time.time;

        if (entityController.EntityType == EntityType.Mob)
            WaveHandler.Instance.RemoveMobFromList(entityController.gameObject);

        if (entityController.transform.parent != null && entityController.transform.parent.gameObject.TryGetComponent(out SquadController squadController))
        {
            Logging.Log("Removing entity " + ToString() + " from Squad " + squadController.ToString(), false);

            squadController.RemoveEntityFromSquad(entityController);
        }

        ItemHandler.Instance.HandleMonsterItemDrop(entityController.EntityType, entityController.gameObject);
    }
    public override EntityState NextEntityState(EntityController entityController)
    {
        return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        if (deathTime + 10f < Time.time)
            UnityEngine.Object.Destroy(entityController.gameObject);
    }
}
