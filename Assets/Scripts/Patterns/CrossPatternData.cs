using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu (menuName ="Weapons/Patterns/Cross")]
public class CrossPatternData : PatternData
{
    public override void GetDirections(Transform owner, List<Vector3> results)
    {
        var forward = GetFlatDirection(owner.forward);
        var right = GetFlatDirection(owner.right);

        results.Add(forward);
        results.Add(-forward);
        results.Add(right);
        results.Add(-right);
    }

    private static Vector3 GetFlatDirection(Vector3 direction)
    {
        direction.y = 0f;
        return direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
    }
}
