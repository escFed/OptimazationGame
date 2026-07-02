using System.Collections.Generic;
using UnityEngine;

public abstract class PatternData : ScriptableObject
{
    [SerializeField] private float baseAttackInterval = 1f;

    public float BaseAttackInterval => baseAttackInterval;
    public abstract void GetDirections(Transform owner, List<Vector3> results);
}
