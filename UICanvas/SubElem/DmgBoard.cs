using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DmgBoard : UIBase
{
    enum GameObjects
    {
        BoardContent
    }

    private Transform _boardContentTrans;
    private List<TextMeshProUGUI> _texts;
    private int _currentIndex = 0;
    private void Awake()
    {
        BindObject(typeof(GameObjects));

        _boardContentTrans = GetObject((int)GameObjects.BoardContent).GetComponent<Transform>();
        _texts = new List<TextMeshProUGUI>();
        for (int i = 0; i< _boardContentTrans.childCount;i++)
        {
            _texts.Add(_boardContentTrans.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }
    private void OnEnable()
    {
        foreach(var a in _texts)
        {
            a.gameObject.SetActive(false);
        }
    }

    public void AddText(eAbsoluteTarget target, string keyword, eDamageType dmgType, string finaldmg)
    {
        _currentIndex = _currentIndex % _boardContentTrans.childCount;
        _texts[_currentIndex].gameObject.SetActive(true);
        _texts[_currentIndex].transform.SetAsLastSibling();

        int count = InitController.Instance.GameDatas.LanguageEnumCount;
        string[] boardText = new string[count];

        if (target == eAbsoluteTarget.Player)
        {
            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue("DmgBoard_AttackedText", out boardText);
        }
        else if (target == eAbsoluteTarget.Enemy)
        {
            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue("DmgBoard_AttackText", out boardText);
        }
        string[] boardText2 = new string[boardText.Length];
        for(int i = 0; i< boardText2.Length;i++)
        {
            boardText2[i] = boardText[i];
        }
        
        if (InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(keyword, out string[] names))
        {
            for(int i = 0; i< names.Length;i++)
            {
                boardText2[i] = string.Format(boardText2[i],names[i], finaldmg);
            }
            _texts[_currentIndex].GetComponent<LocalizeStringEvent>().StringReference.Arguments = boardText2;
            _texts[_currentIndex].GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
            _texts[_currentIndex].GetComponent<LocalizeStringEvent>().StringReference.RefreshString();
        }
        else
        {
            Debug.LogWarning("wrong keyword : " + keyword);
        }


        Color textcolor = Color.white ;
        switch (dmgType)
        {
            case eDamageType.None:
                Debug.LogWarning("None");
                break;
            case eDamageType.Physics:
                if (keyword.Equals("NormalAttack"))
                    textcolor = Color.white;
                else
                    ColorUtility.TryParseHtmlString(Constants.PhysicsColor, out textcolor);
                break;
            case eDamageType.Fire:
                textcolor = Color.red;
                break;
            case eDamageType.Ice:
                textcolor = Color.blue;
                break;
            case eDamageType.Electric:
                textcolor = Color.yellow;
                break;
            case eDamageType.Poison:
                textcolor = Color.green;
                break;
            default:
                break;
        }
        _texts[_currentIndex].color = textcolor;
        _currentIndex++;
    }
}
