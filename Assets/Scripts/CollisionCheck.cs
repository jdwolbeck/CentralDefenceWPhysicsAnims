using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private SpearController spearController;

    private Rigidbody rb;
    private Transform parentTransform;
    private Vector3 velocityBeforeCollision;
    private Vector3 spearPositionBeforeCollision;
    private Quaternion spearRotationBeforeCollision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        parentTransform = transform;
        velocityBeforeCollision = Vector3.zero;
        spearPositionBeforeCollision = Vector3.zero;
        spearRotationBeforeCollision = Quaternion.identity;

        while (parentTransform.parent != null)
            parentTransform = parentTransform.parent;
    }
    private void FixedUpdate()
    {
        velocityBeforeCollision = rb.velocity;
        spearPositionBeforeCollision = parentTransform.position;
        spearRotationBeforeCollision = parentTransform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
            return;

        spearController.CollidedWithObject(collision, velocityBeforeCollision, spearPositionBeforeCollision, spearRotationBeforeCollision);
    }
}
