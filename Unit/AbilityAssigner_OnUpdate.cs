using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAssigner_OnUpdate : AbilityAssigner
{
    private float _proc = 0;
    public override void Excute(eAbilities ability, float mainValue)
    {
        CurrentAbility = ability;
        switch (ability)
        {
            case eAbilities.Phoenix:
                _unit.OnUpdateStart += (eTriggerType type) => { HPRegen(); };
                void HPRegen()
                {
                    _proc += Time.deltaTime;
                    if (_proc > 1)
                    {
                        _proc = 0;
                        _unit.GetHeal(mainValue, eNumType.Percent);
                    }
                }
                break;
            case eAbilities.Sage:
                _unit.OnUpdateStart += (eTriggerType type) => { MPRegen(); };
                void MPRegen()
                {
                    _proc += Time.deltaTime;
                    if (_proc > 1)
                    {
                        _proc = 0;
                        _unit.GetMana(mainValue, eNumType.Percent);
                    }
                }
                break;
        }
    }
}
