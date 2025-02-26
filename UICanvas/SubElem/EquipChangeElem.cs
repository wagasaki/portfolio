using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.U2D;

public class EquipChangeElem : UIBase, IPointerClickHandler
{

    private eEquipType _equipType;
    private int _index;
    string[] names = new string[2];
    private Color _emptyColor = new Color(0.2f, 0.2f, 0.2f, 1);
    enum Texts
    {
        EquipNameText,
    }
    enum Images
    {
        EquipImage,
    }
    public void InitUiElem(eEquipType equipType, int index)
    {

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        _index = index;
        _equipType = equipType;

        RefreshUI();
    }

    public void RefreshUI()
    {
        //int currentPresetNum = InitController.Instance.GamePlays.IngameStat.CurrentActivatedPresetNum;
        int currentPresetNum = InitController.Instance.SaveDatas.UserData.CurrentPrestIndex;
        TextMeshProUGUI targettext = GetText((int)Texts.EquipNameText);
        LocalizeStringEvent stringEvent = targettext.GetComponent<LocalizeStringEvent>();

        switch (_equipType)
        {
            case eEquipType.Weapon:
                WeaponDataEntity weapon = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentWeapon;
                if (weapon != null)
                {
                    targettext.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(weapon.Keyword, out names);
                    stringEvent.StringReference.Arguments = names;
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    GetImage((int)Images.EquipImage).sprite = InitController.Instance.GameDatas.AllItemSpriteDic[weapon.Index.ToString()];//Resources.Load<Sprite>(Paths.WeaponSpriteData + "/" + weapon.Index);
                }
                else
                {
                    targettext.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    GetImage((int)Images.EquipImage).sprite = null;
                }

                break;
            case eEquipType.Armor:
                ArmorDataEntity armor = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentArmor;
                if (armor != null)
                {
                    targettext.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(armor.Keyword, out names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    GetImage((int)Images.EquipImage).sprite = InitController.Instance.GameDatas.AllItemSpriteDic[armor.Index.ToString()]; //Resources.Load<Sprite>(Paths.ArmorSpriteData + "/" + armor.Index);
                }
                else
                {
                    targettext.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    GetImage((int)Images.EquipImage).sprite = null;
                }
                break;
            case eEquipType.Accessory:
                AccessoryDataEntity accessory = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentAccessory[_index];
                if (accessory != null)
                {
                    targettext.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(accessory.Keyword, out names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    GetImage((int)Images.EquipImage).sprite = InitController.Instance.GameDatas.AllItemSpriteDic[accessory.Index.ToString()]; //Resources.Load<Sprite>(Paths.AccessorySpriteData + "/" + accessory.Index);
                    //GetImage((int)Images.EquipImage).sprite = Resources.Load<SpriteAtlas>("RPG/EquipmentData/AccessorySpriteData/Accessory").GetSprite(accessory.Index.ToString());
                }
                else
                {
                    targettext.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    GetImage((int)Images.EquipImage).sprite = null;
                }
                break;
            case eEquipType.SkillStone:
                SkillStoneDataEntity skillStone = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentSkillStone[_index];
                if (skillStone != null)
                {
                    targettext.color = Color.white;
                    InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(skillStone.Keyword, out names);
                    stringEvent.StringReference.Arguments = new[] { names[0], names[1] };
                    stringEvent.StringReference.SetReference("Equip", "Default");
                    GetImage((int)Images.EquipImage).sprite = InitController.Instance.GameDatas.AllItemSpriteDic[skillStone.Index.ToString()]; //Resources.Load<Sprite>(Paths.SkillStoneSpriteData + "/" + skillStone.SkillKeyword);
                }
                else
                {
                    targettext.color = _emptyColor;
                    stringEvent.StringReference.SetReference("Equip", "Menu_Empty");
                    GetImage((int)Images.EquipImage).sprite = null;
                }
                break;
            default:
                break;
        }
        stringEvent.StringReference.RefreshString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenShopPopup(_equipType);
        InitController.Instance.Sounds.PlaySFX(eSFX.Click_Light);
    }

    public void OpenShopPopup(eEquipType equipType)
    {
        Debug.Log("팝업");
        int currentPresetNum = InitController.Instance.SaveDatas.UserData.CurrentPrestIndex;
        int itemindex = -1;
        switch (_equipType)
        {
            case eEquipType.Weapon:
                WeaponDataEntity weapon = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentWeapon;
                if (weapon == null) itemindex = 0;
                else
                    itemindex = weapon.Index;
                break;
            case eEquipType.Armor:
                ArmorDataEntity armor = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentArmor;
                if (armor == null) itemindex = 0;
                else
                    itemindex = armor.Index;
                break;
            case eEquipType.Accessory:
                AccessoryDataEntity accessory = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentAccessory[_index];
                if (accessory == null) itemindex = 0;
                else
                    itemindex = accessory.Index;
                break;
            case eEquipType.SkillStone:
                SkillStoneDataEntity skillStone = InitController.Instance.SaveDatas.UserData.GetPresetDatas[currentPresetNum]._currentSkillStone[_index];
                if (skillStone == null) itemindex = 0;
                else
                    itemindex = skillStone.Index;
                break;
        }

        ShopUIPopup popup = InitController.Instance.UIs.OpenUIPopup<ShopUIPopup>(null, InitController.Instance.UIs.transform);
        popup.InitUIPopup();
        popup.ShowItemTap(equipType, itemindex);
    }
}
