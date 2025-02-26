using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WeaponDataEntity
{
    public int Index;
    public eEquipType Type;
    public string Keyword;
    public string Kor;
    public string Eng;
    public eGrade Grade;
    public eStatInfo BaseStat;
    public int MinDmg;
    public int MaxDmg;
    public int BaseValue;//Average(min, max)
    public int AspeedModifier;
    public eStatInfo SecondStat;
    public float SecondStatValue;
    public eStatInfo HoldingStat;
    public eNumType HoldingStatType;
    public float HoldingValue;
    public HoldingValueCalcType HoldingValueCalcType;
    public int CurrentHoldingCount;
    public int MaxHoldingCount;
    public bool IsBuyable;
    public int Cost;
    public eWeaponSize WeaponSize;
}
