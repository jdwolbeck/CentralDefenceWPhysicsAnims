using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField]
    private SpearController spearController;
    private Rigidbody rb;
    Vector3 velocityBeforeCollision;
    Vector3 spearPositionBeforeCollision;
    Quaternion spearRotationBeforeCollision;
    Transform parentGO;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocityBeforeCollision = Vector3.zero;
        spearPositionBeforeCollision = Vector3.zero;
        spearRotationBeforeCollision = Quaternion.identity;
        parentGO = transform;
        while (parentGO.parent != null)
        {
            parentGO = parentGO.parent;
        }
    }
    private void FixedUpdate()
    {
        velocityBeforeCollision = rb.velocity;
        spearPositionBeforeCollision = parentGO.position;
        spearRotationBeforeCollision = parentGO.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEntered SPEAR...");
        if (collision == null)
            return;
        spearController.CollidedWithObject(collision, velocityBeforeCollision, spearPositionBeforeCollision, spearRotationBeforeCollision);
    }
}
