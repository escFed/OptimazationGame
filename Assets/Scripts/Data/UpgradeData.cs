using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField, TextArea] private string description;
    [SerializeField] private bool canRepeat;

    public string Title => title;
    public string Description => description;
    public bool CanRepeat => canRepeat;

    public abstract void Apply(UpgradeContext context);

}
