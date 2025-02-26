using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAssigner_Stat : AbilityAssigner
{
    public override void Excute(eAbilities ability, float value)
    {
        CurrentAbility = ability;
        switch (ability)
        {
            //case eAbilities.None:
            //    break;
            case eAbilities.Titanic:
                _unit.CurrentBattleStat.ModifyStat(eStatInfo.HP, ability.ToString(), value*0.01f);
                break;
            case eAbilities.Destruction:
                _unit.CurrentBattleStat.ModifyStat(eStatInfo.Atk, ability.ToString(), value * 0.01f);
                break;
            case eAbilities.IronWall:
                _unit.CurrentBattleStat.ModifyStat(eStatInfo.Def, ability.ToString(), value * 0.01f);
                break;
            case eAbilities.Swift:
                _unit.CurrentBattleStat.ModifyStat(eStatInfo.ASpeed, ability.ToString(), value * 0.01f);
                break;
            case eAbilities.Invincible:
                _unit.CurrentBattleStat.ModifyStat(eStatInfo.DmgReduction, ability.ToString(), value * 0.01f);
                break;
            //case eAbilities.Stat_HPRegen:
            //case eAbilities.Stat_MPRegen:
                //break;
            default:
                //현재 None, Stat_HPRegen, Stat_MpRegen은 계획 X
                break;
        }
    }
}
