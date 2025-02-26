using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillStoneDataEntity
{
    public int Index;
    public bool IsNewType;
    public eEquipType Type;
    public string Keyword;
    public string Kor;
    public string Eng;

    public eDamageType DamageType;

    public int Level;

    public int CurrentHoldingCount;
    public int MaxHoldingCount;
    public eStatInfo HoldingStat;
    public float HoldingValue;

    public string SkillKeyword;
    public int SkillLevel;
    public bool IsBuyable;
    public int Cost;




    public SkillDataEntity SkillDataEntity;


}
