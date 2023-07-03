using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    public Gradient HealthGradient;
    public Image FillImage;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void Update()
    {
        FillImage.color = HealthGradient.Evaluate(slider.normalizedValue);
    }
}