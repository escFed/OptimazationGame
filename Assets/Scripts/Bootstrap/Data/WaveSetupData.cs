using System;
using UnityEngine;

[Serializable]
public class WaveSetupData
{
    [SerializeField] private WaveData[] waves;

    public WaveData[] Waves => waves;
}
