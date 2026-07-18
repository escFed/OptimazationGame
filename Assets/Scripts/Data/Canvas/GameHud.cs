using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameHud
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI upgradesText;

    public Slider HealthSlider => healthSlider;
    public TextMeshProUGUI HealthText => healthText;
    public TextMeshProUGUI WaveText => waveText;
    public TextMeshProUGUI WaveTimerText => waveTimerText;
    public TextMeshProUGUI KillsText => killsText;
    public TextMeshProUGUI UpgradesText => upgradesText;
}
