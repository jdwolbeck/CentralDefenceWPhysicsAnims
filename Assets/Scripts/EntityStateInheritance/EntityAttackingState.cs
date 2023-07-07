using UnityEngine;

public class EntityAttackingState : EntityState
{
    public override EntityState NextEntityState(EntityController entityController)
    {
        if (entityController.InAttackAnimation)
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

        if (entityController.TimeToDamageEnemy.Count > 0 && Time.time > entityController.TimeToDamageEnemy[0])
        {
            if (entityController.CurrentTarget != null)
                entityController.CurrentTarget.HealthController.TakeDamage(entityController.gameObject, entityController.Damage);

            entityController.InAttackAnimation = false;
            entityController.TimeToDamageEnemy.RemoveAt(0);
        }
    }
}
