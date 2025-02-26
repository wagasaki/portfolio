using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDistributionElem : UIBase
{
    public int Index { get; set; }
    public Action OnStatChanged;


    
    enum Buttons
    {
        PlusButton,
        MinusButton,
    }
    enum Texts
    {
        RatioText,
    }

    public void InitUIElem(StatUIPopup popup, int index)
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Index = index;


        int[] distribute = new int[]
        {
            PlayerPrefs.GetInt(Constants.VitDistribute, 0),
            PlayerPrefs.GetInt(Constants.StrDistribute, 0),
            PlayerPrefs.GetInt(Constants.AgiDistribute, 0),
            PlayerPrefs.GetInt(Constants.LucDistribute, 0)
        };


        GetButton((int)Buttons.PlusButton).onClick.AddListener(() => 
        { 
            GetText((int)Texts.RatioText).text = popup.UseDistributionPoint(true, this).ToString();
            InitController.Instance.Sounds.PlaySFX(eSFX.Click_Light);
        });
        GetButton((int)Buttons.MinusButton).onClick.AddListener(() => 
        { 
            GetText((int)Texts.RatioText).text = popup.UseDistributionPoint(false, this).ToString();
            InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
        });
        GetText((int)Texts.RatioText).text = distribute[index].ToString();
    }
}
