using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
public class AccessoryUIElem : EquipUIElem<AccessoryDataEntity>
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
        StatContentText,
        CostText,
    }
    enum Images
    {
        ItemImage,
    }
    public AccessoryDataEntity AccessoryEntity { get; private set; }

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
    public override void InitElem(AccessoryDataEntity dataEntity, bool isAcquired, ShopUIPopup shop)
    {
        _callShopUIPopup = shop;
        AccessoryEntity = dataEntity;
        _cost = AccessoryEntity.Cost;
        _maxHoldingCount = AccessoryEntity.MaxHoldingCount;
        _isBuyable = AccessoryEntity.IsBuyable;
        _holdingValue = AccessoryEntity.HoldingValue;


        InitPanelObject(AccessoryEntity, isAcquired);

        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(AccessoryEntity.Keyword, out string[] names);
        //InitController.Instance.GameDatas.HoldingStatDataDic.TryGetValue("Stat_"+dataEntity.HoldingStat.ToString(), out string[] stats);
        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue("Stat_" + AccessoryEntity.HoldingStat.ToString(), out string[] stats);
        if (AccessoryEntity.BaseStat != eStatInfo.None)
        {
            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(AccessoryEntity.BaseStat.ToString(), out string[] baseStats);
            string[] BaseStats = new string[baseStats.Length];
            float basevalue = AccessoryEntity.BaseValue;
            if (AccessoryEntity.IsBaseValuePercent) basevalue *= 0.01f;
            for(int i = 0;i< BaseStats.Length;i++)
            {
                BaseStats[i] = string.Format(baseStats[i], basevalue);
            }
            GetText((int)Texts.StatContentText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = BaseStats;
            GetText((int)Texts.StatContentText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
        }
        else
        {
            if(AccessoryEntity.SecondaryEffect != eSecondaryEffect.None) // 스킬 입력이 되어있는경우
            {
                //Debug.Log(dataEntity.SecondaryEffect);
                InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(AccessoryEntity.SecondaryEffectUsingKeyword, out string[] secondaryEffect);
                string[] SecondaryEffects = new string[secondaryEffect.Length];
                int secondaryEffectLevel = AccessoryEntity.SecondaryEffectLevel;
                //Debug.Log(secondaryEffectLevel);
                for (int i = 0; i < SecondaryEffects.Length; i++)
                {
                    SecondaryEffects[i] = string.Format(secondaryEffect[i], secondaryEffectLevel);
                }
                GetText((int)Texts.StatContentText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = SecondaryEffects;
                GetText((int)Texts.StatContentText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
            }
        }

        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = names;
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = stats;
        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");



        int ownedAmount = InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[AccessoryEntity.Keyword].Count;
        GetText((int)Texts.CollectionCountText).text = string.Format("{0}/{1}", ownedAmount, _maxHoldingCount);
        GetText((int)Texts.HoldingStatValueText).text = Mathf.Ceil(_holdingValue* ownedAmount).ToString();


        GetImage((int)Images.ItemImage).sprite = Resources.Load<Sprite>(Paths.AccessorySpriteData + "/" + AccessoryEntity.Index);
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
        popup.InitUIPopup(eEquipType.Accessory, 
            this,
            _nameText.text,
            GetText((int)Texts.StatContentText).text,
            _callShopUIPopup);
    }

    protected override int SetItemDataOnEquipReturnCount()
    {
        //TODO 처음 눌러서 구입할 수 있을 때라서 count+1이 맥스홀딩 값을 넘을 일은 없긴함. 맥스홀딩은 현재 다 100이상이라
        int count = InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[AccessoryEntity.Keyword].Count + 1;
        InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[AccessoryEntity.Keyword] = new EquipAcquiredCount(true, count);

        return count;
    }
}
