using System;
using UnityEngine;

[Serializable]
public class PlayerSetupData
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Terrain terrain;
    [SerializeField] private PlayerBaseStats stats;
    [SerializeField] private float spawnHeightOffset = 1f;

    public GameObject Prefab => prefab;
    public Terrain Terrain => terrain;
    public PlayerBaseStats Stats => stats;
    public float SpawnHeightOffset => spawnHeightOffset;
}
