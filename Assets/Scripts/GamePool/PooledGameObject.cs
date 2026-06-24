using UnityEngine;

public class PooledGameObject : IPoolable
{
    public PooledGameObject(GameObject instance)
    {
        Instance = instance;
        Instance.SetActive(false);
    }

    public GameObject Instance { get; }
    public bool IsActive => Instance != null && Instance.activeSelf;

    public void OnSpawn()
    {
        Instance.SetActive(true);
    }

    public void OnDespawn()
    {
        Instance.SetActive(false);
    }
}
