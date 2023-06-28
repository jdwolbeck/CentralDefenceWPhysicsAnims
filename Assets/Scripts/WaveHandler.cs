using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct Wave
{
    public int numMobs;
    public float intensity;
}
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler instance { get; private set; }
    public GameObject MobFolder;
    public TMP_Text WaveTextBox;
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private List<GameObject> currentMobs = new List<GameObject>();
    private GameObject mobPrefab;
    private GameObject squadPrefab;
    private SquadController mobSquad;
    private int waveIndex;
    private bool currentWaveInitialized;
    private float spawnRadius;
    private float startOfWave;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    void Start()
    {
        for (int i = 1; i < 10000; i++)
        {
            Wave wave = new Wave();
            wave.numMobs = i;
            wave.intensity = i;
            waves.Add(wave);
        }
        mobPrefab = Resources.Load("Prefabs/Mob") as GameObject;
        squadPrefab = Resources.Load("Prefabs/Squad") as GameObject;
        currentWaveInitialized = false;
        spawnRadius = 50f;
    }
    void Update()
    {
        if (!currentWaveInitialized)
            InitializeWave();

        HandleWaveLogic();
    }
    public bool RemoveMobFromList(GameObject mob)
    {
        foreach (GameObject go in currentMobs)
        {
            if (go == mob)
            {
                currentMobs.Remove(go);
                return true;
            }    
        }
        return false;
    }
    private void InitializeWave()
    {
        if (waveIndex >= waves.Count)
        {
            WaveTextBox.text = "GAME OVER";
            return;
        }
        if (mobSquad == null)
        {
            GameObject squad = Instantiate(squadPrefab, MobFolder.transform);
            mobSquad = squad.GetComponent<SquadController>();
        }
        // Spawn all of the mobs and store them in our currentMobss list
        for (int i = 0; i < waves[waveIndex].numMobs; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(Random.Range(-spawnRadius, spawnRadius), Random.Range(0, 2));
            GameObject mob = Instantiate(mobPrefab, spawnPosition, Quaternion.LookRotation(Vector3.zero - spawnPosition));
            mob.transform.SetParent(mobSquad.transform);
            currentMobs.Add(mob);
        }
        waveIndex++;
        WaveTextBox.text = "Wave: " + waveIndex;
        currentWaveInitialized = true;
        startOfWave = Time.time;
    }
    private void HandleWaveLogic()
    {
        if (currentMobs.Count == 0)
        {
            currentWaveInitialized = false;
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
