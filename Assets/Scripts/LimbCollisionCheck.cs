using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollisionCheck : MonoBehaviour
{
    [SerializeField] private EntityController entityController;
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        Debug.Log("LimbCollisionCheck");
        entityController.HitBySpear(incomingSpear, incomingVelocity, bodyPartHit);
    }
}
