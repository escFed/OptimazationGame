using UnityEngine;

public class TransformView : MonoBehaviour
{
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
    }
}
