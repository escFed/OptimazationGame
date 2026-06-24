using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Weapons/Patterns/Cone")]
public class ConePatternData : PatternData
{
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float angle = 60f;

    public int ProjectileCount => projectileCount;
    public float Angle => angle;

    public override void GetDirections(Transform owner, List<Vector3> results)
    {
        var forward = GetFlatDirection(owner.forward);

        if (projectileCount == 1)
        {
            results.Add(forward);
            return;
        }

        var startAngle = -angle * 0.5f;
        var angleStep = angle / (projectileCount - 1);

        for (var i = 0; i < projectileCount; i++)
        {
            var rotation = Quaternion.AngleAxis(startAngle + angleStep * i, Vector3.up);
            results.Add(rotation * forward);
        }
    }

    private static Vector3 GetFlatDirection(Vector3 direction)
    {
        direction.y = 0f;
        return direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
    }
}
