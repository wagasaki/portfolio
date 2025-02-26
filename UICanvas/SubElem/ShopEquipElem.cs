using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class ShopEquipElem : UIBase
{
    private eEquipType _equipType;
    private int _index;
    private object _uiElem;
    private Image _itemImage;
    private Image _equipImage;
    private Color _emptyColor = new Color(0.2f, 0.2f, 0.2f, 1);
    private ShopUIPopup _callShopPopup;
    enum Texts
    {
        EquipNameText,
        EquipStatText,
    }

    enum Images
    {
        EquipImage,
        ItemImage,
        UnEquipImage
    }

    private void Awake()
    {
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        System.Action onclick = OnClickEquip;
        BindEvent(this.gameObject, onclick, UIEvent.Click);
    }
    public void InitUIElem(eEquipType equiptype, int index, object uielem, ShopUIPopup shop)
    {
        _callShopPopup = shop;
        _equipType = equiptype;
        _index = index;
        _uiElem = uielem;

        System.Action onUnEquip = () => OnClickUnEquip();
        BindEvent(GetImage((int)Images.UnEquipImage).gameObject, onUnEquip, UIEvent.Click);

        _itemImage = GetImage((int)Images.ItemImage);
        _equipImage = GetImage((int)Images.EquipImage);



        Task task = RefreshUIAsync();
        
    }
    public async Task RefreshUIAsync()
    {
        //int currentPresetNum = InitController.Instance.GamePlays.IngameStat.CurrentActivatedPresetNum;
        int currentPresetNum = InitController.Instance.SaveDatas.UserData.CurrentPrestIndex;
        TextMeshProUGUI targetText = GetText((int)Texts.EquipNameText);
        LocalizeStringEvent stringEvent = targetText.GetComponent<LocalizeStringEvent>();
        switch (_equipType)
        {
            case eEquipType.Weapon:
                WeaponDataEntity weapon = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentWeapon;
                if (weapon != null)
                {
                    targetText.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(weapon.Keyword, out string[] names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    _equipImage.gameObject.SetActive(true);
                    _itemImage.sprite = InitController.Instance.GameDatas.AllItemSpriteDic[weapon.Index.ToString()];//Resources.Load<Sprite>(Paths.WeaponSpriteData + "/" + weapon.Index);

                    var entryResult = await LocalizationSettings.StringDatabase.GetTableEntryAsync("Equip", "Menu_Dmg").Task;
                    var value = entryResult.Entry.Value;
                    string dm= string.Format("{0} {1}~{2}",value, weapon.MinDmg.ToString(), weapon.MaxDmg.ToString());
                    entryResult = await LocalizationSettings.StringDatabase.GetTableEntryAsync("Equip", "Menu_Atk").Task;
                    value = entryResult.Entry.Value;
                    string atk = string.Format("{0} +{1}", value, weapon.SecondStatValue);
                    GetText((int)Texts.EquipStatText).text = string.Format("{0}\n{1}", dm, atk);
                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(true);
                }
                else
                {
                    targetText.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    _equipImage.gameObject.SetActive(false);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(false);
                }
                break;
            case eEquipType.Armor:
                ArmorDataEntity armor = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentArmor;
                if (armor != null)
                {
                    targetText.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(armor.Keyword, out string[] names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    _equipImage.gameObject.SetActive(true);
                    _itemImage.sprite = InitController.Instance.GameDatas.AllItemSpriteDic[armor.Index.ToString()];//Resources.Load<Sprite>(Paths.ArmorSpriteData + "/" + armor.Index);


                    var entryResult = await LocalizationSettings.StringDatabase.GetTableEntryAsync("Equip", "Menu_Def").Task;
                    var value = entryResult.Entry.Value;
                    string def = string.Format("{0} +{1}", value, armor.BaseValue.ToString());
                    entryResult = await LocalizationSettings.StringDatabase.GetTableEntryAsync("Equip", "Menu_HP").Task;
                    value = entryResult.Entry.Value;
                    string hp = string.Format("{0} +{1}", value, armor.SecondStatValue);
                    GetText((int)Texts.EquipStatText).text = string.Format("{0}\n{1}", def, hp);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(true);
                }
                else
                {
                    targetText.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    _equipImage.gameObject.SetActive(false);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(false);
                }
                break;
            case eEquipType.Accessory:
                AccessoryDataEntity accessory = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentAccessory[_index];
                if (accessory != null)
                {
                    targetText.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(accessory.Keyword, out string[] names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    _equipImage.gameObject.SetActive(true);
                    _itemImage.sprite = InitController.Instance.GameDatas.AllItemSpriteDic[accessory.Index.ToString()];//Resources.Load<Sprite>(Paths.AccessorySpriteData + "/" + accessory.Index);



                    { //Accessoryuielem에 있는 그대로
                        if (accessory.BaseStat != eStatInfo.None)
                        {
                            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(accessory.BaseStat.ToString(), out string[] baseStats);
                            string[] BaseStats = new string[baseStats.Length];
                            float basevalue = accessory.BaseValue;
                            if (accessory.IsBaseValuePercent) basevalue *= 0.01f;
                            for (int i = 0; i < BaseStats.Length; i++)
                            {
                                BaseStats[i] = string.Format(baseStats[i], basevalue);
                            }
                            GetText((int)Texts.EquipStatText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = BaseStats;
                            GetText((int)Texts.EquipStatText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
                        }
                        else
                        {
                            if (accessory.SecondaryEffect != eSecondaryEffect.None) // 스킬 입력이 되어있는경우
                            {
                                //Debug.Log(dataEntity.SecondaryEffect);
                                InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(accessory.SecondaryEffectUsingKeyword, out string[] secondaryEffect);
                                string[] SecondaryEffects = new string[secondaryEffect.Length];
                                int secondaryEffectLevel = accessory.SecondaryEffectLevel;
                                //Debug.Log(secondaryEffectLevel);
                                for (int i = 0; i < SecondaryEffects.Length; i++)
                                {
                                    SecondaryEffects[i] = string.Format(secondaryEffect[i], secondaryEffectLevel);
                                }
                                GetText((int)Texts.EquipStatText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = SecondaryEffects;
                                GetText((int)Texts.EquipStatText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
                            }
                        }
                    }



                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(true);
                }
                else
                {
                    targetText.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    _equipImage.gameObject.SetActive(false);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(false);
                }
                break;
            case eEquipType.SkillStone:
                SkillStoneDataEntity skillStone = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentSkillStone[_index];
                if (skillStone != null)
                {
                    targetText.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(skillStone.Keyword, out string[] names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    _equipImage.gameObject.SetActive(true);
                    _itemImage.sprite = InitController.Instance.GameDatas.AllItemSpriteDic[skillStone.Index.ToString()];//Resources.Load<Sprite>(Paths.SkillStoneSpriteData + "/" + skillStone.SkillKeyword);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(true);
                }
                else
                {
                    targetText.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    _equipImage.gameObject.SetActive(false);

                    GetImage((int)Images.UnEquipImage).gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
    public void OnClickEquip()
    {
        //TODO 해당 장비 장착
        UserData data = InitController.Instance.SaveDatas.UserData;

        //TODO 악세의 경우 동일 아이템을 여러개 착용할 수 있을지?결정해야함. 스킬스톤은 그러면 안되고... 아 겁나 복잡해졌네.. 걍 한개씩만 끼도록 할까
        switch (_equipType)
        {
            case eEquipType.Weapon:
                WeaponDataEntity weaponData = (_uiElem as WeaponUIElem).WeaponEntity;
                WeaponDataEntity currentWeapon = data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon;
                
                if (currentWeapon != null)
                {
                    if (currentWeapon.Index == weaponData.Index)
                    {
                        CallCantEquipTextEffect();
                        break;
                    }
                    _callShopPopup.WeaponUIElemDic[currentWeapon.Index].Equip(false);
                    InitController.Instance.GamePlays.RemoveStatOnUnEquipWeapon(currentWeapon);
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon = null;
                }
                data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon = weaponData;
                _callShopPopup.WeaponUIElemDic[weaponData.Index].Equip(true);
                InitController.Instance.GamePlays.ApplyStatOnWeaponEquip(weaponData);
                break;
            case eEquipType.Armor:
                ArmorDataEntity armorData = (_uiElem as ArmorUIElem).ArmorEntity;
                ArmorDataEntity currentArmor = data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor;
                if(currentArmor !=null)
                {
                    if (currentArmor.Index == armorData.Index)
                    {
                        CallCantEquipTextEffect();
                        break;
                    }
                    _callShopPopup.ArmorUIElemDic[currentArmor.Index].Equip(false);
                    InitController.Instance.GamePlays.RemoveStatOnUnEquipArmor(currentArmor.BaseValue, currentArmor.SecondStatValue);
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor = null;
                }
                data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor = armorData;
                _callShopPopup.ArmorUIElemDic[armorData.Index].Equip(true);
                InitController.Instance.GamePlays.ApplyStatOnArmorEquip(armorData.BaseValue, armorData.SecondStatValue);
                break;
            case eEquipType.Accessory:
                AccessoryDataEntity accessoryData = (_uiElem as AccessoryUIElem).AccessoryEntity;
                AccessoryDataEntity[] currentAccessory = data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory;
                if (currentAccessory[_index]?.Index == accessoryData.Index)
                {
                    CallCantEquipTextEffect();
                    break; 
                }
                for (int i = 0; i < currentAccessory.Length; i++)
                {
                    if (currentAccessory[i] == null) continue;
                    if (_index != i && currentAccessory[i].Index == accessoryData.Index)
                    {
                        CallCantEquipTextEffect();
                        _callShopPopup.AccessoryUIElemDic[currentAccessory[i].Index].Equip(false);
                        InitController.Instance.GamePlays.RemoveAccessoryStatOnUnEquip(currentAccessory[i]);
                        currentAccessory[i] = null;
                    }
                }
                if (currentAccessory[_index] != null)
                {
                    _callShopPopup.AccessoryUIElemDic[currentAccessory[_index].Index].Equip(false);
                    InitController.Instance.GamePlays.RemoveAccessoryStatOnUnEquip(currentAccessory[_index]);
                    currentAccessory[_index] = null;
                }

                currentAccessory[_index] = accessoryData;
                _callShopPopup.AccessoryUIElemDic[currentAccessory[_index].Index].Equip(true);
                InitController.Instance.GamePlays.ApplyAccessoryStatOnEquip(currentAccessory[_index]);
                #region regacy
                /*
                for (int i = 0; i< data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory.Length;i ++)
                {
                    if (data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[i] == accessoryData)
                    {
                        Debug.Log("동일한거 이미 있음");
                        ApplyStatOnEquip(
                            data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[i].BaseStat, 
                            data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[i].BaseValue, 
                            false);
                        data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[i] = null;
                    }
                }
                if(data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index]!=null)
                {
                    ApplyStatOnEquip(
                        data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index].BaseStat,
                        data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index].BaseValue,
                        false);
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index] = null;
                }

                data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index] = accessoryData;
                ApplyStatOnEquip(
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index].BaseStat,
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory[_index].BaseValue,
                    true);
                */
                #endregion
                break;
            case eEquipType.SkillStone:
                SkillStoneDataEntity skillstoneData = (_uiElem as SkillStoneUIElem).StoneEntity;
                SkillStoneDataEntity[] currentSkillStoneDatas= data.GetPresetDatas[data.CurrentPrestIndex]._currentSkillStone;
                if (currentSkillStoneDatas[_index]?.Index == skillstoneData.Index)
                {
                    CallCantEquipTextEffect();
                    break;
                }
                for (int i = 0; i < currentSkillStoneDatas.Length; i++)
                {
                    if (currentSkillStoneDatas[i] == null) continue;
                    if (_index != i && currentSkillStoneDatas[i].Index == skillstoneData.Index)
                    {
                        CallCantEquipTextEffect();
                        _callShopPopup.SkillStoneUIElemDic[currentSkillStoneDatas[i].Index].Equip(false);
                        currentSkillStoneDatas[i] = null;
                    }
                }
                if (currentSkillStoneDatas[_index] != null)
                {
                    _callShopPopup.SkillStoneUIElemDic[currentSkillStoneDatas[_index].Index].Equip(false);
                    currentSkillStoneDatas[_index] = null;
                }

                currentSkillStoneDatas[_index] = skillstoneData;
                _callShopPopup.SkillStoneUIElemDic[currentSkillStoneDatas[_index].Index].Equip(true);

                break;
            default:
                break;
        }
        //TODO UI refresh 및 스탯 적용
        InitController.Instance.UIs.GetUIPopup("EquipUIPopup")?.GetComponent<EquipUIPopup>()?.RefreshUI(_equipType, _index);
        InitController.Instance.UIs.CloseUIPopup();
        InitController.Instance.Sounds.PlaySFX(eSFX.Click_Light);
        void CallCantEquipTextEffect()
        {
            TextEffect effect = _callShopPopup.InfoTextEffectPool.GetFromPool(0, _callShopPopup.transform);
            effect.EffectText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "SameItemWarning");
            effect.gameObject.SetActive(true);
            effect.InitWithLocalizeText(Vector2.zero);
        }
    }

    public void OnClickUnEquip()
    {
        UserData data = InitController.Instance.SaveDatas.UserData;
        switch (_equipType)
        {
            case eEquipType.Weapon:
                WeaponDataEntity currentWeapon = data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon;
                if (currentWeapon != null)
                {
                    InitController.Instance.GamePlays.RemoveStatOnUnEquipWeapon(currentWeapon);
                    _callShopPopup.WeaponUIElemDic[currentWeapon.Index].Equip(false);
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon = null;
                }
                break;
            case eEquipType.Armor:
                ArmorDataEntity currentArmor = data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor;
                if (currentArmor != null)
                {
                    InitController.Instance.GamePlays.RemoveStatOnUnEquipArmor(currentArmor.BaseValue, currentArmor.SecondStatValue);
                    _callShopPopup.ArmorUIElemDic[currentArmor.Index].Equip(false);
                    data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor = null;
                }
                break;
            case eEquipType.Accessory:
                AccessoryDataEntity[] currentAccessory = data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory;
                if (currentAccessory[_index] != null)
                {
                    InitController.Instance.GamePlays.RemoveAccessoryStatOnUnEquip(currentAccessory[_index]);
                    _callShopPopup.AccessoryUIElemDic[currentAccessory[_index].Index].Equip(false);
                    currentAccessory[_index] = null;
                }
                break;
            case eEquipType.SkillStone:
                SkillStoneDataEntity[] currentSkillStoneDatas = data.GetPresetDatas[data.CurrentPrestIndex]._currentSkillStone;
                if (currentSkillStoneDatas[_index] != null)
                {
                    _callShopPopup.SkillStoneUIElemDic[currentSkillStoneDatas[_index].Index].Equip(false);
                    currentSkillStoneDatas[_index] = null;
                }
                break;
            default:
                break;
        }
        InitController.Instance.UIs.GetUIPopup("EquipUIPopup")?.GetComponent<EquipUIPopup>()?.RefreshUI(_equipType, _index);
        InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
        TextMeshProUGUI targetText = GetText((int)Texts.EquipNameText);
        LocalizeStringEvent stringEvent = targetText.GetComponent<LocalizeStringEvent>();
        targetText.color = Color.gray;
        stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
        _equipImage.gameObject.SetActive(false);
        GetImage((int)Images.UnEquipImage).gameObject.SetActive(false);
    }


    //TODO 지금 그냥 main이라고 집어넣어놨는데 차후 분리를 해도 괜찮고. 인덱스로 넣어도 괜찮고. 무기, 갑옷은 0, 장신구는 0~5. 어짜피 스탯은 다 합산해서 결정하니까.
    
}
