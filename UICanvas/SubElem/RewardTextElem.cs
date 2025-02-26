using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardTextElem : UIBase
{
    private enum Texts
    {
        RewardText,
        ExpText,
    }
    private enum Images
    {
        RewardImage,
        ExpGauge,
    }
    private enum GameObjects
    {
        Exp,
    }

    private Image _rewardImage, _expGauge;
    private TextMeshProUGUI _rewardText, _expText;
    private GameObject _expObj;
    private float _alpha;

    private void Awake()
    {
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));
        _rewardText = GetText((int)Texts.RewardText);
        _rewardImage = GetImage((int)Images.RewardImage);
        _expGauge = GetImage((int)Images.ExpGauge);
        _expObj = GetObject((int)GameObjects.Exp);
        _expText = GetText((int)Texts.ExpText);
    }
    public void GoldText(string value)
    {
        StartCoroutine(InitElem());
        _rewardImage.sprite = InitController.Instance.GameDatas.SpriteDic["Gold"];
        int before = InitController.Instance.GamePlays.IngameStat.CurrentGold - int.Parse(value);

        _expObj.SetActive(false);
        StartCoroutine(GoldTextRolling(before, InitController.Instance.GamePlays.IngameStat.CurrentGold));
    }
    public void ExpText(float exp, int beforelevel, float beforeTotalExp)
    {
        StartCoroutine(InitElem());
        _rewardImage.sprite = InitController.Instance.GameDatas.SpriteDic["EXP"];

        _expObj.SetActive(true);
        StartCoroutine(ExpRolling(exp, beforelevel, beforeTotalExp));
    }
    public void Soul_StaminaText(int amount, Sprite sprite)
    {
        _rewardImage.sprite = sprite;
        _rewardText.text = amount.ToString("N0");
        _expObj.SetActive(false);
    }
    public float Alpha { 
        get { return _alpha; } 
        set 
        { 
            _alpha = value;
            Color color = _rewardImage.color;
            color.a = _alpha;
            _rewardImage.color = color;
            _rewardText.alpha = _alpha;
        } 
    }
    IEnumerator InitElem()
    {
        Alpha = 0;
        while (Alpha < 1)
        {
            Alpha += Time.deltaTime * 2;
            yield return null;
        }
    }
    IEnumerator GoldTextRolling(int startnum, int endnum)
    {
        WaitForSeconds tick = new WaitForSeconds(0.04f);

        int tickNum = (endnum - startnum) / 50;
        int amount = endnum - startnum;
        _rewardText.text = startnum.ToString();
        while(startnum < endnum)
        {
            startnum += tickNum;
            if (startnum >= endnum)
                startnum = endnum;

            _rewardText.text = string.Format("{0:N0}", startnum); //startnum.ToString("#,###") + " Gold";
            yield return tick;
        }
        _rewardText.text = string.Format("{0:N0} <color=green>(+{1:N0})</color>", startnum, amount); //startnum.ToString("#,###") + " Gold";
    }
    IEnumerator ExpRolling(float exp, int beforelevel, float beforeTotalExp)
    {
        float endpoint = exp + beforeTotalExp;

        _rewardText.text = string.Format("Lv {0}",beforelevel);
        _expText.text = string.Format("{0:N0}/{1:N0}", beforeTotalExp - Mathf.Pow(beforelevel - 1, Nums.ExpModifier), Mathf.Pow(beforelevel, Nums.ExpModifier) - Mathf.Pow(beforelevel - 1, Nums.ExpModifier));
        
        float startExp = beforeTotalExp - Mathf.Pow(beforelevel - 1, Nums.ExpModifier);

        int endlv = Mathf.FloorToInt(Mathf.Pow(beforeTotalExp+exp, 1 / Nums.ExpModifier)) + 1;

        float beforelv = beforelevel;
        float lvGap = endlv - beforelv;


        if(lvGap>0)
        {
            float gapcheck;
            gapcheck = Mathf.Min(lvGap, 50);
            while (beforelv < endlv)
            {
                float gaptick = lvGap / gapcheck;

                float m = Mathf.Pow(beforelv, Nums.ExpModifier) - Mathf.Pow(beforelv - 1, Nums.ExpModifier); ;

                while (startExp < m)
                {
                    startExp += Time.deltaTime * m * 30;

                    _expText.text = string.Format("{0:N0}/{1:N0}", startExp, m);
                    _expGauge.fillAmount = startExp / m;

                    yield return null;
                }
                startExp = 0;
                _rewardText.text = string.Format("Lv {0:N0} <color=green>(+{1:N0})</color>", beforelv, beforelv - beforelevel);
                _expText.text = string.Format("{0:N0}/{1:N0}", Mathf.FloorToInt(startExp), Mathf.CeilToInt(m));
                _expGauge.fillAmount = startExp / m;

                beforelv = beforelv + gaptick;

                if (beforelv >= endlv)
                {
                    beforelv = endlv;
                    float endexp = endpoint - Mathf.Pow(beforelv - 1, Nums.ExpModifier);
                    float max = Mathf.Ceil( Mathf.Pow(beforelv, Nums.ExpModifier) - Mathf.Pow(beforelv - 1, Nums.ExpModifier));
                    // 올림 해준건 가끔 경험치가 소수점 차이로 7/7, 20/20처럼 보이는 경우가 있어서 방지용
                    while (startExp < endexp)
                    {
                        startExp += Time.deltaTime * endexp;

                        _rewardText.text = string.Format("Lv {0:N0} <color=green>(+{1:N0})</color>", beforelv, beforelv - beforelevel);
                        _expText.text = string.Format("{0:N0}/{1:N0}", startExp, max);
                        _expGauge.fillAmount = startExp / max;

                        yield return null;
                    }


                    yield break;
                }

                yield return null;
            }
        }
        else
        {
            float endexp = endpoint - Mathf.Pow(beforelv - 1, Nums.ExpModifier);
            float max = Mathf.Ceil(Mathf.Pow(beforelv, Nums.ExpModifier) - Mathf.Pow(beforelv - 1, Nums.ExpModifier));
            while (startExp < endexp)
            {
                startExp += Time.deltaTime * endexp;

                _expText.text = string.Format("{0:N0}/{1:N0}", startExp, max);
                _expGauge.fillAmount = startExp / max;

                yield return null;
            }
        }
    }

}
