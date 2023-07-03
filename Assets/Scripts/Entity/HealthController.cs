using Banspad;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    public float TotalHealth;
    public bool CustomDeath;
    public GameObject HealthBar;

    private Slider healthSlider;
    private bool isDead;
    private bool healthDebug;
    private float timeSinceDamage;

    public delegate void OnDeath();
    public OnDeath onDeath;

    private void Awake()
    {
        healthSlider = HealthBar.GetComponent<Slider>();
        CurrentHealth = TotalHealth;
        timeSinceDamage = 0f;
        HealthBar.SetActive(false);
        isDead = false;
        CustomDeath = false;
        healthDebug = false;
    }
    private void Update()
    {
        HandleHealthBarVisibility();
    }
    public void TakeDamage(GameObject attacker, float damage)
    {
        Logging.Log("GO " + gameObject.ToString() + " took [" + damage + "] damage from " + attacker.ToString(), healthDebug);

        CurrentHealth -= damage;

        if (CurrentHealth <= 0 && !isDead)
        {
            isDead = true;
            onDeath?.Invoke();
            Die();
        }
        else
        {
            if (CurrentHealth >= 0)
            {
                healthSlider.normalizedValue = CurrentHealth / TotalHealth;
                HealthBar.SetActive(true);
                timeSinceDamage = Time.time;
            }
            else
            {
                healthSlider.normalizedValue = 0f;
            }
        }
    }
    private void Die()
    {
        if (!CustomDeath)
        {
            Logging.Log(gameObject.ToString() + " died.", healthDebug);
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
