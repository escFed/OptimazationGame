using System.Collections.Generic;
using UnityEngine;

public abstract class PatternData : ScriptableObject
{
    public abstract void GetDirections(Transform owner, List<Vector3> results);

}
