using System.Collections;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    [SerializeField]
    private bool isDontDestroyOnLoad = false;

    protected virtual void AwakeBehaviour() { }
    protected virtual void StartBehaviour() { }
    protected virtual void LateStartBehaviour() { }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        LateStartBehaviour();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            if (isDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        AwakeBehaviour();
    }

    private void Start()
    {
        StartBehaviour();
        StartCoroutine(LateStart());
    }

    protected virtual void OnDestroyBehaviour() { }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        OnDestroyBehaviour();
    }
}
