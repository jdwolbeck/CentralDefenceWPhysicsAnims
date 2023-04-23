using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAnim : MonoBehaviour
{
    public GameObject basicUnit;
    // Start is called before the first frame update
    void Start()
    {
        if (basicUnit.TryGetComponent(out Animator unitsAnimator))
        {
            unitsAnimator.enabled = false;
        }
    }
}
