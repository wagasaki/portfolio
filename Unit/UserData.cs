using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EquipAcquiredCount
{
    public bool IsAcquired;
    public int Count;

    public EquipAcquiredCount(bool isAcquired, int count)
    {
        IsAcquired = isAcquired;
        Count = count;
    }
}
[Serializable]
public class SoulEnchantStat
{
    public int CurrentLevel;
    public float CurrentValue;
}
[Serializable]
public class UserData
{
    public int Index = 0;
    public long PlayTime = 0;

    //public int BaseAtk;
    //public int BaseDef;
    //public int BaseHP;
    //public int BaseLuck;
    //public int BaseASpeed;

    public Dictionary<string, EquipAcquiredCount> IsWeaponAcuiredDic { get; private set; }//TODO 차라리 무기 데이터에서 bool값 하나 넣어주는게 나을지도?라고 생각을 했는데 추후 케릭이 늘어난다거나 하는 경우는 이게맞음
    public Dictionary<string, EquipAcquiredCount> IsArmorAcquiredDic { get; private set; }
    public Dictionary<string, EquipAcquiredCount> IsAccessoryAcquiredDic { get; private set; }
    public Dictionary<string, EquipAcquiredCount> IsSkillStoneAcquiredDic { get; private set; }
    public Dictionary<string, SoulEnchantStat> SoulEnchantStatDic { get; private set; }

    public IngameStatData IngameStatData { get; set; }
    public int CurrentSoul { get; set; }
    public PresetData[] GetPresetDatas { get; private set; }
    public int CurrentPrestIndex { get; set; }
    public bool[] AutoCastEnabled { get; set; }
    public int SpeedModifier { get; set; }
    private bool _isAdsHide;
    public bool GetIsAdsHide { 
        get 
        { return _isAdsHide; }
        set 
        {
            if(_isAdsHide == false)
            {
                _isAdsHide = value;
            }
            else // 이미 true, 즉 광고제거 구매한경우
            {
                _isAdsHide = true;
            }
        } 
    }

    public bool IsTutorialCleared { get; set; }
    /// <summary>
    /// new data만들때만 사용.
    /// </summary>
    /// <param name="maxlevel"></param>
    /// <param name="maxexp"></param>
    /// <param name="weaponData"></param>
    /// <param name="armorData"></param>
    /// <param name="acceData"></param>
    /// <param name="stoneData"></param>
    public void NewUserData()
    {
        GameDataController gamedatas = InitController.Instance.GameDatas;

        Index = 0;
        PlayTime = 0;
        SpeedModifier = 1;
        CurrentSoul = 0;
        CurrentPrestIndex = 0;
        GetIsAdsHide = false;

        IsWeaponAcuiredDic = new Dictionary<string, EquipAcquiredCount>(gamedatas.WeaponDatas.Weapon.Count);
        foreach(WeaponDataEntity a in gamedatas.WeaponDatas.Weapon)
        {
            EquipAcquiredCount acquiredCount = new EquipAcquiredCount(false, 0);
            IsWeaponAcuiredDic.Add(a.Keyword, acquiredCount);
        }
        IsArmorAcquiredDic = new Dictionary<string, EquipAcquiredCount>(gamedatas.ArmorDatas.Armor.Count);
        foreach(ArmorDataEntity a in gamedatas.ArmorDatas.Armor)
        {
            EquipAcquiredCount acquiredCount = new EquipAcquiredCount(false, 0);
            IsArmorAcquiredDic.Add(a.Keyword, acquiredCount);
        }
        IsAccessoryAcquiredDic = new Dictionary<string, EquipAcquiredCount>(gamedatas.AccessoryDatas.Accessory.Count);
        foreach(AccessoryDataEntity a in gamedatas.AccessoryDatas.Accessory)
        {
            EquipAcquiredCount acquiredCount = new EquipAcquiredCount(false, 0);
            IsAccessoryAcquiredDic.Add(a.Keyword, acquiredCount);
        }
        IsSkillStoneAcquiredDic = new Dictionary<string, EquipAcquiredCount>(gamedatas.SkillStoneDatas.SkillStone.Count);
        foreach(SkillStoneDataEntity a in gamedatas.SkillStoneDatas.SkillStone)
        {
            EquipAcquiredCount acquiredCount = new EquipAcquiredCount(false, 0);
            IsSkillStoneAcquiredDic.Add(a.Keyword, acquiredCount);
        }
        GetPresetDatas = new PresetData[Nums.MaxPresetCount];
        for(int i = 0; i< GetPresetDatas.Length;i++)
        {
            GetPresetDatas[i] = new PresetData();
            if (i == 0)
                GetPresetDatas[i].IsPresetDataOpened = true;
            else
                GetPresetDatas[i].IsPresetDataOpened = false;

        }
        AutoCastEnabled = new bool[Nums.SkillStoneCount];
        for(int i = 0; i< AutoCastEnabled.Length;i++)
        {
            AutoCastEnabled[i] = false;
        }

        SoulEnchantStatDic = new Dictionary<string, SoulEnchantStat>();
        foreach(var a in gamedatas.SoulEnchantDataDic)
        {
            SoulEnchantStat stat = new SoulEnchantStat();
            stat.CurrentLevel = 0;
            stat.CurrentValue = 0;
            SoulEnchantStatDic.Add(a.Key, stat);
        }

        IsTutorialCleared = false;

        IngameStatData = null;
    }

}
