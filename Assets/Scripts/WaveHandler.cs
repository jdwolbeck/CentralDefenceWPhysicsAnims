using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Wave
{
    public int numAttackers;
    public float intensity;
}
public class WaveHandler : MonoBehaviour
{
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private List<GameObject> currentAttackers = new List<GameObject>();
    private GameObject attackerPrefab;
    private int waveIndex;
    private bool currentWaveInitialized;
    private float spawnRadius;

    void Start()
    {
        for (int i = 1; i < 5; i++)
        {
            Wave wave = new Wave();
            wave.numAttackers = i;
            wave.intensity = i;
            waves.Add(wave);
        }
        attackerPrefab = Resources.Load("Prefabs/Attacker") as GameObject;
        currentWaveInitialized = false;
        spawnRadius = 20f;
    }
    void Update()
    {
        if (!currentWaveInitialized)
            InitializeWave();

        HandleWaveLogic();
    }
    private void InitializeWave()
    {
        // Spawn all of the attackers and store them in our currentAttackers list
        for (int i = 0; i < waves[waveIndex].numAttackers; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(Random.Range(-spawnRadius, spawnRadius), Random.Range(0, 2));
            GameObject attacker = Instantiate(attackerPrefab, spawnPosition, Quaternion.LookRotation(Vector3.zero - spawnPosition));
            currentAttackers.Add(attacker);
        }
        currentWaveInitialized = true;
    }
    private void HandleWaveLogic()
    {
        if (currentAttackers.Count == 0)
        {

        }
    }
    private Vector3 GetSpawnPosition(float spawnXPosition, int spawnUpOrDown)
    {
        // Circle Equation: (X - H)^2 + (Y - K)^2 = r^2  ** H & K are both 0
        Vector2 spawnPoint = new Vector2(spawnXPosition, 0);
        if (spawnUpOrDown == 1)
        {
            // Positive direction requested
            spawnPoint.y = Mathf.Sqrt(spawnRadius*spawnRadius - spawnXPosition*spawnXPosition);
        }
        else
        {
            // Negative direction requested
            spawnPoint.y = -Mathf.Sqrt(spawnRadius * spawnRadius - spawnXPosition * spawnXPosition);
        }
        return new Vector3(spawnPoint.x, 0, spawnPoint.y);
    }
}
