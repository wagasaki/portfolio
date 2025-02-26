using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorDataEntity
{
    public int Index;
    public eEquipType Type;
    public string Keyword;
    public string Kor;
    public string Eng;
    public eGrade Grade;
    public eStatInfo BaseStat;
    public int BaseValue;
    public eStatInfo SecondStat;
    public int SecondStatValue;
    public eStatInfo HoldingStat;
    public eNumType HoldingStatType;
    public float HoldingValue;
    public HoldingValueCalcType HoldingValueCalcType;
    public int CurrentHoldingCount;
    public int MaxHoldingCount;
    public int SkillLevel;
    public bool IsBuyable;
    public int Cost;
}
