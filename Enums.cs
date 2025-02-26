using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eReward
{
    None = -1,
    Gold,
    Exp,
    Weapon,
    Armor,
    Accessory,
    SkillStone,
}
public enum eState
{
    CoolTime,
    Usable,
}
public enum eEnemyGrade
{
    None = -1,
    Normal,
    Boss,
}
public enum ePlayerState
{
    NotReadyForBattle = -1,
    Idle,
    Attack,
    Skill,
}
public enum eUIState
{
    Battle,
    Map,
}
public enum eStat
{
    Vitality,
    Strength,
    Agility,
    Luck,
}
public enum eStatInfo
{
    None = -1,
    HP,
    Dmg,
    Atk,
    Def,
    ASpeed,
    CritChance,
    CritDmg,
    HPRegen,
    Mana,
    ManaRegen,
    ItemFind,
    DmgReduction,
    MinDmg,
    MaxDmg,
    Stamina,
    Gold,
    Exp,
    HealingModifier,
    Threat,
    LifeDrain,
    Accuracy,
    Resist,
    Evasion,
    CoolTime,
}
public enum eStatInfoForInfoElem
{
    HP = 0,
    Dmg = 1,
    Atk = 2,
    Def = 3,
    ASpeed = 4,
    Mana = 8,
    ManaRegen = 9,
    DmgReduction = 11,
    Gold = 15,
    Exp = 16,
}
public enum eStatAddType
{
    None = -1,
    Add,
    Multiply
}
public enum eStatCalcType
{
    None = -1,
    AddInt,
    AddPercent,
    //MultiplyInt,
    MultiplyPercent,
}
public enum eAbilities
{
    None = 0,

    Titanic = 1000,
    Destruction,
    IronWall,
    Swift,
    Invincible,

    Phoenix = 2000, //업데이트에 이벤트로 붙이는 퍼센트 체력 회복
    Sage,

    Magma = 3000,
    Electrocute,
    Blizard,
    Death,
    Hemophilia,
    LifeDrain,
    SoulBreaker,

}
public enum eAbilityType
{
    None= 0,
    Stat,
    OnHitEnd,
    OnUpdate,
}
public enum eNumType
{
    Constant,
    Percent,
}
public enum HoldingValueCalcType
{
    LOG10,
    ADD,
}

public enum eEquipType
{
    Weapon,
    Armor,
    Accessory,
    SkillStone,
}

public enum eGrade
{
    Normal,
    Unique,
}
public enum eRelativeTarget
{
    None = -1,
    Self,
    Opponent,
}
public enum eAbsoluteTarget
{
    None= -1,
    Player,
    Enemy,
}
public enum eSecondaryType
{
    None = -1,
    Trigger,
    If,
}

public enum eSFX
{
    Slash,
    Hit,
    Click, // 일반 루트 ui 
    Walk,
    Click_Success, // 버튼 상호작용 성공
    Click_Failed, // 실패
    Click_Light, // elem 등의 작은 유아이
    Click_Enterance, //클릭후 입장
    Deny,// 거절/ 거부
    AcquireItem, //아이템획득
}
public enum eSound
{
    None = -1,
    BGM,
    Back,
    SFX,
}

public enum eTriggerType
{
    Default,
    NormalAttack,
    SkillAttack,
    DebuffTrigger,
}

public enum eDamageType
{
    None= -1,
    Physics,
    Fire,
    Ice,
    Electric,
    Poison,
}
public enum eBattleMap
{
    DraconicTemple,
    Forest,
    GraveYard,
    Mine,
    MistyForest,
    MistyLoad,
    RedForest,
    RuinedTemple,
    IcyCave,
}

public enum eBackSound
{
    None = -1,
    CaveSound,
    ForestSound,
    PlaneSound,
    SnowStormSound,
}

public enum eWeaponSize
{
    None = -1,
    Normal,
    Small,
    Large,
    XLarge,
}

public enum eMapEffect
{
    None, //Default
    HPDecrease, // *0.5, 0.75 (3,2턴)
    MoneyIncrease, // * 2,3,4 (5,3,2턴 최대치)
    EXPIncrease, // * 2,3,4  (5,3,2턴 최대치)
    DropIncrease, // * 2,3,4(5,3,2턴)
    Labyrinth, // 1단계 2단계 3단계 랜덤. 적 강함 랜덤
}

