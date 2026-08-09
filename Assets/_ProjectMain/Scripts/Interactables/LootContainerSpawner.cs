using System.Collections.Generic;
using UnityEngine;

public class LootContainerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int containersPerDay = 5;

    private List<GameObject> activeContainers = new();

    private void Start()
    {
        if(DayNightManager.Instance != null)
        {
            DayNightManager.Instance.OnDayStart += SpawnContainers;
            DayNightManager.Instance.OnNightStart += ClearContainers;
        }
    }

    private void OnDisable()
    {
        if (DayNightManager.Instance != null)
        {
            DayNightManager.Instance.OnDayStart -= SpawnContainers;
            DayNightManager.Instance.OnNightStart -= ClearContainers;
        }
    }

    private void SpawnContainers(int day)
    {
        int count = Mathf.Min(containersPerDay * day, spawnPoints.Length);
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[index];
            availablePoints.RemoveAt(index);

            GameObject containerGO = ObjectPoolManager.Get(containerPrefab, point.position, Quaternion.identity);
            activeContainers.Add(containerGO);
        }
    }

    private void ClearContainers(int day)
    {
        foreach (GameObject containerGO in activeContainers)
        {
            if (containerGO == null) continue;
            containerGO.GetComponent<LootContainer>().ForceDespawn();
        }
        activeContainers.Clear();
    }
}
