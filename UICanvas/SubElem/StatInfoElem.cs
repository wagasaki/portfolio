using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatInfoElem : UIBase
{
    private eStatInfo _eStatinfo;
    enum Texts
    {
        AbilityNameText,
        AbilityAmountText
    }



    public void InitUIElem(eStatInfo statinfo)
    {
        BindText(typeof(Texts));
        _eStatinfo = statinfo;
    }

    public void RefreshText()
    {
        string amountString = string.Empty;
        string colorString = "<color=white>";

        if(_eStatinfo == eStatInfo.Dmg)
        {
            InitController.Instance.GamePlays.IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MinDmg, out Dictionary<eStatCalcType, float> values);
            InitController.Instance.GamePlays.IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MaxDmg, out Dictionary<eStatCalcType, float> values2);
            values.TryGetValue(eStatCalcType.AddInt, out float add);
            values.TryGetValue(eStatCalcType.AddPercent, out float percent);
            values2.TryGetValue(eStatCalcType.AddInt, out float add2);
            values2.TryGetValue(eStatCalcType.AddPercent, out float percent2);
            if (add != 0 || percent != 0 || add2 != 0 || percent2 !=0)
            {
                colorString = "<color=green>";
            }
        }
        else if(_eStatinfo == eStatInfo.ASpeed)
        {
            InitController.Instance.GamePlays.IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.ASpeed, out Dictionary<eStatCalcType, float> values);
            values.TryGetValue(eStatCalcType.MultiplyPercent, out float multi_per);

            if(multi_per != 0)
            {
                colorString = "<color=green>";
            }
        }
        else
        {
            InitController.Instance.GamePlays.IngameStat.AddictiveStatDic.TryGetValue(_eStatinfo, out Dictionary<eStatCalcType, float> values);

            values.TryGetValue(eStatCalcType.AddInt, out float add);
            values.TryGetValue(eStatCalcType.AddPercent, out float percent);

            if (add != 0 || percent != 0)
            {
                colorString = "<color=green>";
            }
        }

        #region regacy
        //switch (_eStatinfo)
        //{
        //    case eStatInfo.HP:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.HP)}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.HP + addicts).ToString();
        //        break;
        //    case eStatInfo.Atk:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.Atk)}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.Atk + addicts).ToString();
        //        break;
        //    case eStatInfo.Def:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Def}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.Def + addicts).ToString();
        //        break;
        //    case eStatInfo.ASpeed:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Aspeed}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.Aspeed + addicts).ToString();
        //        break;
        //    case eStatInfo.CritChance:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.CritChance:P1} {colorString} (+ {total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.CritChance + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.CritDmg:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.CritDmg:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.CritDmg + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.Accuracy:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Accuracy:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.Accuracy + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.Mana:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Mana}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.Mana + addicts).ToString();
        //        break;
        //    case eStatInfo.ManaRegen:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.ManaRegen}{colorString}(+{total})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.ManaRegen + addicts).ToString();
        //        break;
        //    case eStatInfo.ItemFind:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.ItemFind:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.ItemFind + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.DmgReduction:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.DmgReduction:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.DmgReduction + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.MinDmg:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.MinDmg:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.MinDmgRange + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.MaxDmg:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.MaxDmg:P1}{colorString}(+{total:P1})");
        //        //GetText((int)Texts.AbilityAmountText).text = (InitController.Instance.GamePlays.IngameStat.MaxDmgRange + addicts).ToString("P1");
        //        break;
        //    case eStatInfo.Stamina:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Stamina}{colorString}(+{total})");
        //        break;
        //    case eStatInfo.Gold:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Gold:P1}{colorString}(+{total:P1})");
        //        break;
        //    case eStatInfo.Exp:
        //        amountString = ($"{InitController.Instance.GamePlays.IngameStat.Exp:P1}{colorString}(+{total:P1})");
        //        break;

        //    default:
        //        break;
        //}
        #endregion
        switch (_eStatinfo)
        {
            case eStatInfo.HP:
            case eStatInfo.Atk:
            case eStatInfo.Def:
            case eStatInfo.Mana:
            case eStatInfo.ManaRegen:
            case eStatInfo.HPRegen:
            case eStatInfo.Stamina:
                amountString = string.Format("{1}{0:N0}", InitController.Instance.GamePlays.IngameStat.GetTotalStat(_eStatinfo), colorString);
                break;
            case eStatInfo.ASpeed:
                amountString = string.Format("{1}{0:N1}/s", 1/InitController.Instance.GamePlays.IngameStat.GetTotalStat(_eStatinfo), colorString);
                break;
            case eStatInfo.CritChance:
            case eStatInfo.CritDmg:
            case eStatInfo.DmgReduction:
            case eStatInfo.MinDmg:
            case eStatInfo.MaxDmg:
                amountString = string.Format("{1}{0:P0}", InitController.Instance.GamePlays.IngameStat.GetTotalStat(_eStatinfo), colorString);
                break;
            case eStatInfo.ItemFind:
            case eStatInfo.Gold:
            case eStatInfo.Exp:
                amountString = string.Format("{1}{0:P0}", InitController.Instance.GamePlays.IngameStat.GetTotalStat(_eStatinfo), colorString);
                //Debug.Log(_eStatinfo + " / " +InitController.Instance.GamePlays.IngameStat.GetTotalStat(_eStatinfo));
                break;
            case eStatInfo.Dmg:
                amountString = string.Format("{0}{1} - {2}", 
                    colorString, 
                    InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.MinDmg), 
                    InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.MaxDmg));
                break;
            default:
                break;
        }

        GetText((int)Texts.AbilityAmountText).text = amountString;
    }
}
