using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField, TextArea] private string description;

    public string Title => title;
    public string Description => description;

    public abstract void Apply(UpgradeContext context);

}
