using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Components;

public class SkillStoneUIElem : EquipUIElem<SkillStoneDataEntity>
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
        DamageTypeText,
        CollectionText,
        CollectionCountText,
        HoldingStatText,
        HoldingStatValueText,
        SkillContentText,
        CostText,
    }
    enum Images
    {
        ItemImage,
    }
    public SkillStoneDataEntity StoneEntity { get; private set; }

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
    public override void InitElem(SkillStoneDataEntity dataEntity, bool isAcquired, ShopUIPopup shop)
    {
        _callShopUIPopup = shop;
        StoneEntity = dataEntity;
        _cost = StoneEntity.Cost;
        _maxHoldingCount = StoneEntity.MaxHoldingCount;
        _isBuyable = StoneEntity.IsBuyable;
        _holdingValue = StoneEntity.HoldingValue;


        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(StoneEntity.Keyword, out string[] names);
        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = names;
        GetText((int)Texts.NameText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
        InitController.Instance.GameDatas.SkillContentDataDic.TryGetValue(StoneEntity.SkillKeyword, out string[] content);

        string secondaryEffect = null;
        SkillDataEntity skilldata = StoneEntity.SkillDataEntity;
        switch (skilldata.SecondaryEffectKeyword)
        {
            case eSecondaryEffect.Burn:
                secondaryEffect = "<color=red>";
                break;
            case eSecondaryEffect.ElectricShock:
                secondaryEffect = "<color=yellow>";
                break;
            case eSecondaryEffect.FrostBite:
                secondaryEffect = "<color=blue>";
                break;
            case eSecondaryEffect.Poison:
                secondaryEffect = "<color=green>";
                break;
            case eSecondaryEffect.Bleeding:
                secondaryEffect = "<color=#7B009A>";
                break;
            case eSecondaryEffect.Shield:
                secondaryEffect = "<color=#5a5a5a>";
                break;
            case eSecondaryEffect.Enchant:
                break;
            case eSecondaryEffect.Reduction:
                break;
            case eSecondaryEffect.Cleansing:
                break;
            case eSecondaryEffect.Burst:
                break;
            case eSecondaryEffect.Test5:
                break;
            default:
                break;
        }

        switch (StoneEntity.DamageType)
        {
            case eDamageType.None:
                Debug.Log("No damage type");
                break;
            case eDamageType.Physics:
                GetText((int)Texts.DamageTypeText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Physics_Menu");
                break;
            case eDamageType.Fire:
                GetText((int)Texts.DamageTypeText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Fire_Menu");
                break;
            case eDamageType.Ice:
                GetText((int)Texts.DamageTypeText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Ice_Menu");
                break;
            case eDamageType.Electric:
                GetText((int)Texts.DamageTypeText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Electric_Menu");
                break;
            case eDamageType.Poison:
                GetText((int)Texts.DamageTypeText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Poison_Menu");
                break;
            default:
                break;
        }


        for (int i = 0; i < content.Length; i++)
        {
            content[i] = string.Format
                (content[i],
                string.Format("<color=yellow>{0}</color>", skilldata.HitCount),
                string.Format("<color=yellow>{0:P0}</color>", skilldata.Power),
                string.Format("<color=yellow>{0:P0}</color>", skilldata.TotalPower),
                string.Format("<color=yellow>{0:P0}</color>", skilldata.SecondaryEffectProc),
                secondaryEffect + InitController.Instance.GameDatas.LocalizationDataDic[skilldata.SecondaryEffectKeyword.ToString()][i] + "</color>",
                string.Format("<color=yellow>{0:P0}</color>", skilldata.SecondaryEffectValue)
                );
        }
        GetText((int)Texts.SkillContentText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = content;
        GetText((int)Texts.SkillContentText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");

        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue("Stat_" + StoneEntity.HoldingStat.ToString(), out string[] stats);
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = stats;
        GetText((int)Texts.HoldingStatText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");


        int ownedAmount = InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[StoneEntity.Keyword].Count;
        GetText((int)Texts.CollectionCountText).text = string.Format("{0}/{1}", ownedAmount, _maxHoldingCount);
        GetText((int)Texts.HoldingStatValueText).text = Mathf.Ceil(_holdingValue * ownedAmount).ToString();



        InitPanelObject(StoneEntity, isAcquired);

        GetImage((int)Images.ItemImage).sprite = InitController.Instance.GameDatas.AllItemSpriteDic[StoneEntity.Index.ToString()];//Resources.Load<Sprite>(Paths.SkillStoneSpriteData + "/" + dataEntity.Index);//InitController.Instance.GameDatas.SkillIconDic[dataEntity.SkillKeyword];//Resources.Load<Sprite>(Paths.SkillStoneSpriteData + "/" + dataEntity.Index);
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
        popup.InitUIPopup(eEquipType.SkillStone, 
            this,
            _nameText.text,
            GetText((int)Texts.SkillContentText).text,
            _callShopUIPopup);

    }

    protected override int SetItemDataOnEquipReturnCount()
    {
        //TODO 처음 눌러서 구입할 수 있을 때라서 count+1이 맥스홀딩 값을 넘을 일은 없긴함. 맥스홀딩은 현재 다 100이상이라
        int count = InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[StoneEntity.Keyword].Count + 1;
        InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[StoneEntity.Keyword] = new EquipAcquiredCount(true, count);

        return count;
    }
}