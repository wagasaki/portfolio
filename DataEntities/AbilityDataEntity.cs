using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityDataEntity
{
    public int Index;
    public string Key;
    public eAbilityType AbilityType;
    public bool IsProbability;
    public int BaseValue;
    public int MaxValue;
    public int CurrentLevel;
    public int MaxLevel;
}
