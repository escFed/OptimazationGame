using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapons/Patterns/Radial")]
public class RadialPatternData : PatternData
{
    [SerializeField] private int projectileCount = 8;

    public int ProjectileCount => projectileCount;

    public override void GetDirections(Transform owner, List<Vector3> results)
    {
        var baseDirection = GetFlatDirection(owner.forward);
        var angleStep = 360f / projectileCount;

        for (var i = 0; i < projectileCount; i++)
        {
            var rotation = Quaternion.AngleAxis(angleStep * i, Vector3.up);
            results.Add(rotation * baseDirection);
        }
    }

    private static Vector3 GetFlatDirection(Vector3 direction)
    {
        direction.y = 0f;
        return direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
    }
}
