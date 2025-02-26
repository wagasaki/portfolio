using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected readonly Dictionary<ScriptableDebuff, TimedDebuff> _currentDebuffs = new Dictionary<ScriptableDebuff, TimedDebuff>();
    public Dictionary<ScriptableDebuff, TimedDebuff> CurrentDebuffs { get { return _currentDebuffs; }}


    protected float _currentHP;

    public bool IsAlive { get; protected set; }
    /// <summary>
    /// 피격 시
    /// </summary>
    public Action<eTriggerType> OnGetHitStart { get; set; }
    /// <summary>
    /// 일반공격 시작 시.
    /// </summary>
    public Action<eTriggerType> OnDoHitStart { get; set; }
    public Action<eTriggerType, float> OnDoHitEnd { get; set; }
    public Action OnHealStart { get; set; }
    public Action OnEffectFirstStart { get; set; }
    /// <summary>
    /// 매 업데이트 시.
    /// </summary>
    public Action<eTriggerType> OnUpdateStart { get; set; }
    /// <summary>
    /// 효과 적용 시.
    /// </summary>
    public Action<eTriggerType> OnAfterAddEffect { get; set; }

    public BattleStat CurrentBattleStat { get; set; }


    protected float _hptimeProc;
    public abstract void InitUnit(object data, float hpmodifier);

    protected float Atk_Def_Calc(float atk, float def)
    {
        float multiplier;
        if(atk >= def)
        {
            multiplier = 1 + Mathf.Log(atk / def, 3);
            //Debug.Log("atk/def = "+atk/def + "log2(atk/def) = " + multiplier);
            return multiplier;
        }
        else
        {
            multiplier = 1 / (1 + Mathf.Log(def / atk, 3));
            //Debug.Log("def/atk = " + def/atk  + "log2(def/atk) = " + multiplier);
            return multiplier;
        }
    }

    private void Update()
    {
        UpdateMethod();
    }
    protected virtual void UpdateMethod()
    {
        OnUpdateStart?.Invoke(eTriggerType.Default);
        UpdateDebuff();
    }
    protected virtual void UpdateDebuff()
    {
        foreach(TimedDebuff debuff in _currentDebuffs.Values.ToList())
        {
            debuff.Tick(Time.deltaTime);
            if(debuff.IsFinished)
            {
                RemoveDebuff(debuff);
            }
            else
            {
                if (this is UserCharacter)
                {
                    InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerDebuff(debuff);
                }
                else if (this is Enemy)
                {
                    InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyDebuff(debuff);
                }
            }
        }
    }
    public void AddDebuff(TimedDebuff debuff)
    {
        if (IsAlive == false || InitController.Instance.GamePlays.IsBattle == false)
            return;

        if (_currentDebuffs.ContainsKey(debuff.Debuff))
        {
            _currentDebuffs[debuff.Debuff].Activate();
            if (this is UserCharacter)
            {
                InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerDebuff(debuff);
            }
            else if (this is Enemy)
            {
                InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyDebuff(debuff);
            }
        }
        else
        {
            _currentDebuffs.Add(debuff.Debuff, debuff);
            debuff.Activate();
            OnEffectFirstStart?.Invoke();//TODO 이거 위로 올라가야되는거 아닌가? 일단은 쓸일이 없어서 아래 둬도 상관없는데. 혹시 쓰게 된다면 위로 올려야될거같음
            if (this is UserCharacter)
            {
                InitController.Instance.UIs.GetBattlelUICanvas.AddPlayerState(debuff);
            }
            else if (this is Enemy)
            {
                InitController.Instance.UIs.GetBattlelUICanvas.AddEnemySate(debuff);
            }
        }
        OnAfterAddEffect?.Invoke(eTriggerType.Default);
    }
    public void RemoveDebuff(TimedDebuff debuff)
    {
        debuff.End();
        _currentDebuffs.Remove(debuff.Debuff);
        if (this is UserCharacter)
        {
            InitController.Instance.UIs.GetBattlelUICanvas.RemovePlayerState(debuff);
        }
        else if (this is Enemy)
        {
            InitController.Instance.UIs.GetBattlelUICanvas.RemoveEnemyState(debuff);
        }
    }    
    public abstract void GetHit(in DamageStruct attackStat, Action<eTriggerType, float> OnHitEnd, WaitForSeconds tick = null);

    public abstract void GetHeal(float healValue, eNumType numType);
    public abstract void GetMana(float gain, eNumType numType);

    public abstract int GetNormalAttackDamageCalc(float pow);

}

