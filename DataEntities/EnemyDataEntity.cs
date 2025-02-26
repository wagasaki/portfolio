using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyDataEntity
{
    public int Index;
    public string Keyword;
    public string Name_Kor;
    public string Name_Eng;
    public string EnemyType;
    public string EnemyGrade;
    public int HP;
    public int MinDmg;
    public int MaxDmg;
    public int DmgModifier;
    public int Atk;
    public int Def;

    public float ASpeed;
    public string Skill;
    public int StrengthRatio;
    public int AgilityRatio;
    public int VitalityRatio;
    public int LuckRatio;

    public float CritChance;
    public float CritDmg;
    public float HealingModifier = 100;

    public eAbilities Abil_0;
    public int Abil_0_Level;
    public eAbilities Abil_1;
    public int Abil_1_Level;
    public eAbilities Abil_2;
    public int Abil_2_Level;


    public EnemyDataEntity Clone()
    {
        EnemyDataEntity entity = MemberwiseClone() as EnemyDataEntity; ;
        //HACK 참조 형식 있으면 여기서 초기화 및 할당
        return entity;
    }

}
