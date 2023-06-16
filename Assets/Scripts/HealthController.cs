using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public GameObject HealthBar;
    public float TotalHealth;
    public bool customDeath = false;
    private float currentHealth;
    private Slider healthSlider;
    private bool isDead;

    public delegate void OnDeath();
    public OnDeath onDeath;

    void Start()
    {
        healthSlider = HealthBar.GetComponent<Slider>();
        currentHealth = TotalHealth;
        isDead = false;
    }
    void Update()
    {
        
    }
    public void TakeDamage(GameObject attacker, float damage)
    {
        Debug.Log("GO " + gameObject.ToString() + " took [" + damage + "] damage from " + attacker.ToString());
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            onDeath?.Invoke();
            Die();
        }
        else
        {
            if (currentHealth >= 0)
                healthSlider.normalizedValue = currentHealth / TotalHealth;
            else
                healthSlider.normalizedValue = 0f;
        }
    }
    private void Die()
    {
        if (!customDeath)
        {
            Debug.Log(gameObject.ToString() + " died.");
            Destroy(gameObject);
        }
    }
}
