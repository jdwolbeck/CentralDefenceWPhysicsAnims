using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    private void Start()
    {
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.center = Vector3.zero;
        sc.isTrigger = true;
        //sc.radius = 15f;
    }
}
