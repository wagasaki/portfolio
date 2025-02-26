using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class BattleUICanvas : UICanvas
{
    [SerializeField]
    private Button _battleStartBtn, _X1Btn,_autoStartBtn;
    [SerializeField]
    private TextMeshProUGUI _currentLevelText, _timerText;
    [SerializeField]
    private Image _autoActiveAnim;



    private Image _playerHPGauge, _playerShieldGauge, _playerMPGauge, _enemyHPGauge;//, _enemyMPGauge;
    private TextMeshProUGUI _speedBtnText, _playerHPText, _playerMPText, _enemyHPText;//, _enemyMPText;
    private DmgBoard _dmgBoard;

    public Transform _textEffectParent;
    public SkillUseElem[] GetSkillUseElems { get; private set; }
    private GameObject _battleMap;
    private DropTableDisplay _dropTableDisplay;
    private Button[] AutoCastButtons = new Button[Nums.SkillStoneCount];
    private SkillNameDisplayPool _skillNameDisplayPool;
    private SkillNameDisplayEffect _skillNameDisplayEffectPrefab;
    public enum GameObjects
    {
        PlayerStateInfoContent,
        EnemyStateInfoContent,
        TextEffectParent,
        DmgBoard,
        DropTableDisplay,
        SkillNameDisplayPool,
        EnemyAbilityInfo,
        InformImage,
        BannerImage
    }
    public enum SkillUseElems
    {
        Skill_0,
        Skill_1,
        Skill_2,
        Skill_3,
    }
    public enum Images
    {
        PlayerHPGauge,
        PlayerShieldGauge,
        PlayerMPGauge,
        EnemyHPGauge,
        //EnemyMPGauge,
    }
    public enum Texts
    {
        SpeedBtnText,
        PlayerInfoText,
        EnemyInfoText,
        PlayerHPText,
        PlayerMPText,
        EnemyHPText,
        //EnemyMPText,
        EnemyAbilityNameText,
        EnemyAbilityInfoText,
        TimerText
    }
    public enum Buttons
    {
        AutoCastButton0,
        AutoCastButton1,
        AutoCastButton2,
        AutoCastButton3,
        //RetreatButton,
        //StartButton,
    }

    private void Awake()
    {
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        Bind<SkillUseElem>(typeof(SkillUseElems));
        PlayerStateInfoDic = new Dictionary<ScriptableDebuff, StateInfo>();
        EnemyStateInfoDic = new Dictionary<ScriptableDebuff, StateInfo>();

        _speedBtnText = GetText((int)Texts.SpeedBtnText);
        _dmgBoard = GetObject((int)GameObjects.DmgBoard).GetComponent<DmgBoard>();

        _playerHPGauge = GetImage((int)Images.PlayerHPGauge);
        _playerShieldGauge = GetImage((int)Images.PlayerShieldGauge);
        _playerMPGauge = GetImage((int)Images.PlayerMPGauge);
        _enemyHPGauge = GetImage((int)Images.EnemyHPGauge);
        //_enemyMPGauge = GetImage((int)Images.EnemyMPGauge);

        _playerHPText = GetText((int)Texts.PlayerHPText);
        _playerMPText = GetText((int)Texts.PlayerMPText);
        _enemyHPText = GetText((int)Texts.EnemyHPText);
        //_enemyMPText = Get<TextMeshProUGUI>((int)Texts.EnemyMPText);
        _timerText = GetText((int)Texts.TimerText);

        ProperCamsize = Nums.CamSizeMax;

        _textEffectParent = GetObject((int)GameObjects.TextEffectParent).transform;

        _dropTableDisplay = GetObject((int)GameObjects.DropTableDisplay).GetComponent<DropTableDisplay>();

        _skillNameDisplayPool = GetObject((int)GameObjects.SkillNameDisplayPool).GetComponent<SkillNameDisplayPool>();
        _skillNameDisplayPool.SetPrefab(Resources.Load<SkillNameDisplayEffect>(Paths.SkillNameDisplayEffect));
        _skillNameDisplayPool.InitPool();

        GetText((int)Texts.EnemyAbilityNameText).GetComponent<UIEventHandler>().OnClickHandler += () => FadeAbilityInfo();
    }
    float AdsHeight;
    Action OnRemoveAdsAction;
    private void Start()
    {

        AdsHeight = Camera.main.GetComponent<GameCamera>().AdsHeight;
        RectTransform informrect = GetObject((int)GameObjects.InformImage).GetComponent<RectTransform>();
        informrect.anchoredPosition = new Vector2(0, -AdsHeight);
        RectTransform bannerimage = GetObject((int)GameObjects.BannerImage).GetComponent<RectTransform>();
        bannerimage.sizeDelta = new Vector2(bannerimage.sizeDelta.x, AdsHeight + informrect.sizeDelta.y);
        if (InitController.Instance.SaveDatas.UserData.GetIsAdsHide == true)
        {
            OnRemoveAds();
            return;
        }
        void OnRemoveAds()
        {
            informrect.anchoredPosition = Vector2.zero;
            bannerimage.sizeDelta = new Vector2(bannerimage.sizeDelta.x, informrect.sizeDelta.y);
        }
        OnRemoveAdsAction = () => OnRemoveAds();
        if (InitController.Instance.SaveDatas.UserData.GetIsAdsHide == false)
        {
            InitController.Instance.SaveDatas.OnAdsRemove -= OnRemoveAdsAction;
            InitController.Instance.SaveDatas.OnAdsRemove += OnRemoveAdsAction;
        }
    }
    public override void InitUICanvas()
    {
        base.InitUICanvas();



        _speedBtnText.text = string.Format("X {0}", InitController.Instance.SaveDatas.UserData.SpeedModifier);
        _X1Btn.onClick.AddListener(() => { SpeedButtonActive(InitController.Instance.SaveDatas.UserData.SpeedModifier); });

        #region regacy
        //_X2Btn.onClick.AddListener(() => { InitController.Instance.SaveDatas.UserData.SpeedModifier = 2; InitController.Instance.SetTimeScale(2);  });
        //_X3Btn.onClick.AddListener(() => { InitController.Instance.SaveDatas.UserData.SpeedModifier = 3; InitController.Instance.SetTimeScale(3);  });

        //_autoStartBtn.onClick.AddListener(() =>
        //{
        //    InitController.Instance.SaveDatas.UserData.IsAutoBattleStart = !InitController.Instance.SaveDatas.UserData.IsAutoBattleStart;
        //    _autoActiveAnim.gameObject.SetActive(InitController.Instance.SaveDatas.UserData.IsAutoBattleStart);
        //});
        //_autoActiveAnim.gameObject.SetActive(InitController.Instance.SaveDatas.UserData.IsAutoBattleStart);
        //_battleStartBtn.onClick.AddListener(() => 
        //{
        //    _battleStartBtn.enabled = false;
        //    InitController.Instance.GamePlays.StartBattle();

        //});
        //InitController.Instance.GamePlays.OnBattleEnd += () => 
        //{ 
        //    _battleStartBtn.enabled = true;
        //    OnBattleEnd();
        //};
        #endregion

        InitController.Instance.GamePlays.OnBattleEnd -= OnBattleEnd;
        InitController.Instance.GamePlays.OnBattleEnd += OnBattleEnd;

        GetSkillUseElems =  new SkillUseElem[Nums.SkillStoneCount];
        for (int i = 0; i < GetSkillUseElems.Length; i++)
        {
            GetSkillUseElems[i] = Get<SkillUseElem>(i);
            GetSkillUseElems[i].InitUIElem(i);
        }
        for (int i = 0; i < AutoCastButtons.Length; i++)
        {
            AutoCastButtons[i] = GetButton(i);
            if (InitController.Instance.SaveDatas.UserData.AutoCastEnabled[i] == true)
            {
                AutoCastButtons[i].image.sprite = InitController.Instance.GameDatas.SpriteDic["Switch_On"];
            }
            else
            {
                AutoCastButtons[i].image.sprite = InitController.Instance.GameDatas.SpriteDic["Switch_Off"];
            }
            int index = i;
            AutoCastButtons[index].onClick.AddListener(() => OnOffAutoCastButton(index));
        }

        //GetButton((int)Buttons.StartButton).onClick.AddListener(() => InitController.Instance.GamePlays.StartBattle());
        //GetButton((int)Buttons.RetreatButton).onClick.AddListener(() => InitController.Instance.GamePlays.StartBattle());
        SetInformImagePosByAdsActive(InitController.Instance.SaveDatas.UserData.GetIsAdsHide);
    }
    public void SetInformImagePosByAdsActive(bool isadshide)
    {
        if (isadshide == true)
        {
            GetObject((int)GameObjects.InformImage).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            RectTransform informrect = GetObject((int)GameObjects.InformImage).GetComponent<RectTransform>();
            informrect.anchoredPosition = new Vector2(0, -AdsHeight);
            RectTransform bannerimage = GetObject((int)GameObjects.BannerImage).GetComponent<RectTransform>();
            bannerimage.sizeDelta = new Vector2(bannerimage.sizeDelta.x, AdsHeight + informrect.sizeDelta.y);
        }
    }
    public void OnOffAutoCastButton(int index)
    {
        UserData data = InitController.Instance.SaveDatas.UserData;

        data.AutoCastEnabled[index] = !data.AutoCastEnabled[index];
        if(data.AutoCastEnabled[index] == true)
        {
            GetButton(index).image.sprite = InitController.Instance.GameDatas.SpriteDic["Switch_On"];
        }
        else
        {
            GetButton(index).image.sprite = InitController.Instance.GameDatas.SpriteDic["Switch_Off"];
        }
    }

    public void SpeedButtonActive(int speed)
    { 
        speed += 1;
        speed %= 4;
        if (speed == 0) speed += 1;
        InitController.Instance.SaveDatas.UserData.SpeedModifier = speed; 
        InitController.Instance.SetTimeScale(speed);
        _speedBtnText.text = string.Format("X {0}", speed);
    }

    public void InstantiateBattleMap(eBattleMap battleMap)
    {
        if (_battleMap != null)
        {
            Destroy(_battleMap.gameObject);
            _battleMap = null;
        }

        _battleMap = Instantiate(Resources.Load<GameObject>(Paths.BattleMap + battleMap.ToString()));
        _battleMap.transform.position = new Vector3(0, -0.55f, 0); //new Vector3(0, -0.23f, 0);
    }
    Coroutine _abilrout = null;
    public void EnemyAbilityTextRefresh(string value, string infovalue)
    {
        GetText((int)Texts.EnemyAbilityNameText).text = value;
        GetText((int)Texts.EnemyAbilityInfoText).text = infovalue;
        GameObject info = GetObject((int)GameObjects.EnemyAbilityInfo);
        RectTransform rect = info.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, GetText((int)Texts.EnemyAbilityInfoText).preferredHeight*1.5f);
        info.SetActive(false);
    }
    private void FadeAbilityInfo()
    {

        if (_abilrout != null)
        {
            StopCoroutine(_abilrout);
            _abilrout = null;
            GetObject((int)GameObjects.EnemyAbilityInfo).SetActive(false);
            return;
        }
        _abilrout = StartCoroutine(FadeAbilityInfoRout());

        IEnumerator FadeAbilityInfoRout()
        {
            WaitForSeconds tick = new WaitForSeconds(4f);
            GetObject((int)GameObjects.EnemyAbilityInfo).SetActive(true);
            yield return tick;
            GetObject((int)GameObjects.EnemyAbilityInfo).SetActive(false);
        }
    }
    public void SkillNameDisplay(Color color, string name)
    {
        SkillNameDisplayEffect effect = _skillNameDisplayPool.GetFromPool(0, _skillNameDisplayPool.transform);
        effect.gameObject.SetActive(true);
        effect.InitEffect(color, name);
    }
    public void RefreshTimer(int time)
    {
        if(time>=0)
        {
            _timerText.text = string.Concat("00 : ", time.ToString().PadLeft(2, '0'));
            _timerText.color = Color.white;
        }
        else
        {
            _timerText.text = string.Concat("00 : ", (-time).ToString().PadLeft(2, '0'));
            _timerText.color = Color.red;
        }
    }
    public override void RefreshUI(bool isTarget)
    {
        base.RefreshUI(isTarget);

        if (isTarget == false) return;

        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(InitController.Instance.GamePlays.CurrentMapWayData.Keyword, out string[] value);
        LocalizeStringEvent ev = _currentLevelText.GetComponent<LocalizeStringEvent>();
        ev.StringReference.Arguments = value;
        ev.StringReference.SetReference("Equip", "Default");


        for (int i = 0; i < GetSkillUseElems.Length; i++)
        {
            GetSkillUseElems[i].RefreshUIElem();
        }

        _dropTableDisplay.InitDisplay();

    }
    public void OnBattleEnd()
    {
        foreach(var a in PlayerStateInfoDic.Values)
        {
            Destroy(a.gameObject);
        }
        PlayerStateInfoDic.Clear();
        foreach (var a in EnemyStateInfoDic.Values)
        {
            Destroy(a.gameObject);
        }
        EnemyStateInfoDic.Clear();
        foreach(Transform a in _textEffectParent)
        {
            a.gameObject.SetActive(false);
        }
        _timerText.text = string.Empty;
        _timerText.color = Color.white;
    }
    private Dictionary<ScriptableDebuff, StateInfo> PlayerStateInfoDic;
    private Dictionary<ScriptableDebuff, StateInfo> EnemyStateInfoDic;
    public void AddPlayerState(TimedDebuff debuff)
    {
        StateInfo info = Instantiate(Resources.Load<StateInfo>(Paths.StateInfo), GetObject((int)GameObjects.PlayerStateInfoContent).transform);
        info.InitStateInfo(debuff);
        PlayerStateInfoDic.Add(debuff.Debuff, info);
    }
    public void RemovePlayerState(TimedDebuff debuff)
    {
        Destroy(PlayerStateInfoDic[debuff.Debuff].gameObject);
        PlayerStateInfoDic.Remove(debuff.Debuff);
    }
    public void RefreshPlayerDebuff(TimedDebuff debuff)
    {
        PlayerStateInfoDic[debuff.Debuff].RefreshStateInfo(debuff);
    }
    public void AddEnemySate(TimedDebuff debuff)
    {
        StateInfo info = Instantiate(Resources.Load<StateInfo>(Paths.StateInfo), GetObject((int)GameObjects.EnemyStateInfoContent).transform);
        info.InitStateInfo(debuff);
        EnemyStateInfoDic.Add(debuff.Debuff, info);
    }
    public void RemoveEnemyState(TimedDebuff debuff)
    {
        Destroy(EnemyStateInfoDic[debuff.Debuff].gameObject);
        EnemyStateInfoDic.Remove(debuff.Debuff);
    }
    public void RefreshEnemyDebuff(TimedDebuff debuff)
    {
        EnemyStateInfoDic[debuff.Debuff].RefreshStateInfo(debuff);
    }

    public void SetLvNameText(string[] names)
    {
        if(InitController.Instance.Locales.CurrentLocaleID == eLanguage.Eng)
        {
            GetText((int)Texts.EnemyInfoText).text = $"LV {InitController.Instance.GamePlays.CurrentMapWayData.WayPointLevel} {names[0]}";
            GetText((int)Texts.PlayerInfoText).text = $"LV {InitController.Instance.GamePlays.IngameStat.CurrentLevel} Player";
        }
        else if(InitController.Instance.Locales.CurrentLocaleID == eLanguage.Kor)
        {
            GetText((int)Texts.EnemyInfoText).text = $"LV {InitController.Instance.GamePlays.CurrentMapWayData.WayPointLevel} {names[1]}";
            GetText((int)Texts.PlayerInfoText).text = $"LV {InitController.Instance.GamePlays.IngameStat.CurrentLevel} 플레이어";
        }
    }

    public void RefreshPlayerHP(float current, float shield, float max)
    {
        if (shield + current >= max)
        {
            _playerHPGauge.fillAmount = current / (max+shield);
            _playerShieldGauge.fillAmount = 1;
        }
        else
        {
            _playerHPGauge.fillAmount = current / max;
            _playerShieldGauge.fillAmount = (current+shield) /max;
        }

        if (current < 0) current = 0;
        if(shield == 0)
        {
            _playerHPText.text = $"{current:0}/{max:0}";
        }
        else
        {
            _playerHPText.text = $"{current:0}({shield:0})/{max:0}";
        }
    }

    public void RefreshPlayerMP(float current, float max)
    {
        _playerMPGauge.fillAmount = current / max;
        if (current < 0) current = 0;
        _playerMPText.text = $"{current:0}/{max:0}";
    }

    public void RefreshEnemyHP(float current, float max)
    {
        _enemyHPGauge.fillAmount = current / max;
        if (current < 0) current = 0;
        _enemyHPText.text = $"{current:0}/{max:0}";
    }

    public void AddtoDmgBoard(eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg)
    {
        _dmgBoard.AddText(target, keyword, dmgType, finaldmg.ToString("N0"));
    }
}
