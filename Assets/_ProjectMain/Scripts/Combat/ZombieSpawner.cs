using System.Collections;
using UnityEngine;


public class ZombieSpawner : MonoBehaviour
{

    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int zombiesPerNight = 7;

    private int zombiesAlive = 0;

    private void Start()
    {
        DayNightManager.Instance.OnNightStart += SpawnWave;
        Zombie.OnZombieRemoved += HandleZombieRemoved;
    }

    private void OnDisable()
    {
        if(DayNightManager.Instance != null)
            DayNightManager.Instance.OnNightStart -= SpawnWave;
        Zombie.OnZombieRemoved -= HandleZombieRemoved;
    }

    private void SpawnWave(int currentDay)
    {
        for (int i = 0; i < zombiesPerNight * currentDay; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            ObjectPoolManager.Get(zombiePrefab, spawn.position, Quaternion.identity);
            zombiesAlive++;
        }
    }

    private void HandleZombieRemoved()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0) DayNightManager.Instance.EndNightEarly();
    }
}