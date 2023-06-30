using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public GameObject HealthBar;
    public float TotalHealth;
    public bool customDeath = false;
    public float currentHealth { get; private set; }
    private Slider healthSlider;
    private bool isDead;
    private float timeSinceDamage;

    public delegate void OnDeath();
    public OnDeath onDeath;

    void Start()
    {
        healthSlider = HealthBar.GetComponent<Slider>();
        currentHealth = TotalHealth;
        timeSinceDamage = 0f;
        HealthBar.SetActive(false);
        isDead = false;
    }
    void Update()
    {
        HandleHealthBarVisibility();
    }
    public void TakeDamage(GameObject attacker, float damage)
    {
        //Debug.Log("GO " + gameObject.ToString() + " took [" + damage + "] damage from " + attacker.ToString());
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
            {
                healthSlider.normalizedValue = currentHealth / TotalHealth;
                HealthBar.SetActive(true);
                timeSinceDamage = Time.time;
            }
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
    private void HandleHealthBarVisibility()
    {
        if (HealthBar.activeInHierarchy && healthSlider.normalizedValue > 0.1f && Time.time >= timeSinceDamage + 5f)
        {
            HealthBar.SetActive(false);
        }
    }
}
