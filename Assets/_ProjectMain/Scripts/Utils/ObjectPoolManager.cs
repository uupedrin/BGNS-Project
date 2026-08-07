using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    private static Dictionary<int, PooledObjectInfo> _pools = new();
    private static GameObject _poolRoot;

    protected override void AwakeBehaviour()
    {
        CreatePoolRoot();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        _pools.Clear();
        CreatePoolRoot();
    }

    private void CreatePoolRoot()
    {
        _poolRoot = new GameObject("Pool Objects");
    }

    public static void CreatePool(GameObject prefab, int size)
    {
        int id = prefab.GetInstanceID();
        if (_pools.ContainsKey(id)) return;

        var pool = new PooledObjectInfo(prefab.name);
        _pools[id] = pool;

        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(prefab, _poolRoot.transform);
            obj.SetActive(false);
            pool.Inactive.Add(obj);
        }
    }

    #region PUBLIC_API_GET
    public static GameObject Get(GameObject prefab, bool setActive = true) => Get(prefab, Vector3.zero, Quaternion.identity, null, setActive);
    public static GameObject Get(GameObject prefab, Transform parent, bool setActive = true) => Get(prefab, Vector3.zero, Quaternion.identity, parent, setActive);
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, bool setActive = true) => Get(prefab, position, rotation, null, setActive);
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool setActive = true)
    {
        int id = prefab.GetInstanceID();

        if(!_pools.TryGetValue(id, out var pool))
        {
            pool = new PooledObjectInfo(prefab.name);
            _pools[id] = pool;
        }

        GameObject obj;
        
        if(pool.Inactive.Count > 0)
        {
            obj = pool.Inactive[^1];
            pool.Inactive.RemoveAt(pool.Inactive.Count - 1);
        }
        else
        {
            obj = Instantiate(prefab, _poolRoot.transform);
            obj.name = prefab.name;

            var pooled = obj.AddComponent<PooledObjectReference>();
            pooled.PrefabID = id;
        }

        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(setActive);

        return obj;
    }

    public static T Get<T>(T prefab, bool setActive = true) where T : Component => Get(prefab, Vector3.zero, Quaternion.identity, null, setActive);
    public static T Get<T>(T prefab, Transform parent, bool setActive = true) where T : Component => Get(prefab, Vector3.zero, Quaternion.identity, parent, setActive);
    public static T Get<T>(T prefab, Vector3 position, Quaternion rotation, bool setActive = true) where T : Component => Get(prefab, position, rotation, null, setActive);
    public static T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool setActive = true) where T:Component
    {
        GameObject obj = Get(prefab.gameObject, position, rotation, parent, setActive);
        return obj.GetComponent<T>();
    }
    #endregion

    #region PUBLIC_API_RETURN
    public static void Return(GameObject obj)
    {
        if (obj == null) return;

        var pooled = obj.GetComponent<PooledObjectReference>();

        if(pooled == null)
        {
            Destroy(obj);
            return;
        }

        if(!_pools.TryGetValue(pooled.PrefabID, out var pool) )
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(_poolRoot.transform);
        pool.Inactive.Add(obj);
    }

    public static void Return(GameObject obj, float delay)
    {
        Instance.StartCoroutine(ReturnDelayed(obj, delay));
    }

    private static IEnumerator ReturnDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Return(obj);
    }
    #endregion

}

public class PooledObjectInfo
{
    public string Name { get; private set; }
    public List<GameObject> Inactive { get; } = new();

    public PooledObjectInfo(string name)
    {
        Name = name;
    }
}

internal class PooledObjectReference : MonoBehaviour
{
    public int PrefabID { get; set; }
}