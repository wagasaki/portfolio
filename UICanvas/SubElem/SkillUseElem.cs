using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUseElem : UIBase, IPointerClickHandler
{
    private int _index;
    private SkillDataEntity _data;
    private Image _fillImage, _frameImage;
    public eState CurrentState { get; set; }
    private enum Images
    {
        SkillImage,
        FillImage,
        frameImage,
    }
    private enum Texts
    {
        ManaText,
    }
    private enum GameObjects
    {
        Panel,
    }
    
    public void InitUIElem(int index)
    {
        _index = index;
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));
        _fillImage = GetImage((int)Images.FillImage);
        _frameImage = GetImage((int)Images.frameImage);
        RefreshUIElem();

    }
    public void RefreshUIElem()
    {
        _data = InitController.Instance.SaveDatas.UserData.GetPresetDatas[InitController.Instance.SaveDatas.UserData.CurrentPrestIndex]?._currentSkillStone[_index]?.SkillDataEntity;
        if (_data == null)
        {
            GetObject((int)GameObjects.Panel).SetActive(false);
            GetImage((int)Images.SkillImage).sprite = InitController.Instance.GameDatas.SkillIconDic["EmptyIcon"];
            _frameImage.gameObject.SetActive(false);
        }
        else
        {
            GetObject((int)GameObjects.Panel).SetActive(true);
            GetText((int)Texts.ManaText).text = _data.Cost.ToString("N0");

            GetImage((int)Images.SkillImage).sprite = InitController.Instance.GameDatas.SkillIconDic[_data.Keyword];
            _frameImage.gameObject.SetActive(true);
        }
        RefreshCoolTime(0);
    }
    public void RefreshCoolTime(float coolTime)
    {
        _fillImage.fillAmount = coolTime;
        if (coolTime <= 0)
            _frameImage.gameObject.SetActive(true);
    }
    public void SkillNotUsable()
    {
        GetImage((int)Images.frameImage).gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        InitController.Instance.GamePlays.GetSkillController.PlayerUseSkill(_index);
    }
}
