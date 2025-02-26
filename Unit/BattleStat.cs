using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStat
{
    /// <summary>
    /// 동상 등의 스탯에 영향을 주는 디버프 부여할 때 사용
    /// </summary>
    public Dictionary<eStatInfo, Dictionary<string, float>> StatModifierDic { get; private set; }
    /// <summary>
    /// 이곳에서는 모두 합산으로(차후에 계산 방식이 다른 항목이 추가될 경우에는..인게임스탯처럼 나눠도되긴한데. 아직은 필요X) 예시)공속 + 0.7과 -0.5가 들어간 경우 합산으로 +0.2가 적용됨.
    /// </summary>
    public Dictionary<eStatInfo, float> CalculatedStatModifierDic { get; private set; }


    public void InitData()
    {
        StatModifierDic = new Dictionary<eStatInfo, Dictionary<string, float>>();
        CalculatedStatModifierDic = new Dictionary<eStatInfo, float>();
        int count = InitController.Instance.GameDatas.StatInfoEnumCount;
        for (int i = 0; i < count; i++)
        {
            SetModifier((eStatInfo)i);
        }
    }
    /// <summary>
    /// 스텟 변경
    /// </summary>
    /// <param name="statinfo"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void ModifyStat(eStatInfo statinfo,string key, float value)
    {
        if (StatModifierDic[statinfo].ContainsKey(key))
        {
            StatModifierDic[statinfo][key] = value;
        }
        else
        {
            StatModifierDic[statinfo].Add(key, value);
        }
        SetModifier(statinfo);
    }
    /// <summary>
    /// 변경한 스탯 초기화(제거)
    /// </summary>
    /// <param name="statinfo"></param>
    /// <param name="key"></param>
    public void RemoveModified(eStatInfo statinfo, string key)
    {
       StatModifierDic[statinfo].Remove(key);
       SetModifier(statinfo);
    }
    /// <summary>
    /// 아직은 합산만. 최소 수치 0으로 -까지는 안감// modify, remove할때 무조건 호출 해줘야함. 픽스해주는 개념으로다가
    /// </summary>
    /// <param name="statinfo"></param>
    private void SetModifier(eStatInfo statinfo)
    {
        float result = 1;
        if(StatModifierDic.ContainsKey(statinfo))
        {
            foreach (var a in StatModifierDic[statinfo])
            {
                result += a.Value;
            }
        }
        else
        {
            StatModifierDic.Add(statinfo, new Dictionary<string, float>());
        }

        if(CalculatedStatModifierDic.ContainsKey(statinfo))
        {
            CalculatedStatModifierDic[statinfo] = Mathf.Max(0.1f, result);
        }
        else
        {
            CalculatedStatModifierDic.Add(statinfo, Mathf.Max(0.1f, result));
        }
        #region regacy
        //switch (statinfo)
        //{
        //    case eStatInfo.HP:
        //        foreach (var a in HPModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.Atk:
        //        foreach (var a in AtkModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.Def:
        //        foreach (var a in DefModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.ASpeed:
        //        foreach (var a in ASpeedModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.CritChance:
        //        foreach (var a in CritChanceModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.CritDmg:
        //        foreach (var a in CritDmgModifierDic.Values)
        //        {
        //            result += a;
        //        }
        //        break;
        //    case eStatInfo.Accuracy:
        //        break;
        //    case eStatInfo.Mana:
        //        break;
        //    case eStatInfo.ManaRegen:
        //        break;
        //    case eStatInfo.ItemFind:
        //        break;
        //    case eStatInfo.DmgReduction:
        //        break;
        //    case eStatInfo.MinDmg:
        //        break;
        //    case eStatInfo.MaxDmg:
        //        break;
        //    case eStatInfo.Stamina:
        //        break;
        //    case eStatInfo.Gold:
        //        break;
        //    case eStatInfo.Exp:
        //        break;
        //    default:
        //        break;
        //}
        #endregion
    }

}
