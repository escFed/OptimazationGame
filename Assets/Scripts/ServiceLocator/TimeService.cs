using UnityEngine;

public class TimeService 
{
    public float DeltaTime => Time.deltaTime * TimeScale;
    public float UnscaledDeltaTime => Time.unscaledDeltaTime;
    public float TimeScale { get; private set; } = 1f;

    public void SetTimeScale(float value)
    {
        TimeScale = Mathf.Max(0f, value);
    }
}
