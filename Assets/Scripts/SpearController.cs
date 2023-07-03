using Banspad;
using UnityEngine;

public class SpearController : MonoBehaviour
{
    public float TotalMass;
    public float Damage;

    private float timeSinceInstantiation;
    private bool hasDoneDamage;

    private void Start()
    {
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            TotalMass += rb.mass;

        timeSinceInstantiation = 0f;
        hasDoneDamage = false;
    }
    private void Update()
    {
        timeSinceInstantiation += Time.deltaTime;

        if (timeSinceInstantiation >= 30f)
            Destroy(gameObject);

        //Rigidbody rb = GetComponentInChildren<Rigidbody>();
        //Show velocity tangent of flight// Debug.DrawLine(rb.transform.position, rb.transform.position + rb.velocity, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
    }
    public void CollidedWithObject(Collision collision, Vector3 velocityBeforeCollision, Vector3 spearPosBeforeColl, Quaternion spearRotBeforeColl)
    {
        if (collision == null)
        {
            Debug.Log("We received a null collision!");
            return;
        }

        // Get the parent game object of whatever we hit
        GameObject parentGO = collision.gameObject;
        bool isEntity = false;

        if (parentGO.TryGetComponent(out EntityController tmp2))
            isEntity = true;

        while (parentGO.transform.parent != null && !isEntity)
        {
            parentGO = parentGO.transform.parent.gameObject;

            if (parentGO.TryGetComponent(out EntityController tmp))
                isEntity = true;
        }

        if (isEntity)
        { 
            RemoveRigidBodies();

            // Set this spear to be a child of the entity it hit
            transform.parent = collision.transform;

            if (parentGO.TryGetComponent(out EntityController entityController))
            {
                entityController.HitBySpear(gameObject, velocityBeforeCollision, collision.gameObject);

                if (!hasDoneDamage)
                {
                    Logging.Log("SpearController do damage to " + parentGO.ToString(), false);

                    parentGO.GetComponent<HealthController>().TakeDamage(gameObject, Damage);
                    hasDoneDamage = true;
                }
            }
        }
        else
        { // Some other object other than a entity was hit, just stick based on angle
            float angleOfCollision = Vector3.Angle(collision.GetContact(0).normal, velocityBeforeCollision);

            //Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            //Debug.Log("Angle of collision: " + angleOfCollision);

            if (angleOfCollision >= 105f || angleOfCollision <= 75f) // Good angle
                RemoveRigidBodies();
        }
    }
    private void RemoveRigidBodies()
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            if (rb.gameObject.TryGetComponent(out FixedJoint fixedJoint))
                Destroy(fixedJoint);

            BoxCollider boxCollider = rb.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null)
                Destroy(boxCollider);

            CapsuleCollider capsuleCollider = rb.GetComponentInChildren<CapsuleCollider>();
            if (capsuleCollider != null)    
                Destroy(capsuleCollider);

            CollisionCheck[] spearPartsCollChecks = rb.gameObject.GetComponentsInChildren<CollisionCheck>();
            foreach (CollisionCheck collisionCheck in spearPartsCollChecks)
                Destroy(collisionCheck);

            rb.isKinematic = true;
            Destroy(rb);
        }
    }
}
