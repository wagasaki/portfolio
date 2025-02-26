using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillController : ObjectPool<SkillObject>
{


    protected void InstantiateSkillObject(DamageStruct stat, int index, SkillDataEntity data, UnitBase caster, UnitBase target )
    {
        float accuracy=0;
        if (caster is UserCharacter)
        {
            UserCharacter character = caster as UserCharacter;
            accuracy = character.BaseAccuracy;
        }
        float effectAccuracy = data.SecondaryEffectProc * (1 + accuracy);
        Debug.Log("스킬 발동으로 인한 디버프 부여 확률 : " + effectAccuracy);

        switch (data.SkillActionType)
        {
            case eSkillActionType.DmgSkill:
                {
                    SkillObject obj = GetFromPool(index, this.transform);
                    obj.gameObject.SetActive(true);
                    obj.InitAttackSkillObj(stat, target);
                }
                break;
            case eSkillActionType.DmgAddDebuffSkill:
                {
                    SkillObject obj = GetFromPool(index, this.transform);
                    obj.gameObject.SetActive(true);
                    obj.InitAttackSkillObj(stat, target);
                    for (int i = 0; i < stat.HitCount; i++)
                    {
                        if (Random.Range(0f, 1f) < effectAccuracy)
                        {
                            AddSecondaryEffect(stat, data.SecondaryEffectKeyword, data.SecondaryEffectStackCount, target.gameObject);
                        }
                    }
                }
                break;
            case eSkillActionType.DmgAddBuffSkill:
                {
                    SkillObject obj = GetFromPool(index, this.transform);
                    obj.gameObject.SetActive(true);
                    obj.InitAttackSkillObj(stat, target);
                    for (int i = 0; i < stat.HitCount; i++)
                    {
                        if (Random.Range(0f, 1f) < effectAccuracy)
                        {
                            AddSecondaryEffect(stat, data.SecondaryEffectKeyword, data.SecondaryEffectStackCount, target.gameObject);
                        }
                    }
                }
                break;
            case eSkillActionType.DmgExplodeDebuffSkill:
                {
                    foreach (var a in target.GetComponent<UnitBase>().CurrentDebuffs)
                    {
                        if (a.Key.SecondaryEffectType == data.SecondaryEffectKeyword)
                        {
                            Debug.Log(a.Key.SecondaryEffectType + " == " + data.SecondaryEffectKeyword);
                            float total = 1 + (data.SecondaryEffectValue * a.Value.GetEffectStatckCount);
                            stat.Dmg = (int)(stat.Dmg * total);
                            target.GetComponent<UnitBase>().RemoveDebuff(a.Value);
                            break;
                        }
                    }

                    SkillObject obj = GetFromPool(index, this.transform);
                    obj.gameObject.SetActive(true);
                    obj.InitAttackSkillObj(stat, target);
                }
                break;
            case eSkillActionType.SelfBuffSkill:
                {
                    {
                        SkillObject obj = GetFromPool(index, this.transform);
                        obj.gameObject.SetActive(true);
                        obj.InitBuffSkillObj(stat, target);
                        for (int i = 0; i < stat.HitCount; i++)
                        {
                            if (Random.Range(0f, 1f) < effectAccuracy)
                            {
                                AddSecondaryEffect(stat, data.SecondaryEffectKeyword, data.SecondaryEffectStackCount, target.gameObject);
                            }
                        }
                    }
                }
                break;
            case eSkillActionType.DebuffSkill:
                {
                    {
                        for (int i = 0; i < stat.HitCount; i++)
                        {
                            if (Random.Range(0f, 1f) < effectAccuracy)
                            {
                                AddSecondaryEffect(stat, data.SecondaryEffectKeyword, data.SecondaryEffectStackCount, target.gameObject);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }


    public static void AddSecondaryEffect(in DamageStruct stat, eSecondaryEffect effect, int stackCount, GameObject target)
    {
        if (target == null) return;

        UnitBase unitBase = target.GetComponent<UnitBase>();
        if(unitBase is UserCharacter)
        {
            UserCharacter user = (UserCharacter)unitBase;
            float resist = user.BaseResist;
            float proc = Random.Range(0, 1f);
            if (proc < resist)
            {
                Debug.Log("Resist!! " + resist);
                user.ResistDebuff();
                return;
            }

        }
        switch (effect)
        {
            case eSecondaryEffect.Burn:
                //if (target != null)
                //{
                TimedBurnDebuff burn = new TimedBurnDebuff(Resources.Load<ScriptableBurnDebuff>(Paths.Burn), target);
                burn.SetDebuffStat(stat, stackCount);
                unitBase.AddDebuff(burn);
                //}

                break;
            case eSecondaryEffect.ElectricShock:
                //if (target != null)
                //{
                TimedElectricShockDebuff electricShock = new TimedElectricShockDebuff(Resources.Load<ScriptableElectricShockDebuff>(Paths.ElectricShock), target);
                electricShock.SetDebuffStat(stat, stackCount);
                unitBase.AddDebuff(electricShock);
                //}
                break;
            case eSecondaryEffect.FrostBite:
                //if (target != null)
                //{
                TimedFrostBiteDebuff frostbite = new TimedFrostBiteDebuff(Resources.Load<ScriptableFrostBiteDebuff>(Paths.FrostBite), target);
                frostbite.SetDebuffStat(stat, stackCount);
                unitBase.AddDebuff(frostbite);
                //}
                break;
            case eSecondaryEffect.Poison:
                //if (target != null)
                //{
                TimedPoisonDebuff poison = new TimedPoisonDebuff(Resources.Load<ScriptablePoisonDebuff>(Paths.Poison), target);
                poison.SetDebuffStat(stat, stackCount);
                unitBase.AddDebuff(poison);
                //}
                break;
            case eSecondaryEffect.Bleeding:
                //if (target != null)
                //{
                TimedBleedingDebuff bleeding = new TimedBleedingDebuff(Resources.Load<ScriptableBleedingDebuff>(Paths.Bleeding), target);
                bleeding.SetDebuffStat(stat, stackCount);
                unitBase.AddDebuff(bleeding);
                //}
                break;
            case eSecondaryEffect.Shield:
                break;
            case eSecondaryEffect.Enchant:
                break;
            case eSecondaryEffect.Reduction:
                break;
            case eSecondaryEffect.Cleansing:
                break;
            case eSecondaryEffect.Burst:
                break;
            case eSecondaryEffect.Test5:
                break;
            default:
                break;
        }
    }
}
