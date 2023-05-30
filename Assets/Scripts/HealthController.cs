using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] private GameObject HealthBar;
    public float TotalHealth;
    private float currentHealth;
    private Slider healthSlider;

    public delegate void OnDeath();
    public OnDeath onDeath;

    void Start()
    {
        healthSlider = HealthBar.GetComponent<Slider>();
        currentHealth = TotalHealth;
    }
    void Update()
    {
        
    }
    public void TakeDamage(GameObject attacker, float damage)
    {
        Debug.Log("GO " + gameObject.ToString() + " took [" + damage + "] damage from " + attacker.ToString());
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
            Die();
        }
        else
        {
            healthSlider.normalizedValue = currentHealth / TotalHealth;
        }
    }
    private void Die()
    {
        Debug.Log(gameObject.ToString() + " died.");
        Destroy(gameObject);
    }
}
