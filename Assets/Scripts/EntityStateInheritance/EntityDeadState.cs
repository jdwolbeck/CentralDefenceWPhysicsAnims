using Banspad;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EntityDeadState : EntityState
{
    private float deathTime;
    public EntityDeadState(EntityController entityController)
    {
        Rigidbody[] rigidbodies = entityController.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
            rb.isKinematic = false;


        entityController.GetComponent<Animator>().enabled = false;
        entityController.NavAgent.enabled = false;
        entityController.NavObstacle.enabled = false;
        entityController.HealthController.HealthBar.SetActive(false);
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
        if (Time.time > deathTime + 10f)
            Object.Destroy(entityController.gameObject);
    }
}
