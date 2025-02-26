using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Random = UnityEngine.Random;




public class MapUICanvas : UICanvas//, IPointerClickHandler
{
    private struct Threaten
    {
        public float BaseNum;
        public int Count;
        public Threaten(float baseNum)
        {
            BaseNum = baseNum;
            int index = 0;
            while(Mathf.Pow(BaseNum, index) <=100)
            {
                index++;
            }
            Count = index+1;
        }
    }

    #region 줌인아웃. 현재 안씀
    /*
    private RectTransform _worldViewPort;
    private float _touchWaitTic = 0.2f;
    float _clickTime = 0;
    private float _zoomSpeed = 5;
    private float _widthViewPort, _heightViewPort;
    private float _camToViewPortCoef;
    private bool _zoomActive;
    private bool _zoomIn= true;
    */
    #endregion
    private Camera _mainCam;
    [SerializeField]
    private Transform _wayPointParent;
    private List<MapWayPointDataEntity> _wayPointDataList;
    private List<MapIcon> _wayPointList;
    //private List<MapIcon> _activeWayPointList;
    private RectTransform _characterInMapRect;
    private RectTransform _characterSprite;
    private SpriteRenderer _weaponSprite;
    private Transform _content;
    private int _currentWayIndex;
    public int CurrentWayIndex { get { return _currentWayIndex; } }

    public MapIcon GetCurrentWayPoint { get; private set; }
    Action<int> OnMoveButton;
    private Image _currentThreatGauge;
    private Image _dayNightImage;
    private Image _symbolMove;
    private int _dayCount;
    private LocalizeStringEvent _currentStateText;

    private GamePlayController _playController;

    private Threaten _threaten;
    private float _threatenElement;// 초기값 3.3 위협도 감소 아이템 끼면 점점 낮아짐. 3차항
    public float ThreatenElemnt { get { return _threatenElement; } set { _threatenElement = (float)((-1E-06) * Mathf.Pow(value, 3) + 0.0004 * Mathf.Pow(value, 2) - 0.0483 * value + 3.2869f); } }
    private bool _isMoving = false;
    public bool IsMoving { get { return _isMoving; } }
    private Transform _dayNightRotateRoot;
    private readonly int _dayCountMax = 10; //밤->낮 까지 카운드 10 필요하다는 뜻
    private MapEffect _currentMapEffect;
    public MapEffect CurrentMapEffect => _currentMapEffect;
    //private MapEffect _mapEffectOnMap;
    //public List<MapEffect> _mapEffectOnMapList;
    private Dictionary<int, MapEffect> _mapEffectOnMapDic;
    public float Value;
    public int Duration;
    TextMeshProUGUI _effectInfoText;
    LocalizeStringEvent _infoEv;
    //private int _mapEffectCount;

    enum GameObjects
    {
        Content,
        CharacterInMap,
        CharacterSprite,
        MapOrder,
        InformImage,
        MapSizeSlier,
        Weapon,
        DayNightIcon,
        AdsPos,
        EffectPanel,
        MapEffectInfoButton,
        MapEffectInfoPage,
    }
    enum Images
    {
        NextThreatGauge,
        CurrentThreatGauge,
        DayNightImage,
        SymbolMove,
        EffectSymbol,
    }
    enum Texts
    {
        ThreatText,
        StaminaText,
        LvContentText,
        GoldText,
        CurrentStateText,
        EffectInfoText
    }
    enum Buttons
    {
        ShopButton,
        EquipButton,
        StatButton,
        SettingButton,
    }

    private void Awake()
    {
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        _content = GetObject((int)GameObjects.Content).transform;
        _currentThreatGauge = GetImage((int)Images.CurrentThreatGauge);
        _currentStateText = GetText((int)Texts.CurrentStateText).GetComponent<LocalizeStringEvent>();
        _currentStateText.StringReference.SetReference("UpperUI", "CurrentState_Idle");
        _mainCam = Camera.main;

        RectTransform rt = GetComponent<RectTransform>();
        float canvasHeight = rt.rect.height;
        float desiredCanvasWidth = canvasHeight * _mainCam.aspect;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredCanvasWidth);


        _dayNightRotateRoot = GetObject((int)GameObjects.DayNightIcon).transform.Find("RotateRoot").transform;
        float size = PlayerPrefs.GetFloat("MapSize", 1);
        SetSizeContent(size);
        Slider slider = GetObject((int)GameObjects.MapSizeSlier).GetComponent<Slider>();
        slider.value = _content.localScale.x;
        slider.onValueChanged.AddListener((float value) => SetSizeContent(value));

        _characterInMapRect = GetObject((int)GameObjects.CharacterInMap).GetComponent<RectTransform>();
        _characterSprite = GetObject((int)GameObjects.CharacterSprite).GetComponent<RectTransform>();
        _weaponSprite = GetObject((int)GameObjects.Weapon).GetComponent<SpriteRenderer>();

        _dayNightImage = GetImage((int)Images.DayNightImage);
        _dayCount = 0;
        _symbolMove = GetImage((int)Images.SymbolMove);
        _symbolMove.gameObject.SetActive(false);
        _touchBlcokPanel.GetComponent<UIEventHandler>().OnClickHandler = () => _isMoving = false;
        _effectInfoText = GetText((int)Texts.EffectInfoText);
        _infoEv = _effectInfoText.GetComponent<LocalizeStringEvent>();
        _mapEffectOnMapDic = new Dictionary<int, MapEffect>();

        GameObject infopage = GetObject((int)GameObjects.MapEffectInfoPage);
        infopage.SetActive(false);


        Action OnInfoClicked = () =>
        {
            infopage.SetActive(!infopage.activeSelf);
        };
        Action ToInfoClose = () => infopage.SetActive(false);
        infopage.GetComponent<UIEventHandler>().OnClickHandler = ToInfoClose;
        GetObject((int)GameObjects.MapEffectInfoButton).GetComponent<UIEventHandler>().OnClickHandler = OnInfoClicked;
        Action onMapClickAction = () =>
        {
            InitController.Instance.GamePlays.OnMapIconClick?.Invoke(null);
            ToInfoClose.Invoke();
        };
        _content.GetComponent<UIEventHandler>().OnClickHandler -= onMapClickAction;
        _content.GetComponent<UIEventHandler>().OnClickHandler += onMapClickAction;
    }
    float AdsHeight;
    Action OnRemoveAdsAction;
    RectTransform _informRect, _adsposRect;

    private void Start()
    {
        AdsHeight = _mainCam.GetComponent<GameCamera>().AdsHeight;
        _informRect = GetObject((int)GameObjects.InformImage).GetComponent<RectTransform>();
        _informRect.anchoredPosition = new Vector2(0, -AdsHeight);
        _adsposRect = GetObject((int)GameObjects.AdsPos).GetComponent<RectTransform>();
        _adsposRect.sizeDelta = new Vector2(_adsposRect.sizeDelta.x, AdsHeight + _informRect.sizeDelta.y);
        if (InitController.Instance.SaveDatas.UserData.GetIsAdsHide == true)
        {
            OnRemoveAds();
            return;
        }
    }
    void OnRemoveAds()
    {
        _informRect.anchoredPosition = Vector2.zero;
        _adsposRect.sizeDelta = new Vector2(_adsposRect.sizeDelta.x, _informRect.sizeDelta.y);
        //Debug.Log("-------------OnRemoveAds_MapUICanvas : " + adsposrect.sizeDelta);
    }
    private void OnEnable()
    {
        OnRemoveAdsAction = () => OnRemoveAds();
        if (InitController.Instance.SaveDatas.UserData.GetIsAdsHide == false)
        {
            InitController.Instance.SaveDatas.OnAdsRemove -= OnRemoveAdsAction;
            InitController.Instance.SaveDatas.OnAdsRemove += OnRemoveAdsAction;
        }
    }
    private void OnDisable()
    {
        float size = _content.localScale.x;
        PlayerPrefs.SetFloat("MapSize", size);

        if (InitController.Instance == null) return;
        InitController.Instance.SaveDatas.OnAdsRemove -= OnRemoveAdsAction;

        RefreshMapEffectText();
    }
    public override void InitUICanvas()
    {
        base.InitUICanvas();

        GetButton((int)Buttons.ShopButton).onClick.AddListener(() => {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            ShopUIPopup popup = InitController.Instance.UIs.OpenUIPopup<ShopUIPopup>("ShopUIPopup", InitController.Instance.UIs.transform);
            popup.InitUIPopup();
        });
        GetButton((int)Buttons.EquipButton).onClick.AddListener(() => {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            EquipUIPopup popup = InitController.Instance.UIs.OpenUIPopup<EquipUIPopup>("EquipUIPopup", InitController.Instance.UIs.transform);
            popup.InitUIPopup();
        });
        GetButton((int)Buttons.StatButton).onClick.AddListener(() => {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            StatUIPopup popup = InitController.Instance.UIs.OpenUIPopup<StatUIPopup>("StatUIPopup", InitController.Instance.UIs.transform);
            popup.InitUIPopup();
        });
        GetButton((int)Buttons.SettingButton).onClick.AddListener(() => {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            SettingUIPopup popup = InitController.Instance.UIs.OpenUIPopup<SettingUIPopup>("SettingUIPopup", InitController.Instance.UIs.transform);
            popup.InitUIPopup();
        });

        _playController = InitController.Instance.GamePlays;

        _playController.OnWeaponEquipped -= SetCharacterWeaponSprite;
        _playController.OnWeaponEquipped += SetCharacterWeaponSprite;
        _playController.OnWeaponUnEquipped -= SetCharacterWeaponSprite;
        _playController.OnWeaponUnEquipped += SetCharacterWeaponSprite;
        _playController.OnAccessoryEquipped -= SetThreatGaugeInfo;
        _playController.OnAccessoryEquipped += SetThreatGaugeInfo;
        _playController.OnAccessoryUnEquipped -= SetThreatGaugeInfo;
        _playController.OnAccessoryUnEquipped += SetThreatGaugeInfo;

        _playController.OnGoldUseOrGain -= (int gold) => GetText((int)Texts.GoldText).text = gold.ToString("N0");
        _playController.OnGoldUseOrGain += (int gold) => GetText((int)Texts.GoldText).text = gold.ToString("N0");
        _playController.OnLevelUP -= (int level) => GetText((int)Texts.LvContentText).text = level.ToString();
        _playController.OnLevelUP += (int level) => GetText((int)Texts.LvContentText).text = level.ToString();
        _playController.OnStaminaUse -= (int stamina) => GetText((int)Texts.StaminaText).text = stamina.ToString();
        _playController.OnStaminaUse += (int stamina) => GetText((int)Texts.StaminaText).text = stamina.ToString();
        _playController.OnAccessoryUnEquipped -= (AccessoryDataEntity dn) => GetText((int)Texts.StaminaText).text = Mathf.Max(0, _playController.IngameStat.GetTotalStat(eStatInfo.Stamina)).ToString("N0");
        _playController.OnAccessoryUnEquipped += (AccessoryDataEntity dn) => GetText((int)Texts.StaminaText).text = Mathf.Max(0, _playController.IngameStat.GetTotalStat(eStatInfo.Stamina)).ToString("N0");
        _playController.OnAccessoryEquipped -= (AccessoryDataEntity dn) => GetText((int)Texts.StaminaText).text = Mathf.Max(0, _playController.IngameStat.GetTotalStat(eStatInfo.Stamina)).ToString("N0");
        _playController.OnAccessoryEquipped += (AccessoryDataEntity dn) => GetText((int)Texts.StaminaText).text = Mathf.Max(0, _playController.IngameStat.GetTotalStat(eStatInfo.Stamina)).ToString("N0");



        GetText((int)Texts.GoldText).text = InitController.Instance.GamePlays.IngameStat.CurrentGold.ToString("N0");
        GetText((int)Texts.LvContentText).text = InitController.Instance.GamePlays.IngameStat.CurrentLevel.ToString();
        GetText((int)Texts.StaminaText).text = Mathf.Max(0, _playController.IngameStat.GetTotalStat(eStatInfo.Stamina)).ToString("N0");

        SetCharacterWeaponSprite(InitController.Instance.SaveDatas.UserData.GetPresetDatas[InitController.Instance.SaveDatas.UserData.CurrentPrestIndex]._currentWeapon);

        float stat = InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.Threat);
        ThreatenElemnt = stat;
        _threaten = new Threaten(ThreatenElemnt);

        _dayCount = InitController.Instance.GamePlays.IngameStat.DayCount;
        _dayNightImage.color = DayColor(_dayCount);



        _dayNightRotateRoot.rotation = Quaternion.Euler(0, 0, -90 -18 * _dayCount);
        string map = "Map";
        GetObject((int)GameObjects.DayNightIcon).GetComponent<Canvas>().sortingLayerName = map;
        GetObject((int)GameObjects.MapOrder).GetComponent<Canvas>().sortingLayerName = map;
    }

    public void SetSizeContent(float size)
    {
        size = Mathf.Clamp(size, 0.6f, 1f);
        _content.localScale = Vector3.one * size;
    }
    /// <summary>
    /// 맵 이동 시 사용. mapicon 일괄 삭제 / 생성.
    /// </summary>
    /// <param name="mapName"></param>
    /// <param name="startIndex"></param>
    public void InitMapData(string mapName, int startIndex)
    {        
        _currentWayIndex = startIndex;


        if (_wayPointList!=null)
        {
            foreach(var a in _wayPointList)
            {
                Destroy(a.gameObject);
            }
            _wayPointList.Clear();
        }
        Debug.Log(mapName);
        GetObject((int)GameObjects.Content).GetComponent<Image>().sprite = InitController.Instance.GameDatas.MapSpriteDic[mapName];
        MapIcon _mapIconPrefab = Resources.Load<MapIcon>(Paths.MapIcon);
        _wayPointList = new List<MapIcon>();
        //_activeWayPointList = new List<MapIcon>();

        _wayPointDataList = InitController.Instance.GamePlays.CurrentMapWayPointDataEntityList;
        foreach (MapWayPointDataEntity a in _wayPointDataList)
        {
            MapIcon icon = Instantiate(_mapIconPrefab, _wayPointParent);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector3(a.MapPosX, a.MapPosY, 0);
            OnMoveButton = MoveToReachableWayPoint;
            icon.InitMapIcon(a, OnMoveButton, this);
            _wayPointList.Add(icon);
        }



        GetCurrentWayPoint = _wayPointList[_currentWayIndex];
        SetCharacterPosOnMap(_currentWayIndex);
        _characterInMapRect.anchoredPosition = new Vector2(_wayPointDataList[_currentWayIndex].MapPosX, _wayPointDataList[_currentWayIndex].MapPosY);


        InitController.Instance.GamePlays.CurrentMapWayData = _wayPointDataList[_currentWayIndex];

        _currentMapEffect = null;
        _mapEffectOnMapDic.Clear();

        //_mapEffectCount = 0;
        //foreach (var a in _wayPointDataList)
        //{
        //    if (a.WayPointType == WayPointType.Boss || a.WayPointType == WayPointType.WayPoint)
        //    {
        //        continue;
        //    }
        //    _mapEffectCount++;
        //}
        //_mapEffectCount = (_mapEffectCount - 1) / 3;

        GenerateMapEffect(_currentWayIndex);
        //==========================================================
    }


    private void Update()
    {
        float currentfillAmount = _playController.IngameStat.EncounterCount / (float)_threaten.Count;
        _currentThreatGauge.fillAmount = currentfillAmount * Random.Range(0.97f, 1.03f);
    }
    public override void RefreshUI(bool isTarget)
    {
        base.RefreshUI(isTarget);
        SetCharacterPosOnMap(_currentWayIndex);
    }
    public void SetThreatGaugeInfo(AccessoryDataEntity data)
    {
        if (data == null) return;
        
        if(data.BaseStat == eStatInfo.Threat)
        {
            float stat = InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.Threat);
            ThreatenElemnt = stat;
            _threaten = new Threaten(ThreatenElemnt);
        }
    }
    public void SetCharacterWeaponSprite(WeaponDataEntity weapon)
    {
        if (weapon == null)
        {
            _weaponSprite.sprite = null;
        }
        else
        {
            _weaponSprite.sprite = InitController.Instance.GameDatas.AllItemSpriteDic[weapon.Index.ToString()];//Resources.Load<Sprite>(Paths.WeaponSpriteData + "/" + weapon.Index);
            if (weapon.WeaponSize == eWeaponSize.Large)
                _weaponSprite.transform.localScale = new Vector3(2, 2, 1);
            else
                _weaponSprite.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
    }
    public void SetCharacterPosOnMap(int currentWayIndex)
    {
        float posX = - _wayPointDataList[currentWayIndex].MapPosX;
        float posY = - _wayPointDataList[currentWayIndex].MapPosY;
        _content.localPosition = new Vector2(posX, posY);
    }

    public void MoveToReachableWayPoint(int dest)
    {
        WayNod[] nodes = new WayNod[_wayPointList.Count];
        for (int i = 0; i < nodes.Length; i++)
        {
            List<int> test = Array.ConvertAll(_wayPointDataList[i].LinkedWayPoint.Split(','), int.Parse).ToList();
            nodes[i] = new WayNod(i, test);
        }
        StartCoroutine(go());
        IEnumerator go()
        {
            int[] ways = Astar.CalcWays(_currentWayIndex, dest, nodes);
            int index = 0;
            _isMoving = true;
            _characterSprite.GetComponent<Animator>().Play("Run");
            _currentStateText.StringReference.SetReference("UpperUI", "CurrentState_Move");
            InitController.Instance.Sounds.PlaySFXLoop(eSFX.Walk);

            while (index < ways.Length-1 && _isMoving)
            {
                int current = ways[index];
                index++;
                int next = ways[index];
                InitController.Instance.Sounds.PlaySFXLoop(eSFX.Walk);

                if (index >= ways.Length - 1) _isMoving = false;
                yield return (CharacterMoving(current, next));
            }
            //ActiveMapEffect(ways[index]);
            _characterSprite.GetComponent<Animator>().Play("Idle");
            _currentStateText.StringReference.SetReference("UpperUI", "CurrentState_Idle");
            InitController.Instance.Sounds.StopSFX(eSFX.Walk);
        }


    }
    IEnumerator CharacterMoving(int current, int next)
    {
        TouchBlockPanelOnOff(true);
        _wayPointList[current].ActiveWayPoint(false); //HACK 현재 발판 비활성화


        _currentWayIndex = next;
        Vector2 destpos = new Vector2(_wayPointDataList[next].MapPosX, _wayPointDataList[next].MapPosY);
        if(destpos.x < _characterInMapRect.anchoredPosition.x)
        {
            _characterSprite.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _characterSprite.localScale = new Vector3(1, 1, 1);
        }
        float distance = Vector2.Distance(destpos, _characterInMapRect.anchoredPosition);
        //float distance2 = Vector2.Distance(-1 * destpos, _content.localPosition);
        float proc = 0.5f;

        _dayCount++;
        InitController.Instance.GamePlays.IngameStat.DayCount = _dayCount;
        Color daycolor = DayColor(_dayCount);
        Quaternion nextrotation = Quaternion.Euler(0, 0, -90 -18 * _dayCount);

        while (proc>=0)
        {
            proc -= Time.deltaTime;
            _characterInMapRect.anchoredPosition = Vector2.MoveTowards(_characterInMapRect.anchoredPosition, destpos, distance*Time.deltaTime *2);
            _content.localPosition = Vector2.Lerp(_content.localPosition, destpos * (-1), Time.deltaTime);

            _dayNightImage.color = Color.Lerp(_dayNightImage.color, daycolor, 0.5f - proc);
            _dayNightRotateRoot.rotation = Quaternion.Lerp(_dayNightRotateRoot.rotation, nextrotation, 0.5f - proc);
           yield return null;
        }
        _characterInMapRect.anchoredPosition = destpos;

        if(_isMoving == false)
        {
            ActiveMapEffect(next);
        }

        ActivateSelectWayPoint(next);
        _wayPointList[next].ActiveWayPoint(true);//HACK 목표발판 활성화 이거는 위에 activateselcetwaypoint에 합쳐도 될거같음. 마무리할때 테스트 
        TouchBlockPanelOnOff(false);
    }

    void ActivateSelectWayPoint(int pointIndex)
    {
        InitController.Instance.GamePlays.CurrentMapWayData = _wayPointDataList[pointIndex];
        int encounterCount = _playController.IngameStat.EncounterCount;
        if (_wayPointDataList[pointIndex].WayPointType == WayPointType.WayPoint)// 목적지가 웨이포인트일 경우 인카운터 체크 안함
        {
            return;
        }
        else
        {
            //ActiveMapEffect(pointIndex);

            float ForcedEncounterProbability = Mathf.Clamp(Mathf.Pow(ThreatenElemnt, encounterCount), 0, 100);

            if (Random.Range(10, 100f) <= ForcedEncounterProbability) //TODO HACK 확률이 10 미만인 경우는 패스. 너무 낮을때 만나면 좀... 
            {
                _isMoving = false;
                int current = (int)InitController.Instance.SaveDatas.UserData.IngameStatData.GetTotalStat(eStatInfo.Stamina);
                if (current > 0)
                {
                    InitController.Instance.GamePlays.MoveToBattleMap(_wayPointDataList[pointIndex], _currentMapEffect);
                }
            }
            else
            {
                _playController.IngameStat.EncounterCount++;
            }
        }
    }
    Coroutine _effectMoveRout;
    void ActiveMapEffect(int index)
    {
        MapEffect temp = _wayPointList[index].CurrentMapEffect;
        if (temp != null)
        {
            _currentMapEffect = temp;
            _wayPointList[index].CurrentMapEffect = null;
            _mapEffectOnMapDic.Remove(index);
        }
        else return;

        Debug.Log(_currentMapEffect.EffectType + " / " + _currentMapEffect.EffectValue + " / " + _currentMapEffect.Duration);
        if (_currentMapEffect != null)
        {
            Vector2 pos = _mainCam.WorldToScreenPoint(_wayPointList[index].transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetObject((int)GameObjects.MapOrder).GetComponent<RectTransform>(), pos, _mainCam, out Vector2 localpos);

            _symbolMove.rectTransform.anchoredPosition = localpos;

            Vector2 targetpos = _mainCam.WorldToScreenPoint(GetImage((int)Images.EffectSymbol).transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetObject((int)GameObjects.MapOrder).GetComponent<RectTransform>(), targetpos, _mainCam, out Vector2 targetlocalpos);
            if (_effectMoveRout != null) StopCoroutine(_effectMoveRout);
            _effectMoveRout = null;
            _effectMoveRout = StartCoroutine(move());
            
            IEnumerator move()
            {
                _symbolMove.gameObject.SetActive(true);
                Vector2 velocity = Vector2.zero;
                bool arrived = false;
                while (arrived == false)
                {
                    _symbolMove.rectTransform.anchoredPosition = Vector2.SmoothDamp(_symbolMove.rectTransform.anchoredPosition, targetlocalpos, ref velocity, 0.3f);
                    if (Vector2.Distance(_symbolMove.rectTransform.anchoredPosition, targetlocalpos) < 20)
                    {
                        arrived = true;
                        RefreshMapEffectText();
                    }
                    yield return null;
                }
            }
        }
        else
        {
            RefreshMapEffectText();
        }
    }
    void GenerateMapEffect(int index)
    {
        int count = (int)InitController.Instance.SaveDatas.UserData.SoulEnchantStatDic[Constants.Soul_Scout].CurrentValue + Nums.BaseMapEffectCount;
        if (_mapEffectOnMapDic.Count >= count) return;

        List<int> indexlist = new List<int>();
        foreach(var a in _wayPointDataList)
        {
            if (a.Index == index || a.WayPointType == WayPointType.Boss || a.WayPointType == WayPointType.WayPoint)
            {
                continue;
            }
            indexlist.Add(a.Index);
        }

        for(int i = indexlist.Count-1; i>=0; i--)
        {
            int tempindex = indexlist[i];
            if (_mapEffectOnMapDic.ContainsKey(tempindex)) indexlist.Remove(tempindex);
        }
        while (_mapEffectOnMapDic.Count < count)
        {

            int randindex = Random.Range(0, indexlist.Count);
            //Debug.Log(randindex + "/" + indexlist[randindex] + "/" + indexlist.Count);

            MapEffect effect = new MapEffect(true);
            _wayPointList[indexlist[randindex]].CurrentMapEffect = effect;

            _mapEffectOnMapDic.Add(indexlist[randindex], _wayPointList[indexlist[randindex]].CurrentMapEffect);
            indexlist.Remove(indexlist[randindex]);
        }
    }
    public void AfterBattleAdjustMapEffect()
    {
        Debug.Log("전투 후 맵 복귀");
        InitController.Instance.GamePlays.OnMapIconClick?.Invoke(null);
        if (_currentMapEffect!=null)
        {
            _currentMapEffect.Duration -= 1;
            RefreshMapEffectText();
        }
        else
        {
            _infoEv.StringReference.SetReference("UpperUI", "MapEffect");
            _infoEv.RefreshString();
        }
        GenerateMapEffect(_currentWayIndex);
    }
    void RefreshMapEffectText()
    {
        if (_currentMapEffect == null)
        {
            _infoEv.StringReference.SetReference("UpperUI", "MapEffect");
            _infoEv.RefreshString();
            return;
        }

        Duration = _currentMapEffect.Duration;
        Value = _currentMapEffect.EffectValue;

        if(Duration<= 0)
        {
            _currentMapEffect = null;
            _infoEv.StringReference.SetReference("UpperUI", "MapEffect");
        }
        else
        {
            switch (_currentMapEffect.EffectType)
            {
                case eMapEffect.None:
                    break;
                case eMapEffect.HPDecrease:
                    _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_DecreaseHP);
                    break;
                case eMapEffect.MoneyIncrease:
                    _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseGold);
                    break;
                case eMapEffect.EXPIncrease:
                    _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseExp);
                    break;
                case eMapEffect.DropIncrease:
                    _infoEv.StringReference.SetReference("UIs", Constants.MapEffect_IncreaseFind);
                    break;
                case eMapEffect.Labyrinth:
                    break;
                default:
                    break;
            }
        }
        _infoEv.RefreshString();
        _effectInfoText.rectTransform.sizeDelta = new Vector2(_effectInfoText.preferredWidth + 30, _effectInfoText.rectTransform.sizeDelta.y);
        _symbolMove.gameObject.SetActive(false);
    }
    private Color DayColor(int daycount)
    {
        int updown = (daycount) / _dayCountMax;
        float daynight = Mathf.Pow(-1, updown) * ((daycount) % _dayCountMax) + _dayCountMax * (updown % 2);
        float daynightcoef = (1f / _dayCountMax);
        return new Color(0, 0, 0, Mathf.Clamp(daynight * daynightcoef, 0.2f, 0.8f));
    }

    /* 맵 줌인아웃. 현재 사용안함. 필요가 없음...
    private void Update()
    {
        CanvasWorldZoomUpdate();
    }
    public void ZoomSetting()
    {
        _widthViewPort = _worldViewPort.sizeDelta.x;
        _heightViewPort = _worldViewPort.sizeDelta.y;
        _camToViewPortCoef = (Nums.CamSizeMax - Nums.CamSizeMin) * 2;
    }
    private void ResetZoomSetting()
    {
        _mainCam.orthographicSize = Nums.CamSizeMax;
        ProperCamsize = Nums.CamSizeMax;
        _worldViewPort.sizeDelta = new Vector2(_widthViewPort, _heightViewPort);
        _zoomActive = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPress.CompareTag("MapUICanvas"))
        {
            Debug.Log("맵유아이");
            if (Time.time - _clickTime <= _touchWaitTic)
            {
                Debug.Log("0.2초내에 더블클릭");
                _zoomActive = true;
                if (_mainCam.orthographicSize >= Nums.CamSizeMax)
                {
                    _zoomIn = false;
                }
                else if (_mainCam.orthographicSize <= Nums.CamSizeMin)
                {
                    _zoomIn = true;
                }
            }
            else
            {
                Debug.Log("그냥클릭");
                _clickTime = Time.time;
            }
        }
    }

    public void CanvasWorldZoomUpdate()
    {
        if (_zoomActive)
        {
            Debug.Log(_zoomActive);
            if (_zoomIn)
            {
                _mainCam.orthographicSize += Time.deltaTime * _zoomSpeed;
                _worldViewPort.sizeDelta += new Vector2(_widthViewPort / _camToViewPortCoef, _heightViewPort / _camToViewPortCoef) * Time.deltaTime * _zoomSpeed;
                if (_mainCam.orthographicSize >= Nums.CamSizeMax)
                {
                    _mainCam.orthographicSize = Nums.CamSizeMax;
                    ProperCamsize = Nums.CamSizeMax;
                    _worldViewPort.sizeDelta = new Vector2(_widthViewPort, _heightViewPort);
                    _zoomActive = false;
                }

            }
            else
            {
                _mainCam.orthographicSize -= Time.deltaTime * _zoomSpeed;
                _worldViewPort.sizeDelta -= new Vector2(_widthViewPort / _camToViewPortCoef, _heightViewPort / _camToViewPortCoef) * Time.deltaTime * _zoomSpeed;

                if (_mainCam.orthographicSize <= Nums.CamSizeMin)
                {
                    _mainCam.orthographicSize = Nums.CamSizeMin;
                    ProperCamsize = Nums.CamSizeMin;
                    _worldViewPort.sizeDelta = new Vector2(_widthViewPort * 0.5f, _heightViewPort * 0.5f);

                    _zoomActive = false;
                }
            }
        }
    }  //TODO 하다보니.. 현재 맵 크기에서는 굳이 줌이 필요가 없는걸....? 이걸 왜 하루 종일 만들고있었나...!ㅋㅋㅋ
    */
}
