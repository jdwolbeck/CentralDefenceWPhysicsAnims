using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class SpearController : MonoBehaviour
{
    public float totalMass;
    private float timeSinceInstantiation;
    void Start()
    {
        timeSinceInstantiation = 0f;
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            totalMass += rb.mass;
        }
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
        Debug.Log("Collided with object: " + collision.gameObject.ToString());
        
        // Get the parent game object of whatever we hit
        GameObject parentGO = collision.gameObject;
        while (parentGO.transform.parent != null) 
        {
            parentGO = parentGO.transform.parent.gameObject;
        }
        bool isUnit = parentGO.TryGetComponent<UnitController>(out UnitController temp);
        if (isUnit)
        {
            Debug.Log("IsUnit was true.");
            RemoveRigidBodies();
            // Set this spear to be a child of the unit it hit
            //transform.SetPositionAndRotation(spearPosBeforeColl, spearRotBeforeColl);
            //transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.parent = collision.transform;
            //transform.SetParent(collision.transform, true);//parentGO.transform, false);
            if (parentGO.TryGetComponent(out UnitController unitController))
            {
                unitController.HitBySpear(gameObject, velocityBeforeCollision, collision.gameObject);
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
                Debug.Log("Spear did not stick with an angle of " + angleOfCollision);
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
