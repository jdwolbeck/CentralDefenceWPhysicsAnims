using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class HubController : MonoBehaviour
{
    [SerializeField] private GameObject hoveringCrystal;
    [SerializeField] private GameObject healthBar;
    private float lowHoverPoint;
    private float highHoverPoint;
    private int hoverDirection;
    private float hoverDuration;
    private float startTime;
    private float elapsedTime;
    private bool firstTimeHover;

    void Start()
    {
        lowHoverPoint = hoveringCrystal.transform.position.y;
        highHoverPoint = lowHoverPoint + 1f;
        hoverDirection = 1;
        hoverDuration = 1.5f;
        firstTimeHover = true;
    }
    void Update()
    {
        HoverCrystal();
    }
    private void HoverCrystal()
    {
        if (firstTimeHover)
        {
            firstTimeHover = false;
            startTime = Time.time;
        }
        float startPoint;
        float endPoint;
        if (hoverDirection == 1)
        {
            startPoint = lowHoverPoint;
            endPoint = highHoverPoint;
        }
        else
        {
            startPoint = highHoverPoint;
            endPoint = lowHoverPoint;
        }
        elapsedTime = Time.time - startTime;
        if (elapsedTime < hoverDuration)
        {
            hoveringCrystal.transform.position = new Vector3(hoveringCrystal.transform.position.x, easeInOutQuad(startPoint, endPoint, elapsedTime / hoverDuration), hoveringCrystal.transform.position.z);
            healthBar.transform.position = new Vector3(healthBar.transform.position.x, 1.11f + easeInOutQuad(startPoint, endPoint, elapsedTime / hoverDuration), healthBar.transform.position.z);
        }
        else
        {
            hoverDirection = -hoverDirection;
            startTime = Time.time;
        }
    }
    private float easeInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
    }
}
