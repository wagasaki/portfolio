using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAssigner_OnHitEnd : AbilityAssigner
{
    public override void Excute(eAbilities ability, float currentValue)
    {
        CurrentAbility = ability;
        switch (ability)
        {
            case eAbilities.LifeDrain: // 데미지 비례 흡혈  1~10% 까지
                void Vampire(eTriggerType type, float finalDmg)
                {
                    if(type== eTriggerType.NormalAttack)
                    {
                        //Debug.Log(finalDmg);
                        float healvalue = Mathf.Max(1, finalDmg * currentValue * 0.01f);
                        //Debug.Log("healvalue : " + healvalue);
                        _unit.GetHeal(healvalue, eNumType.Constant);
                    }
                        
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => Vampire(type,dmg);
                break;
            case eAbilities.SoulBreaker: // 최대 마나 비례 마나 감소  1~10%까지
                void SoulBreaker(eTriggerType type, float finalDmg)
                {
                    Enemy enemy = _unit as Enemy;
                    if(enemy !=null)
                    {
                        if(enemy.Target!=null)
                        {
                            enemy.Target.UseMana(currentValue,eNumType.Percent, true);
                        }    
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => SoulBreaker(type, dmg);
                break;
            case eAbilities.Magma:
                void burn()
                {
                    if (Random.Range(0, 100) >= currentValue)
                    {
                        return;
                    }
                    if (_unit is Enemy)
                    {
                        Enemy enemy = _unit as Enemy;
                        if (enemy.Target == null) return;
                        DamageStruct stat = enemy.GetEnemyDamageStruct();

                        PlayerSkillController.AddSecondaryEffect(stat, eSecondaryEffect.Burn, 1, enemy.Target.gameObject);
                    }
                    else if (_unit is UserCharacter)
                    {
                        UserCharacter user = _unit as UserCharacter;
                        if (user.Target == null) return;
                        DamageStruct stat = user.GetUserCharacterDamageStruct();

                        PlayerSkillController.AddSecondaryEffect(stat, eSecondaryEffect.Burn, 1, user.Target.gameObject);
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => burn();
                break;
            case eAbilities.Blizard:
                void FrostBite()
                {
                    if (Random.Range(0, 100) >= currentValue)
                    {
                        return;
                    }
                    if (_unit is Enemy)
                    {
                        Enemy enemy = _unit as Enemy;
                        if (enemy.Target == null) return;
                        DamageStruct stat = enemy.GetEnemyDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.FrostBite, 1, enemy.Target.gameObject);
                    }
                    else if (_unit is UserCharacter)
                    {
                        UserCharacter user = _unit as UserCharacter;
                        if (user.Target == null) return;
                        DamageStruct stat = user.GetUserCharacterDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.FrostBite, 1, user.Target.gameObject);
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => FrostBite();
                break;
            case eAbilities.Electrocute:
                void ElectricShock()
                {
                    if (Random.Range(0, 100) >= currentValue)
                    {
                        return;
                    }
                    if (_unit is Enemy)
                    {
                        Enemy enemy = _unit as Enemy;
                        if (enemy.Target == null) return;
                        DamageStruct stat = enemy.GetEnemyDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.ElectricShock, 1, enemy.Target.gameObject);
                    }
                    else if (_unit is UserCharacter)
                    {
                        UserCharacter user = _unit as UserCharacter;
                        if (user.Target == null) return;
                        DamageStruct stat = user.GetUserCharacterDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.ElectricShock, 1, user.Target.gameObject);
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => ElectricShock();
                break;
            case eAbilities.Death:
                void Poison()
                {
                    if (Random.Range(0, 100) >= currentValue)
                    {
                        return;
                    }
                    if (_unit is Enemy)
                    {
                        Enemy enemy = _unit as Enemy;
                        if (enemy.Target == null) return;
                        DamageStruct stat = enemy.GetEnemyDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.Poison, 1, enemy.Target.gameObject);
                    }
                    else if (_unit is UserCharacter)
                    {
                        UserCharacter user = _unit as UserCharacter;
                        if (user.Target == null) return;
                        DamageStruct stat = user.GetUserCharacterDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.Poison, 1, user.Target.gameObject);
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => Poison();
                break;
            case eAbilities.Hemophilia:
                void Bleeding()
                {
                    if (Random.Range(0, 100) >= currentValue)
                    {
                        return;
                    }
                    if (_unit is Enemy)
                    {
                        Enemy enemy = _unit as Enemy;
                        if (enemy.Target == null) return;
                        DamageStruct stat = enemy.GetEnemyDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.Bleeding, 1, enemy.Target.gameObject);
                    }
                    else if (_unit is UserCharacter)
                    {
                        UserCharacter user = _unit as UserCharacter;
                        if (user.Target == null) return;
                        DamageStruct stat = user.GetUserCharacterDamageStruct();

                        SkillController.AddSecondaryEffect(stat, eSecondaryEffect.Bleeding, 1, user.Target.gameObject);
                    }
                }
                _unit.OnDoHitEnd += (eTriggerType type, float dmg) => Bleeding();
                break;
        }
    }
}
