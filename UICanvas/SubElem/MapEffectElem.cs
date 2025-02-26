using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class MapEffectElem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _infoText;
    [SerializeField]
    private Image _symbol;
    public int Duration = 0;
    public float Value = 0;

    private LocalizeStringEvent _infoEv;
    private void Awake()
    {
        _infoEv = _infoText.GetComponent<LocalizeStringEvent>();
    }
    public void InitElem(MapEffect effect)
    {
        Duration = effect.Duration;
        Value = effect.EffectValue;
        switch (effect.EffectType)
        {
            case eMapEffect.None:
                break;
            case eMapEffect.HPDecrease:
                _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_DecreaseHP);
                break;
            case eMapEffect.MoneyIncrease:
                _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseGold);
                break;
            case eMapEffect.EXPIncrease:
                _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseExp);
                break;
            case eMapEffect.DropIncrease:
                _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseFind);
                break;
            case eMapEffect.Labyrinth:
                break;
            default:
                break;
        }
        _infoEv.RefreshString();

        float prefWidth = 200;
        float prefHeight = 100;
        //if (_infoText.preferredWidth > 330) prefWidth = _infoText.preferredWidth;
        //else prefWidth = 330;

        _infoText.rectTransform.sizeDelta = new Vector2(prefWidth, prefHeight);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(_symbol.rectTransform.sizeDelta.x + prefWidth, _symbol.rectTransform.sizeDelta.y);
    }
}
