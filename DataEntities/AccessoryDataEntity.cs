using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AccessoryDataEntity
{
    public int Index;
    public bool IsNewType;
    public eEquipType Type;
    public string Keyword;
    public string Kor;
    public string Eng;
    public eStatInfo BaseStat;
    public eGrade Grade;
    public int Level;
    public int BaseValue;
    public bool IsBaseValuePercent;
    public eStatAddType ValueAddType;
    public eStatInfo HoldingStat;
    public eNumType HoldingStatType;
    public float HoldingValue;
    public HoldingValueCalcType HoldingValueCalcType;
    public int CurrentHoldingCount;
    public int MaxHoldingCount;
    public eSecondaryEffect SecondaryEffect;
    public int SecondaryEffectLevel;
    public string SecondaryEffectUsingKeyword;
    public bool IsBuyable;
    public int Cost;
}
