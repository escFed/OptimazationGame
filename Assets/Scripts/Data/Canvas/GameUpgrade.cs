using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameUpgrade
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] titleTexts;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;

    public Button[] Buttons => buttons;
    public TextMeshProUGUI[] TitleTexts => titleTexts;
    public TextMeshProUGUI[] DescriptionTexts => descriptionTexts;
}
