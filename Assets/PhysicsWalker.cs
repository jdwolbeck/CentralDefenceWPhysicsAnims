using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsWalker : MonoBehaviour
{
    // The maximum force that will be applied to the ragdoll's joints to make it move
    public float maxForce = 10f;

    // The ragdoll's rigidbody components
    private Rigidbody[] rigidbodies;

    private float switchTime;

    // The current state of the ragdoll's left leg (1 for forward, -1 for back)
    private int leftLeg = 1;

    // The current state of the ragdoll's right leg (1 for forward, -1 for back)
    private int rightLeg = -1;

    private float additionalForce;

    void Start()
    {
        int index = 0;
        // Get all of the rigidbody components on the ragdoll
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.useGravity = false;
        }
        switchTime = Time.time;
    }

    void FixedUpdate()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            // Apply an upward force to keep the ragdoll afloat
            if (rb.name == "Chest")
            {
                //rb.AddForce(Vector3.up * 9.8f * 17);
                rb.transform.position = new Vector3(0, 2.5f, 0);
            }

            // Apply a force to the hips to make the ragdoll walk
            if (rb.name == "Hips")
            {
                rb.transform.position = new Vector3(0, 2, 0);
                //rb.AddForce(transform.forward * maxForce);
            }

            // Apply a torque to the left leg to swing it forward or back
            if (leftLeg == 1)
                additionalForce = 10f;
            else
                additionalForce = 0.5f;
            if (rb.name == "UpperLeg.L")
                rb.AddTorque(transform.up * maxForce * leftLeg * additionalForce);
            if (rb.name == "LowerLeg.L")
                rb.AddTorque(transform.up * maxForce * leftLeg * additionalForce);

            // Apply a torque to the right leg to swing it forward or back
            if (rightLeg == 1)
                additionalForce = 10f;
            else
                additionalForce = 0.5f;
            if (rb.name == "UpperLeg.R")
                rb.AddTorque(-transform.up * maxForce * rightLeg * additionalForce);
            if (rb.name == "LowerLeg.R")
                rb.AddTorque(transform.up * maxForce * rightLeg * additionalForce);

            if (switchTime + 3 < Time.time)
            {
                if (leftLeg == -1)
                    Debug.Log("Left leg");
                else
                    Debug.Log("Right leg");
                // Switch the state of the left and right legs to create a walking pattern
                leftLeg = -leftLeg;
                rightLeg = -rightLeg;
                switchTime = Time.time;
            }
        }
    }
}
