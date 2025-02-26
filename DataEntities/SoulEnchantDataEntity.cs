using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoulEnchantDataEntity
{
    public int Index;
    public string Key;
    public int MaxLevel;
    public float MainValue;
    public float ValuePerLevel;
    public byte ValueCipher; // ÀÚ¸´¼ö
    public bool IsPercent;
    public eSoulCalcType CalcType;
    public int CalcB;
    public int CurrentLevel;
    public float CurrentValue;
    public float CurrentCost;
}

public enum eSoulCalcType
{
    Linear,
    LinearXLinear,
}