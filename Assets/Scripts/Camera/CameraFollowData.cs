using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Follow Data")]
public class CameraFollowData : ScriptableObject
{
    [SerializeField] private Vector3 offset = new (0f, 18f, -12f);
    [SerializeField] private Vector3 lookAtOffset = new (0f, 0.75f, 0f);
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private bool lookAtTarget = true;

    public Vector3 Offset => offset;
    public Vector3 LookAtOffset => lookAtOffset;
    public float SmoothTime => smoothTime;
    public bool UseSmoothing => useSmoothing;
    public bool LookAtTarget => lookAtTarget;
}
