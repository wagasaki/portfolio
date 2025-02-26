using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticElem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText, _valueText;
    public void InitElem(string name, string value)
    {
        _nameText.text = name;
        _valueText.text = value;
    }
}
