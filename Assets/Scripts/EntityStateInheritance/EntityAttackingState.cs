using UnityEngine;

public class EntityAttackingState : EntityState
{
    private float timeSinceSetDestination;
    private bool rotationDirection;
    private bool firstTimeCircling = true;

    public override EntityState NextEntityState(EntityController entityController)
    {
        if (entityController.InAttackAnimation || entityController.CurrentTarget != null)
            return this;
        else if (entityController.CurrentTarget == null)
            return new EntityIdleState();
        else if (!entityController.InAttackAnimation && Vector3.Distance(entityController.CurrentTarget.transform.position, entityController.transform.position) > entityController.AttackRange)
            return new EntityMovingToLocationState();
        else
            return this;
    }
    public override void HandleStateLogic(EntityController entityController)
    {
        base.HandleStateLogic(entityController);

        HandleAttackingTarget(entityController);
        DoDamageToTarget(entityController);
    }
    private void HandleAttackingTarget(EntityController entityController)
    {
        if (entityController.CurrentTarget != null)
        {
            if (entityController.CurrentTarget.GetComponent<EntityController>()?.CurrentState is EntityDeadState)
                entityController.ClearTarget();

            if (entityController.CanReachTarget || Vector3.Distance(entityController.transform.position, entityController.CurrentTarget.transform.position) <= entityController.AttackRange)
                entityController.AnimationBoolInCombat = entityController.MoveToAttackTarget(entityController.CurrentTarget);
            else
            {
                entityController.AnimationBoolInCombat = false;
                if (Time.time > timeSinceSetDestination)
                {
                    if (firstTimeCircling)
                    {
                        rotationDirection = false;
                        if (Random.Range(0, 2) == 0)
                            rotationDirection = true;
                        //firstTimeCircling = false;
                    }

                    entityController.CircleCurrentTarget(rotationDirection);
                    timeSinceSetDestination = Time.time + Random.Range(0.2f, 1f);
                }
            }
        }
        else
        {
            entityController.AnimationBoolInCombat = false;
        }
    }
    private void DoDamageToTarget(EntityController entityController)
    {
        if (entityController.TimeToDamageEnemy.Count > 0 && Time.time > entityController.TimeToDamageEnemy[0])
        {
            if (entityController.CurrentTarget != null)
                entityController.CurrentTarget.HealthController.TakeDamage(entityController.gameObject, entityController.Damage);

            entityController.InAttackAnimation = false;
            entityController.TimeToDamageEnemy.RemoveAt(0);
        }
    }
}
