using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapons/Patterns/Straight")]
public class StraightPatternData : PatternData
{
    public override void GetDirections(Transform owner, List<Vector3> results)
    {
        var direction = owner.forward;
        direction.y = 0f;

        results.Add(direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward);
    }
}
