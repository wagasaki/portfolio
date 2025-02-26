using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateInfo : UIBase
{
    enum Images
    {
        StateDurationFill,
        StateImage,
    }
    enum Texts
    {
        StateDurationText,
    }
    private Image _stateFill;
    private TextMeshProUGUI _durationText;
    private void Awake()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        _stateFill = GetImage((int)Images.StateDurationFill);
        _durationText = GetText((int)Texts.StateDurationText);
    }
    internal void InitStateInfo(TimedDebuff debuff)
    {
        GetImage((int)Images.StateImage).sprite = InitController.Instance.GameDatas.StateIconDic[debuff.Debuff.Name];
        if(debuff.Debuff.IsDebuff)
        {
            _stateFill.color = Color.red;
        }
        else//debuff아닌경우(버프라던가)
        {
            _stateFill.color = Color.green;
        }
    }
    public void RefreshStateInfo(TimedDebuff debuff)
    {
        _stateFill.fillAmount = debuff.GetCurrentDuration/debuff.Debuff.Duration;
        _durationText.text = debuff.GetEffectStatckCount.ToString();
    }
}
