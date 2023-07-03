using UnityEngine;

public class GroundItem : MonoBehaviour
{
    private void Awake()
    {
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();

        sc.center = Vector3.zero;
        sc.isTrigger = true;
    }
}
