using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableController : MonoBehaviour
{
    public HealthController HealthController;

    protected virtual void Awake()
    {
        HealthController = GetComponent<HealthController>();
    }
}
