using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedDebuff
{
    protected UnitBase _unitBase;
    protected float _duration;
    protected int _effectStacks;
    protected int _stackCount = 1;
    public int GetEffectStatckCount { get { return _effectStacks; } }
    public ScriptableDebuff Debuff { get; }
    protected readonly GameObject _target;
    public bool IsFinished { get; set; }

    protected float _proc;
    protected float _value;
    protected DamageStruct _attackStat;
    public float GetCurrentDuration => _duration;
    public TimedDebuff(ScriptableDebuff debuff, GameObject target)
    {
        Debuff = debuff;
        _target = target;
    }

    public void Tick(float delta)
    {
        _duration -= delta;
        if(_duration <=0)
        {
            End();
        }
    }
    public void Activate()
    {
        if(Debuff.IsEffectStacked || _duration<=0)
        {
            ApplyEffect();
            if (_effectStacks< Debuff.EffectMaxStack)
            {
                _effectStacks = Mathf.Min(_effectStacks + _stackCount, Debuff.EffectMaxStack);
            }
        }
        if(Debuff.IsDurationStacked || _duration<=0)
        {
            _duration += Debuff.Duration;
        }
        else if(!Debuff.IsDurationStacked)
        {
            _duration = Debuff.Duration;
        }
    }

    /// <summary>
    /// 필수 구현 // Activate할 때 적용해야 할 항목들 필수 구현(상태창에 아이콘 띄우기, unitbase에 적절한 action에 할당
    /// </summary>
    protected abstract void ApplyEffect();
    /// <summary>
    /// 발동 조건이 됐을 때 발동시키는 메서드
    /// </summary>
    protected abstract void OnTriggerInvoke(eTriggerType eTriggerType);
    public abstract void SetDebuffStat(in DamageStruct attackStat, int stackCount);
    public abstract void End();
}
