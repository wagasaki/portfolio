using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
public class ArmorUIElem : EquipUIElem<ArmorDataEntity>
{
    enum Buttons
    {
        BuyButton,
    }
    enum GameObjects
    {
        TouchObject,
        PanelObject,
        CostImage,
        EquippedTag
    }
    enum Texts
    {
        NameText,
        CollectionText,
        CollectionCountText,
        HoldingStatText,
        HoldingStatValueText,
        BaseStatText,
        BaseStatContentText,
        StatText,
        StatContentText,
        CostText,
    }
    enum Images
    {
        ItemImage,
    }
    public ArmorDataEntity ArmorEntity { get; private set; }

    private void Awake()
    {
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        { // HACK 공통 할당
            _nameText = GetText((int)Texts.NameText);
            _costText = GetText((int)Texts.CostText);
            _collectionCountText = GetText((int)Texts.CollectionCountText);
            _holdingStatValueText = GetText((int)Texts.HoldingStatValueText);
            _equippedTag = GetObject((int)GameObjects.EquippedTag);
            _panelObject = GetObject((int)GameObjects.PanelObject);
            _costImage = GetObject((int)GameObjects.CostImage);
            _buyButton = GetButton((int)Buttons.BuyButton);
        }
    }
    public override void InitElem(ArmorDataEntity dataEntity, bool isAcquired, ShopUIPopup shop)
    {
        _callShopUIPopup = shop;
        ArmorEntity = dataEntity;
        _cost = ArmorEntity.Cost;
        _maxHoldingCount = ArmorEntity.MaxHoldingCount;
        _isBuyable = ArmorEntity.IsBuyable;
        _holdingValue = ArmorEntity.HoldingValue;


        InitPanelObject(ArmorEntity, isAcquired);


        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(ArmorEntity.Keyword, out string[] names);
        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue("Stat_" + ArmorEntity.HoldingStat.ToString(), out string[] stats);

        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = names;
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = stats;
        
        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");

        //GetText((int)Texts.StatContentText).text = string.Format(" + <color=green>{0} (+{1:P0})</color>", dataEntity.BaseValue, dataEntity.AddictiveStatValue);
        GetText((int)Texts.BaseStatContentText).text = string.Format("+ <color=#c0c0c0>{0}</color>", ArmorEntity.BaseValue);
        GetText((int)Texts.StatContentText).text = string.Format("+ <color=#c0c0c0>{0}</color>", ArmorEntity.SecondStatValue);
       


        int ownedAmount = InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[ArmorEntity.Keyword].Count;
        GetText((int)Texts.CollectionCountText).text = string.Format("{0}/{1}", ownedAmount, _maxHoldingCount);
        GetText((int)Texts.HoldingStatValueText).text = Mathf.Ceil(_holdingValue * ownedAmount).ToString();


        GetImage((int)Images.ItemImage).sprite = Resources.Load<Sprite>(Paths.ArmorSpriteData + "/" + ArmorEntity.Index);
        //TODO 현재 상태 데이터 불러와서 데이터 입력. 



        //터치인식 후 장착 등의 내용을 가진 팝업 
        GetObject((int)GameObjects.TouchObject).GetComponent<UIEventHandler>().OnClickHandler += () => ShowEquipPopup();


        InitController.Instance.GamePlays.OnGoldUseOrGain -= (int gold) => ColorCostText(gold);
        InitController.Instance.GamePlays.OnGoldUseOrGain += (int gold) => ColorCostText(gold);
        ColorCostText(InitController.Instance.GamePlays.IngameStat.CurrentGold);
    }
    protected override void ShowEquipPopup()
    {
        base.ShowEquipPopup();
        ShopEquipUIPopup popup = InitController.Instance.UIs.OpenUIPopup<ShopEquipUIPopup>(null, InitController.Instance.UIs.transform);
        popup.InitUIPopup(eEquipType.Armor, 
            this,
            _nameText.text,
            GetText((int)Texts.BaseStatText).text + "   " + GetText((int)Texts.BaseStatContentText).text + "\n" +
            GetText((int)Texts.StatText).text + "   " + GetText((int)Texts.StatContentText).text,
            _callShopUIPopup);
    }
    protected override int SetItemDataOnEquipReturnCount()
    {
        //TODO 처음 눌러서 구입할 수 있을 때라서 count+1이 맥스홀딩 값을 넘을 일은 없긴함. 맥스홀딩은 현재 다 100이상이라
        int count = InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[ArmorEntity.Keyword].Count + 1;
        InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[ArmorEntity.Keyword] = new EquipAcquiredCount(true, count);

        return count;
    }
}
