using System.Collections;
using UnityEngine;


public class ZombieSpawner : MonoBehaviour
{

    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int zombiesPerNight = 7;

    private void Start()
    {
        DayNightManager.Instance.OnNightStart += SpawnWave;
    }

    private void OnDisable()
    {
        if(DayNightManager.Instance != null)
            DayNightManager.Instance.OnNightStart -= SpawnWave;
    }

    private void SpawnWave(int currentDay)
    {
        for (int i = 0; i < zombiesPerNight * currentDay; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            ObjectPoolManager.Get(zombiePrefab, spawn.position, Quaternion.identity);
        }
    }
}