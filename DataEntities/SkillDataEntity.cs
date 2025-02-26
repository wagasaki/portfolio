using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillDataEntity
{
    public int Index;
    public string Keyword;
    public eSkillActionType SkillActionType;
    public eDamageType DamageType;
    public int HitCount;
    public float Power;
    public float TotalPower;
    public int Cost;
    public float CoolTime;
    public eRelativeTarget SkillTarget;
    public eSecondaryType SecondaryType;
    public eSecondaryEffect SecondaryEffectKeyword;
    public eRelativeTarget SecondaryEffectTarget;
    public float SecondaryEffectProc;
    public float SecondaryEffectValue;
    public int SecondaryEffectStackCount;
    public string Sound;
}
