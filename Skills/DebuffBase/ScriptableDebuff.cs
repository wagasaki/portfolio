using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//public enum eDebuffTrigger
//{
//    StatValue,
//    BoolValue,
//    GetHit,
//    DoHit,
//    Tic,
//}
//public enum eDebuffDecreaseTrigger
//{
//    GetHit,
//    DoHit,
//    GetHeal,
//    Tic,

//}
//public enum eDebuffTriggerType
//{
//    Time,
//    Action,
//    Bool,
//}

public abstract class ScriptableDebuff : ScriptableObject
{
    public string Name;
    public eSecondaryEffect SecondaryEffectType;
    public eDamageType DamageType;
    public bool IsDebuff;
    public float Duration;
    public float MainValue;
    [Header("트리거 횟수")]
    public int TriggerCount;
    [Header("스택 당 효과 증가")]
    public float StackCoef;
    public bool IsDurationStacked;
    public bool IsEffectStacked;
    public int EffectMaxStack;

    public abstract TimedDebuff InitializeDebuff(GameObject target);
}
