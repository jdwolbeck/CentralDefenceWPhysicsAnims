using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class SpearController : MonoBehaviour
{
    public float totalMass;
    public float damage;
    private float timeSinceInstantiation;
    private bool hasDoneDamage;
    void Start()
    {
        timeSinceInstantiation = 0f;
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            totalMass += rb.mass;
        }
        hasDoneDamage = false;
    }
    void Update()
    {
        timeSinceInstantiation += Time.deltaTime;
        if (timeSinceInstantiation >= 30f)
        {
            Destroy(gameObject);
        }
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
        Debug.Log("Collided with object " + collision.gameObject.ToString());
        // Get the parent game object of whatever we hit
        GameObject parentGO = collision.gameObject;
        bool isUnit = false;
        if (parentGO.TryGetComponent(out UnitController tmp2))
        {
            //Debug.Log("Found unit controller on unit we hit with spear.");
            isUnit = true;
        }
        while (parentGO.transform.parent != null && !isUnit)
        {
            parentGO = parentGO.transform.parent.gameObject;
            if (parentGO.TryGetComponent(out UnitController tmp))
            {
                //Debug.Log("Found unit controller on unit we hit with spear.");
                isUnit = true;
            }
        }

        Debug.Log("Parent object " + parentGO.ToString() + " isUnit? " + isUnit);
        if (isUnit)
        { 
            RemoveRigidBodies();
            // Set this spear to be a child of the unit it hit
            transform.parent = collision.transform;
            if (parentGO.TryGetComponent(out UnitController unitController))
            {
                unitController.HitBySpear(gameObject, velocityBeforeCollision, collision.gameObject);
                if (!hasDoneDamage)
                {
                    parentGO.GetComponent<HealthController>().TakeDamage(gameObject, damage);
                    hasDoneDamage = true;
                }
            }
        }
        else
        { // Some other object other than a unit was hit, just stick based on angle
            float angleOfCollision = Vector3.Angle(collision.GetContact(0).normal, velocityBeforeCollision);
            //Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            //Debug.Log("Angle of collision: " + angleOfCollision);

            if (angleOfCollision >= 105f || angleOfCollision <= 75f)
            { // Good angle
                EnableKinematics();
            }
            else
            {
                //Debug.Log("Spear did not stick with an angle of " + angleOfCollision);
            }
        }
    }
    private void EnableKinematics()
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
    }
    private void RemoveRigidBodies()
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            if (rb.gameObject.TryGetComponent<FixedJoint>(out FixedJoint fixedJoint))
                Destroy(fixedJoint);
            BoxCollider boxCollider = rb.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null)
                Destroy(boxCollider);
            CapsuleCollider capsuleCollider = rb.GetComponentInChildren<CapsuleCollider>();
            if (capsuleCollider != null)    
                Destroy(capsuleCollider);
            CollisionCheck[] spearPartsCollChecks = rb.gameObject.GetComponentsInChildren<CollisionCheck>();
            if (spearPartsCollChecks.Length > 0)
            {
                foreach (CollisionCheck collisionCheck in spearPartsCollChecks)
                {
                    Destroy(collisionCheck);
                }
            }
            rb.isKinematic = true;
            Destroy(rb);
        }
    }
}
