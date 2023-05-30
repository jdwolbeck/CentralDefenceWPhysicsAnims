using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollisionCheck : MonoBehaviour
{
    [SerializeField] private UnitController unitController;
    public void HitBySpear(GameObject incomingSpear, Vector3 incomingVelocity, GameObject bodyPartHit)
    {
        Debug.Log("LimbCollisionCheck");
        unitController.HitBySpear(incomingSpear, incomingVelocity, bodyPartHit);
    }
}
