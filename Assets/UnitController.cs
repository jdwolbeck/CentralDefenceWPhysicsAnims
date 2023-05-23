using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public float totalMass;
    private Rigidbody[] rigidBodies;
    private Animator animator;
    private float timeSinceSpear;
    private float standUpCooldown;
    private bool recentlyHitBySpear;
    private void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            totalMass += rigidBody.mass;
        }
        standUpCooldown = 2f;
        recentlyHitBySpear = false;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (recentlyHitBySpear && (timeSinceSpear + standUpCooldown) <= Time.time)
        { // Commence stand up sequence.
            recentlyHitBySpear = false;
            foreach(Rigidbody rb in rigidBodies)
            {
                rb.isKinematic = true;
            }
            animator.enabled = true;
        }
    }
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        animator.enabled = false;
        foreach(Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        float massRatio = incomingSpear.GetComponent<SpearController>().totalMass / totalMass;
        //Debug.Log("Total mass of unit: " + totalMass + " total mass of Spear: " + incomingSpear.GetComponent<SpearController>().totalMass + " final ratio: " + massRatio);
        bodyPartHit.GetComponent<Rigidbody>().AddForce(5 * massRatio * incomingVelocity, ForceMode.Impulse);
        timeSinceSpear = Time.time;
        recentlyHitBySpear = true;
    }
}
