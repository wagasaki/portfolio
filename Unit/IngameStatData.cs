using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 한 세션 진행용 스탯.
/// </summary>
[System.Serializable]
public class IngameStatData : ICloneable
{
    public int CurrentLevel { get; set; }
    public float CurrentEXP;
    public int CurrentGold { get; set; }

    public int FreeStatPoint { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Vitality { get; set; }
    public int Luck { get; set; }

    private float VitalEfficiency;
    private float StrEfficiency;
    private float AgiEfficiency;
    private float LuckEfficiency;
    private int StartVit;
    private int StartStr;
    private int StartAgi;
    private int StartLuck;
    private int StartGold;
    private float LifeDrainBase;//소수점. 0.04면 4%. 
    private float ManaRegenAdd;
    private float AccuracyBase;
    private float ResistBase;
    private float EvasionBase;
    private float CoolTimeBase;


    private const int baseHP = 200;
    private const int baseMinDmg = 30;
    private const int baseMaxDmg = 45;
    private const int baseCritChance = 20;
    private const int baseCritDmg = 130;
    //private const int baseAccuracy = 80;
    private const int baseStamina = 30;
    private const float ItemFindModifierBase = 100;
    private const float GoldModifierBase = 100;
    private const float ExpModifierBase = 100;
    private const float HealingModifier = 100;
    private const int BaseThreatModifier = 0;

    private float HP { get { return baseHP + Vitality * 10 * (1 + VitalEfficiency); } }
    private float Atk { get { return Strength * (1 + StrEfficiency); } }
    private float Def { get { return Agility * (1 + AgiEfficiency); } }
    private float Aspeed = 2;
    private float CritChance { get { return baseCritChance * Mathf.Clamp((Agility - 10) / (CurrentLevel * 2), 0.6f, 4f); } }//Mathf.Min(4, Mathf.Max(0.6f, ((Agility-10) / (CurrentLevel * 2)))); } } //크리확률 12~ 80%까지
    private float CritDmg { get { return baseCritDmg * Mathf.Clamp((Strength - 10) / (CurrentLevel * 2), 0.8f, 2f); } }//Mathf.Min(2, Mathf.Max(0.8f, ((Strength-10) / (CurrentLevel * 2)))); } }  // 크리 데미지 104% ~ 260%까지

    private float HPRegen;
    private float Mana;
    private float ManaRegen;

    private float DmgReduction;
    private float MinDmg { get { return CurrentLevel * 2 + baseMinDmg; } }
    private float MaxDmg { get {return CurrentLevel * 2 + baseMaxDmg; } }

    public int CurrentBaseStamina { get; set; }

    public int DayCount { get; set; }


    /// <summary>
    /// 로드 후 시작 시, 저장된 장비 목록을 불러와서 스탯을 적용시켜주기 때문에 세이브-로드 시 초기화해야함. 
    /// </summary>
    public Dictionary<eStatInfo, Dictionary<eStatCalcType, float>> AddictiveStatDic { get; set; }

    public Dictionary<eStatInfo, int> ItemOwnedStatDic { get; set; }

    /// <summary>
    /// 로드 후 시작 시, 저장된 장비 목록을 불러와서 스탯을 적용시켜주기 때문에 세이브-로드 시 초기화해야함. 
    /// </summary>
    public Dictionary<eSecondaryEffect, int> SecondaryEffectOnHit { get; set; }

    public Dictionary<string, List<bool>> IsMapClearedInfoDic { get; set; }

    public string CurrentMapName { get; set; }
    public int CurrentMapWayIndex { get; set; }
    public int EncounterCount { get; set; }
    //public int CurrentStamina { get; set; }
    public int Threat { get; set; }


    public void NewInagmeStatData()
    {
        Dictionary<string, SoulEnchantStat> souldic = InitController.Instance.SaveDatas.UserData.SoulEnchantStatDic;
        VitalEfficiency = souldic[Constants.Soul_VitEfficiency].CurrentValue;
        StrEfficiency = souldic[Constants.Soul_StrEfficiency].CurrentValue;
        AgiEfficiency = souldic[Constants.Soul_AgiEfficiency].CurrentValue;
        LuckEfficiency = souldic[Constants.Soul_LuckEfficiency].CurrentValue;

        StartVit = (int)souldic[Constants.Soul_StartVit].CurrentValue;
        StartStr = (int)souldic[Constants.Soul_StartStr].CurrentValue;
        StartAgi = (int)souldic[Constants.Soul_StartAgi].CurrentValue;
        StartLuck = (int)souldic[Constants.Soul_StartLuck].CurrentValue;
        StartGold = (int)souldic[Constants.Soul_PocketMoney].CurrentValue;
        LifeDrainBase = souldic[Constants.Soul_LifeSteal].CurrentValue * 100; // 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        ManaRegenAdd = souldic[Constants.Soul_Generate].CurrentValue * 100; // 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        AccuracyBase = souldic[Constants.Soul_Accuracy].CurrentValue * 100;// 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        ResistBase = souldic[Constants.Soul_Resist].CurrentValue * 100;// 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        EvasionBase = souldic[Constants.Soul_Evasion].CurrentValue * 100;// 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        CoolTimeBase = souldic[Constants.Soul_CoolTime].CurrentValue;// 소수점값임.. int캐스팅 ㄴㄴㄴㄴ
        //사실 이게 다 int float이 구분될 필요는 없는데.. 걍 수치를 int형으로 다 통일하고 그에 맞춰서 값을 걍 어느 정도 조정하면 되는데 하... 일단은 이렇게 계속 해보자
        Debug.Log($"{AccuracyBase}, {ResistBase}, {EvasionBase}, {CoolTimeBase}");
        CurrentLevel = 1;
        CurrentGold = 0 + StartGold;//TODO 테스트용
        CurrentEXP = 0;
        FreeStatPoint = 0;
        Vitality = 10 + StartVit;
        Strength = 10 + StartStr;
        Agility = 10 + StartAgi;
        Luck = 10 + StartLuck;




        Aspeed = 2;

        Mana = 100;
        ManaRegen = 1 + ManaRegenAdd;
        HPRegen = 0;

        DmgReduction = 0;
        CurrentBaseStamina = baseStamina;
        AddictiveStatDic = new Dictionary<eStatInfo, Dictionary<eStatCalcType, float>>();
        int count = InitController.Instance.GameDatas.StatInfoEnumCount;
        for(int i = 0; i< count; i++ )
        {
            AddictiveStatDic.Add((eStatInfo)i, new Dictionary<eStatCalcType, float>());
        }

        SecondaryEffectOnHit = new Dictionary<eSecondaryEffect, int>();

        IsMapClearedInfoDic = new Dictionary<string, List<bool>>();

        foreach(var a in InitController.Instance.GameDatas.MapWayPointDataEntityDic)
        {
            List<bool> cleared = new List<bool>();
            for(int i = 0; i < a.Value.Count;i++)
            {
                cleared.Add(false);
            }
            IsMapClearedInfoDic.Add(a.Key, cleared);
        }


        SetItemOwnedStat();

        CurrentMapName = Constants.FirstMap;
        CurrentMapWayIndex = 0;
        EncounterCount = 0;
        DayCount = 0;
    }
    /// <summary>
    /// 제네릭으로 바꾸기엔 내부 연산에서 float, int 혼용하는 경우가 있어서... 고민을 좀 해봐야할듯
    /// </summary>
    /// <param name="statInfo"></param>
    /// <returns></returns>
    public float GetTotalStat(eStatInfo statInfo)
    {
        //{ get { return 1 + Mathf.Sqrt(Luck / (float)CurrentLevel); } } // TODO공식은 차후 수정 어떻게 할지 고민 많이 필요할듯? 위에 공방체도 마찬가지
        float luck_base_stat = 1;
        if (statInfo == eStatInfo.Gold || statInfo == eStatInfo.ItemFind)
        {
            float luckmdified = Mathf.Max(1, Luck * (1 + LuckEfficiency));
            luck_base_stat = Mathf.Max(1, (Mathf.Sqrt(Mathf.Log(luckmdified, 5) * Mathf.Min(1, Mathf.Sqrt(luckmdified / CurrentLevel + 10)))) - 0.167f);
            //Debug.Log(luck_base_stat);
        }
        //float luck_base_stat = Mathf.Max(1, (Mathf.Sqrt(Mathf.Log(Luck, 5) * Mathf.Min(1, Mathf.Sqrt(Luck / CurrentLevel + 10)))) - 0.167f);

        float value = 0;
        float Add = 0;
        float MultiplyPercent = 100;
        float MultiplyPercentLowest = 10;
        float percent = 1f;
        switch (statInfo)
        {
            case eStatInfo.HP:
                value = HP;
                break;
            case eStatInfo.Atk:
                value = Atk;
                break;
            case eStatInfo.Def:
                value = Def;
                break;
            case eStatInfo.ASpeed:
                value = Aspeed;
                break;
            case eStatInfo.CritChance:
                value = CritChance;
                percent = 0.01f;
                break;
            case eStatInfo.CritDmg:
                value = CritDmg;
                percent = 0.01f;
                break;
            case eStatInfo.HPRegen:
                value = HPRegen;
                break;
            case eStatInfo.Mana:
                value = Mana;
                break;
            case eStatInfo.ManaRegen:
                value = ManaRegen;
                break;
            case eStatInfo.ItemFind:
                value = ItemFindModifierBase * luck_base_stat;
                percent = 0.01f;
                break;
            case eStatInfo.DmgReduction:
                value = DmgReduction;
                percent = 0.01f;
                break;
            case eStatInfo.MinDmg:
                value = MinDmg;
                break;
            case eStatInfo.MaxDmg:
                value = MaxDmg;
                break;
            case eStatInfo.Stamina:
                value = CurrentBaseStamina;
                break;
            case eStatInfo.Gold:
                value = GoldModifierBase * luck_base_stat;
                percent = 0.01f;
                break;
            case eStatInfo.Exp:
                value = ExpModifierBase;
                percent = 0.01f;
                break;
            case eStatInfo.HealingModifier:
                value = HealingModifier;
                percent = 0.01f;
                break;
            case eStatInfo.Threat:
                value = BaseThreatModifier;
                break;
            case eStatInfo.LifeDrain:
                value = LifeDrainBase;
                percent = 0.01f;
                break;
            case eStatInfo.Accuracy:
                value = AccuracyBase;
                percent = 0.01f;
                break;
            case eStatInfo.Resist:
                value = ResistBase;
                percent = 0.01f;
                break;
            case eStatInfo.Evasion:
                value = EvasionBase;
                percent = 0.01f;
                break;
            case eStatInfo.CoolTime:
                value = CoolTimeBase;
                percent = 0.01f;
                break;
            default:
                break;
        }
        foreach (var a in AddictiveStatDic[statInfo])
        {
            if (a.Key == eStatCalcType.AddInt || a.Key == eStatCalcType.AddPercent)
            {
                Add += a.Value;
            }
            else if(a.Key == eStatCalcType.MultiplyPercent)//MultiplyPercent -- 공속 같은 경우 이거 사용. 이 경우 multiplyint는 1인 상태
            {
                MultiplyPercent += a.Value;
            }
            else
            {
                Debug.LogWarning("error");
            }
        }

        Add += ItemOwnedStatDic[statInfo]; //보유 아이템 누적 개수로 인한 스탯증가. 이건 모두 합산으로만 처리할 예정. 

        float multiplywithinPercent = Mathf.Max(MultiplyPercent, MultiplyPercentLowest) * 0.01f;//곱해주는 경우 0.01 밑으로 떨어지지 않음. 공속같은경우, 2라면 아무리 빨라도 0.02 속 이상 불가

        //Debug.Log(string.Format("{2} ({0} + {1}) x {3} x{4}", value, Add, statInfo, multiplywithinPercent, percent));
        value = (value + Add) * multiplywithinPercent * percent;
        //TODO 크리티컬 데미지와 확률의 경우 여기서 또 clamp걸어줘야되는데....
        return value;
    }

    public void SetItemOwnedStat()
    {
        if (ItemOwnedStatDic != null)
            ItemOwnedStatDic = null;

        ItemOwnedStatDic = new Dictionary<eStatInfo, int>();

        int count = InitController.Instance.GameDatas.StatInfoEnumCount;
        for (int i = 0; i < count; i++)
        {
            ItemOwnedStatDic.Add((eStatInfo)i, 0);
        }
        foreach (var a in InitController.Instance.GameDatas.WeaponDatas.Weapon)
        {
            if (InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword].IsAcquired)
            {
                int value = Mathf.CeilToInt(a.HoldingValue * InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword].Count);

                ItemOwnedStatDic[a.HoldingStat] += value;
            }
        }
        foreach (var a in InitController.Instance.GameDatas.ArmorDatas.Armor)
        {
            if (InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword].IsAcquired)
            {
                int value = Mathf.CeilToInt(a.HoldingValue * InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword].Count);

                ItemOwnedStatDic[a.HoldingStat] += value;
            }
        }
        foreach (var a in InitController.Instance.GameDatas.AccessoryDatas.Accessory)
        {
            if (InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword].IsAcquired)
            {
                int value = Mathf.CeilToInt(a.HoldingValue * InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword].Count);

                ItemOwnedStatDic[a.HoldingStat] += value;
            }
        }
        foreach (var a in InitController.Instance.GameDatas.SkillStoneDatas.SkillStone)
        {
            if (InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword].IsAcquired)
            {
                int value = Mathf.CeilToInt(a.HoldingValue * InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword].Count);

                ItemOwnedStatDic[a.HoldingStat] += value;
            }
        }
        InitController.Instance.GamePlays.OnItemOwnedStatChanged?.Invoke();
    }
    /// <summary>
    /// 장비 착용하고 있을 시 스탯 중첩되는 경우 방지. 추가 스탯, 온힛부과효과 외에도 장비 장착 시 추가되는 부분은 여기서 초기화 필수
    /// </summary>
    public void ClearDataOnLoad()
    {
        foreach(var a in AddictiveStatDic)
        {
            a.Value.Clear();
        }
        SecondaryEffectOnHit.Clear();
        Debug.Log("Cleared [addictiveStat & secondaryeffectonhit] On Load");
    }

    /// <summary>
    /// for DeepCopy
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        #region regacy
        //IngameStatData data = new IngameStatData();
        //data.CurrentLevel = this.CurrentLevel;
        //data.FreeStatPoint = this.FreeStatPoint;
        //data.Strength = this.Strength;
        //data.Agility = this.Agility;
        //data.Vitality = this.Vitality;
        //data.Luck = this.Luck;

        //data.StatDistribitionRatio = this.StatDistribitionRatio;
        //data.MaxDistributionPoint = this.MaxDistributionPoint;
        //data.IsAutoDistribute = this.IsAutoDistribute;

        //data.Aspeed = this.Aspeed;
        //data.CritChance = this.CritChance;
        //data.CritDmg = this.CritDmg;
        //data.Accuracy = this.Accuracy;

        //data.Mana = this.Mana;
        //data.ManaRegen = this.ManaRegen;
        //data.DmgReduction = this.DmgReduction;
        //data.MaxDmgRange = this.MaxDmgRange;
        //data.MinDmgRange = this.MinDmgRange;

        #endregion
        //TODO 오호라.. 걍 타입캐스팅해줘도되는구나?
        IngameStatData data = MemberwiseClone() as IngameStatData;

        return data;
    }
}
