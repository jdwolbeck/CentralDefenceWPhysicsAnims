using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct Wave
{
    public int NumMobs;
    public float Intensity;
}
public class WaveHandler : MonoBehaviour
{
    public static WaveHandler Instance { get; private set; }
    public GameObject MobFolder;
    public TMP_Text WaveTextBox;

    [SerializeField] private List<Wave> waves;
    [SerializeField] private List<GameObject> currentMobs;

    private GameObject mobPrefab;
    private GameObject squadPrefab;
    private SquadController mobSquad;
    private int waveIndex;
    private bool currentWaveInitialized;
    private float startOfWave;

    private const float SPAWN_RADIUS = 50f;
    private const float WAVE_COUNT = 10000;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        waves = new List<Wave>();
        currentMobs = new List<GameObject>();
        mobPrefab = Resources.Load("Prefabs/Mob") as GameObject;
        squadPrefab = Resources.Load("Prefabs/Squad") as GameObject;
        currentWaveInitialized = false;

        for (int waveIndex = 15; waveIndex < WAVE_COUNT; waveIndex++)
        {
            Wave wave = new Wave();
            wave.NumMobs = waveIndex;
            wave.Intensity = waveIndex;
            waves.Add(wave);
        }
    }
    private void Update()
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

        // Spawn all of the mobs and store them in our currentMobs list
        for (int i = 0; i < waves[waveIndex].NumMobs; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(Random.Range(-SPAWN_RADIUS, SPAWN_RADIUS), Random.Range(0, 2));
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
            currentWaveInitialized = false;
    }
    private Vector3 GetSpawnPosition(float spawnXPosition, int spawnUpOrDown)
    {
        // Circle Equation: (X - H)^2 + (Y - K)^2 = r^2  ** H & K are both 0
        Vector2 spawnPoint = new Vector2(spawnXPosition, 0);

        if (spawnUpOrDown == 1)
            spawnPoint.y = Mathf.Sqrt(SPAWN_RADIUS*SPAWN_RADIUS - spawnXPosition*spawnXPosition); // Positive direction requested
        else
            spawnPoint.y = -Mathf.Sqrt(SPAWN_RADIUS * SPAWN_RADIUS - spawnXPosition * spawnXPosition); // Negative direction requested

        return new Vector3(spawnPoint.x, 0, spawnPoint.y);
    }
}
