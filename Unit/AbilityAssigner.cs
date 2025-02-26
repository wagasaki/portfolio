using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityAssigner : MonoBehaviour
{
    protected UnitBase _unit;
    public eAbilities CurrentAbility { get; protected set; }
    protected void Awake()
    {
        _unit = GetComponent<UnitBase>();
    }

    /// <summary>
    /// value 는 퍼센트의 상수값. 4%면 4, 1.5%면 1.5, 100% 면 100
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="value"></param>
    public abstract void Excute(eAbilities ability, float value);
}
