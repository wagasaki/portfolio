using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;


public class GamePlayController : SerializedMonoBehaviour
{
    private bool _isReady = false;
    private Enemy _enemyPrefab;
    
    //private UserCharacter _userCharacter;
    public Vector2 UserCharacterPos { get { return new Vector2(-1.34f, -1.4f); } }
    public Vector2 EnemyPos { get { return new Vector2(1.3f, -1.47f); } }
    [SerializeField]
    private DropItem _dropItemPrefab;

    #region character
    //private UserCharacter _currentCharacter;
    //private List<Enemy> _currentEnemyList;
    

    #endregion



    private Coroutine _stageEndRoutine;

    private MapWayPointDataEntity _currentMapWayData;
    public MapWayPointDataEntity CurrentMapWayData {
        get
        {
            return _currentMapWayData;
        }
        set
        {
            IngameStat.CurrentMapWayIndex = value.Index;
            _currentMapWayData = value;
        }
    }
    public bool IsCurrentWayPointCleared { 
        get 
        {
            Debug.Log(IngameStat.CurrentMapName + " ? " + CurrentMapWayData.Index);
            return IngameStat.IsMapClearedInfoDic[IngameStat.CurrentMapName][CurrentMapWayData.Index]; 
        } 
        private set 
        { 
            IngameStat.IsMapClearedInfoDic[IngameStat.CurrentMapName][CurrentMapWayData.Index] = value; 
        } 
    }

    public UserCharacter GetUserCharacter { get; set; }
    public PlayerSkillController GetSkillController { get; private set; }
    /// <summary>
    /// 입장 후 한 세션 내에셔만 사용하는 임시 스탯. 영구 저장되는 스탯은 savedata.userdata로 저장
    /// </summary>
    public IngameStatData IngameStat { get; private set; }
    public SkillDataEntity[] SkillDatas { get; set; }

    public List<MapWayPointDataEntity> CurrentMapWayPointDataEntityList { get; set; } 
    public List<Enemy> CurrentEnemyList { get; private set; }
    public List<Enemy> KilledEnemyList { get; private set; }
    public bool IsBattle { get; private set; }

    private DropItem _dropitem;

    //private Action _onBattleEnd;
    public Action OnBattleEnd { get; set; }
    public Action<MapIcon> OnMapIconClick { get; set; }
    

    public DamageTextEffectPool GetTextEffectPool { get; set; }

    public Action OnItemOwnedStatChanged { get; set; }
    public bool InitGamePlayController(bool isNewGame)
    {
        if (_isReady) return _isReady;
        Debug.Log("GamePlayController Init");

        UserData data = InitController.Instance.SaveDatas.UserData;
        if (isNewGame)
        {
            data.IngameStatData = new IngameStatData();
            data.IngameStatData.NewInagmeStatData();
            IngameStat = InitController.Instance.SaveDatas.UserData.IngameStatData;
        }
        else
        {
            IngameStat = data.IngameStatData;
        }
        Debug.Log(isNewGame + "/ " + IngameStat);
        if(string.IsNullOrEmpty(IngameStat.CurrentMapName))
        {
            IngameStat.CurrentMapName = Constants.FirstMap;
            IngameStat.CurrentMapWayIndex = 0;
        }
        CurrentMapWayPointDataEntityList = InitController.Instance.GameDatas.MapWayPointDataEntityDic[IngameStat.CurrentMapName];
        CurrentMapWayData = CurrentMapWayPointDataEntityList[IngameStat.CurrentMapWayIndex];

        Action OnLoaded = () => Loaded();
        InitController.Instance.OnAfterAllControllerLoaded -= OnLoaded;
        InitController.Instance.OnAfterAllControllerLoaded += OnLoaded;


        void Loaded()
        {
            if(data.IsTutorialCleared)
            {
                Debug.Log(IngameStat.CurrentMapName);
                InitController.Instance.UIs.MapNameTextEffect(IngameStat.CurrentMapName);
            }
            else
            {
                InitController.Instance.UIs.OpenUIPopup<TutorialUIPopup>(null, InitController.Instance.UIs.transform).InitUIPopup();
                Debug.Log("tutorial popup");
            }
        }

        IngameStat.ClearDataOnLoad();

        WeaponDataEntity weaponData = data.GetPresetDatas[data.CurrentPrestIndex]._currentWeapon;
        if (weaponData != null)
        {
            ApplyStatOnWeaponEquip(weaponData);
        }

        ArmorDataEntity armorData = data.GetPresetDatas[data.CurrentPrestIndex]._currentArmor;
        if (armorData != null)
        {
            ApplyStatOnArmorEquip(armorData.BaseValue, armorData.SecondStatValue);
        }

        AccessoryDataEntity[] accessoryDatas = data.GetPresetDatas[data.CurrentPrestIndex]._currentAccessory;
        for (int i = 0; i < accessoryDatas.Length; i++)
        {
            if (accessoryDatas[i] != null)
            {
                ApplyAccessoryStatOnEquip(accessoryDatas[i]);
            }
        }
        //----------------
        //_enemyPosArr = new Vector2[Nums.MaxEnemyCountColumn * Nums.MaxEnemyCountRow];
        //for(int i = 0; i< Nums.MaxEnemyCountRow; i++)
        //{
        //    for(int j = 0; j<Nums.MaxEnemyCountColumn; j++)
        //    {
        //        _enemyPosArr[i * Nums.MaxEnemyCountColumn + j] = new Vector2(0.2f + i * 0.8f, -0.2f - j * 1.1f);
        //    }
        //}
        //---------Enemy-----------------
        _enemyPrefab = Resources.Load<Enemy>(Paths.Enemy);
        //--------------------
        GetTextEffectPool = Instantiate(Resources.Load<DamageTextEffectPool>(Paths.DamageTextEffectPool));
        GetTextEffectPool.SetPrefab(Resources.Load<DamageTextEffect>(Paths.DmgTextEffect));
        GetTextEffectPool.InitPool();
        //Instantiate(Resources.Load<TextEffect>(Paths.DmgTextEffect));
        //--------------------
        GetSkillController = Instantiate(Resources.Load<PlayerSkillController>(Paths.SkillController));
        GetSkillController.InitPool();
        GetSkillController.gameObject.SetActive(false);
        IsBattle = false;



        CurrentEnemyList = new List<Enemy>();
        KilledEnemyList = new List<Enemy>();
        //-------------------------------------------------------------
        OnAccessoryUnEquipped -=(AccessoryDataEntity dn)=> CheckStamina(false);
        OnAccessoryUnEquipped += (AccessoryDataEntity dn) => CheckStamina(false);


        //===============================================================

        Debug.Log(IngameStat.GetTotalStat(eStatInfo.Gold));

        _isReady = true;
        return _isReady;
    }
    private MapEffect _mapEffect;
    public void MoveToBattleMap(MapWayPointDataEntity dataEntity, MapEffect effect = null)
    {
        if (InitController.Instance.GamePlays.StaminaAddictive(Nums.BaseBattleStaminaCost))
        {
            InitController.Instance.UIs.CloseAllUIPopup();
            InitController.Instance.GamePlays.CurrentMapWayData = dataEntity;
            float playerHPModifier = 1;
            float enemyHPModifier = 1;
            _mapEffect = effect;
            if(_mapEffect!=null && _mapEffect.EffectType == eMapEffect.HPDecrease)
            {
                enemyHPModifier = _mapEffect.EffectValue;
            }

            InitController.Instance.GamePlays.SetBattle(playerHPModifier, enemyHPModifier);
            IngameStat.EncounterCount = 0;



            Action afterFade = () =>
            {
                StartBattle();
            };

            InitController.Instance.UIs.LoadingPanel(PermanantUI.MapUI, PermanantUI.BattleUI, null, afterFade);
            InitController.Instance.UIs.GetBattlelUICanvas.InstantiateBattleMap(dataEntity.BattleMap);
            InitController.Instance.Sounds.PlayBGM(Constants.Battle, eBackSound.None);
        }
    }
    public void MoveToOtherMap(MapWayPointDataEntity mapdata)
    {
        OnMapIconClick = null;
        IngameStat.CurrentMapName = mapdata.WayPointTo;

        Action DuringFadeAction = () => {
            CurrentMapWayPointDataEntityList = InitController.Instance.GameDatas.MapWayPointDataEntityDic[IngameStat.CurrentMapName];
            CurrentMapWayData = CurrentMapWayPointDataEntityList[mapdata.WayPointToIndex];
            InitController.Instance.UIs.GetMapUICanvas.InitMapData(mapdata.WayPointTo, mapdata.WayPointToIndex);
        };


        Action AfterFadeAction = () =>
        {
            InitController.Instance.UIs.MapNameTextEffect(IngameStat.CurrentMapName);
            InitController.Instance.Sounds.PlayBGM(Constants.Main, InitController.Instance.GamePlays.CurrentMapWayData.BackSound);
        };

        InitController.Instance.UIs.LoadingPanel(PermanantUI.MapUI, PermanantUI.MapUI, DuringFadeAction, AfterFadeAction);
    }
    
    public void SetBattle(float playerHPModifier, float enemyHpModifier)
    {
        if (GetUserCharacter != null)
        {
            //Debug.Log(GetUserCharacter);
            if(GetUserCharacter.gameObject.activeInHierarchy)
            {
                Destroy(GetUserCharacter.gameObject);
            }
            
        }
        if (CurrentEnemyList != null) 
        {
            foreach (var a in CurrentEnemyList)
            {
                Destroy(a.gameObject);
            }
            CurrentEnemyList.Clear();
        }
        CharacterInit(playerHPModifier);

        EnemyInit(enemyHpModifier);


        _dropitem?.gameObject.SetActive(false);
        GetSkillController.gameObject.SetActive(true);
        GetSkillController.SetSkillPrefabPool();
    }
    public void StartBattle()
    {
        if (!IsBattle)
        {
            IsBattle = true;
            //_userCharacter.
            GetUserCharacter.StartBattle();
            //enemy 공격시작
            foreach (Enemy enemy in CurrentEnemyList)
            {
                enemy.StartBattle();
            }
            StartCoroutine(TimeAttack());
            IEnumerator TimeAttack()
            {
                int time = 30;
                int index = 0;
                int term = 0;
                string timeAttack = "TimeAttack";
                InitController.Instance.UIs.GetBattlelUICanvas.RefreshTimer(time);
                while (IsBattle)
                {
                    yield return YieldCache.WaitForSeconds(1f);
                    InitController.Instance.UIs.GetBattlelUICanvas.RefreshTimer(time);
                    time--;

                    if(time<= term)
                    {
                        string stage = string.Concat(timeAttack, index.ToString());
                        GetUserCharacter.CurrentBattleStat.ModifyStat(eStatInfo.Dmg, stage, 2^index);
                        foreach (Enemy enemy in CurrentEnemyList)
                        {
                            enemy.CurrentBattleStat.ModifyStat(eStatInfo.Dmg, stage, 2^index);
                        }
                        index++;
                        term-=5;
                    }
                }
            }

            int speed = InitController.Instance.SaveDatas.UserData.SpeedModifier;
            speed = Mathf.Min(3, Mathf.Max(1, speed));
            InitController.Instance.SetTimeScale(speed);
        }
    }
    public void CharacterInit(float hpmodifier)
    {
        UserCharacter prefab = Resources.Load<UserCharacter>(Paths.UserCharacter);
        GetUserCharacter = null;
        GetUserCharacter = Instantiate(prefab);
        GetUserCharacter.transform.position = UserCharacterPos;

        GetUserCharacter.OnHPChange_Display -= (float current, float shield, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerHP(current,shield, max);
        GetUserCharacter.OnHPChange_Display += (float current, float shield, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerHP(current, shield, max);
        GetUserCharacter.OnMPChange_Display -= (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerMP(current, max);
        GetUserCharacter.OnMPChange_Display += (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerMP(current, max);
        GetUserCharacter.OnDamage_Display -= (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);
        GetUserCharacter.OnDamage_Display += (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);



        GetUserCharacter.InitUnit(IngameStat.Clone(), hpmodifier);
        SkillDatas = new SkillDataEntity[Nums.SkillStoneCount];

        SkillStoneDataEntity[] skillstone = InitController.Instance.SaveDatas.UserData.GetPresetDatas[InitController.Instance.SaveDatas.UserData.CurrentPrestIndex]._currentSkillStone;
        for(int i = 0; i< skillstone.Length;i++)
        {
            if(skillstone[i]!=null)
            {
                SkillDatas[i] = skillstone[i].SkillDataEntity;
            }
        }

        WeaponDataEntity currentWeapon = InitController.Instance.SaveDatas.UserData.GetPresetDatas[InitController.Instance.SaveDatas.UserData.CurrentPrestIndex]._currentWeapon;
        if (currentWeapon == null)
        {
            GetUserCharacter.WeaponSpriteRenderer.sprite = null;
        }
        else
        {
            GetUserCharacter.WeaponSpriteRenderer.sprite = Resources.Load<Sprite>(Paths.WeaponSpriteData + "/" + currentWeapon.Index);
            if (currentWeapon.WeaponSize == eWeaponSize.Large)
                GetUserCharacter.WeaponSpriteRenderer.transform.localScale = new Vector3(2, 2, 1);
            else
                GetUserCharacter.WeaponSpriteRenderer.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }


        Debug.Log("UserCharacter Init");
        //TODO 캐릭터 스텟 데이터 세팅
    }
    public void EnemyInit(float hpmodifier)
    {
        if (CurrentEnemyList != null && CurrentEnemyList.Count > 0)
        {
            for (int i = 0; i < CurrentEnemyList.Count; i++)
            {
                Destroy(CurrentEnemyList[i]?.gameObject);
            }
        }

        EnemyDataEntity enemyData = InitController.Instance.GameDatas.EnemyDataDic[CurrentMapWayData.EnemyType][CurrentMapWayData.EnemyIndex];

        Enemy enemy = Instantiate(_enemyPrefab, new Vector2(1.3f, -1.47f), Quaternion.identity, transform);
        CurrentEnemyList.Add(enemy);
        //enemy.transform.position = new Vector2(1.3f, -1.47f);//_enemyPosArr[selectedPos[i]];
        enemy.OnHPChange_Display -= (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyHP(current, max);
        enemy.OnHPChange_Display += (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyHP(current, max);
        enemy.OnDamage_Display -= (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);
        enemy.OnDamage_Display += (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);
        enemy.InitUnit(enemyData, hpmodifier);
        Debug.Log(enemyData.Keyword);
        string[] enemyname = InitController.Instance.GameDatas.LocalizationDataDic[enemyData.Keyword];
        InitController.Instance.UIs.GetBattlelUICanvas.SetLvNameText(enemyname);
    }
    public void EnemyDie(Enemy enemy)
    {
        if (CurrentEnemyList.Count > 0)
        {
            enemy.OnHPChange_Display -= (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyHP(current, max);
            enemy.OnDamage_Display -= (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);
            
            CurrentEnemyList.Remove(enemy);
            KilledEnemyList.Add(enemy);
            //TODO 보상 관련 처리


            //TODO 스테이지 종료 처리
            if (CurrentEnemyList.Count <= 0)
            {
                if (_stageEndRoutine != null)
                {
                    StopCoroutine(_stageEndRoutine);
                    _stageEndRoutine = null;
                }
                _stageEndRoutine = StartCoroutine(StageEnd(true));
            }
        }
    }
    public void PlayerDie(UserCharacter player)
    {
        Debug.Log("유저 죽음");
        player.OnHPChange_Display -= (float current, float shield, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerHP(current, shield, max);
        player.OnMPChange_Display -= (float current, float max) => InitController.Instance.UIs.GetBattlelUICanvas.RefreshPlayerMP(current, max);
        player.OnDamage_Display -= (eAbsoluteTarget target, string keyword, eDamageType dmgType, float finaldmg) => InitController.Instance.UIs.GetBattlelUICanvas.AddtoDmgBoard(target, keyword, dmgType, finaldmg);
        
        if (_stageEndRoutine != null)
        {
            StopCoroutine(_stageEndRoutine);
            _stageEndRoutine = null;
        }
        _stageEndRoutine = StartCoroutine(StageEnd(false));
    }
    private IEnumerator StageEnd(bool isPlayerWin)
    {
        // TODO 결과창 UI 호출 이제 만들기 시작합시닷
        IsBattle = false;

        Dictionary<string, float> playerdmgrecord = new Dictionary<string, float>();
        Dictionary<string, float> enemydmgrecord = new Dictionary<string, float>();

        // 양쪽 모두 어택 멈추기
        GetUserCharacter.StopAttack();  // 유저가 죽을경우도 고려하기
        GetSkillController.gameObject.SetActive(false);
        Action afterFadeAction = null;
        //보상 획득
        float mapEffectGold = 1;
        float mapEffectExp = 1;
        float mapEffectDropRate = 1;

        //Debug.Log(_mapEffect.EffectValue + " /// " + _mapEffect.EffectType);
        if(_mapEffect != null&& _mapEffect.EffectType != eMapEffect.None)
        {
            switch (_mapEffect.EffectType)
            {
                case eMapEffect.None:
                    break;
                case eMapEffect.HPDecrease:
                    break;
                case eMapEffect.MoneyIncrease:
                    mapEffectGold = _mapEffect.EffectValue;
                    break;
                case eMapEffect.EXPIncrease:
                    mapEffectExp = _mapEffect.EffectValue;
                    break;
                case eMapEffect.DropIncrease:
                    mapEffectDropRate = _mapEffect.EffectValue;
                    break;
                case eMapEffect.Labyrinth:
                    break;
                default:
                    break;
            }


            _mapEffect = null;
        }




        int equipindex = -1;
        int gold = (int)(CurrentMapWayData.Reward_Gold * (IngameStat.GetTotalStat(eStatInfo.Gold)) * mapEffectGold); //HACK TEMP
        float explevelmodifier = (Mathf.Clamp((CurrentMapWayData.WayPointLevel / (float)IngameStat.CurrentLevel), 1, 2f)); // 이걸 넣어야할 이유가 있을까? 낮을 렙만 잡는다고 패널티를 줄 이유가 없을거같은데 고민 좀 더 해보자.
        int exp = (int)(CurrentMapWayData.Reward_Exp * explevelmodifier * (IngameStat.GetTotalStat(eStatInfo.Exp))); //HACK TEMP
        int stamina = 0;
        int soul = 0;
        object item = null;

        //----해당값은 졌다면 쓰일일 없음. 이겼을 경우 리워드 쪽에서 표시하기 위해 남기는것.
        int currentLevel = IngameStat.CurrentLevel;
        float currentTotal = IngameStat.CurrentEXP;
        // -----
        int dropitem = -1;
        if (isPlayerWin)//승리
        {
            //------------------------------------gold
            GainGold(gold);

            //------------------------------------dropitems
            dropitem = DropItem(mapEffectDropRate);

            equipindex = dropitem / 10000;
            if(dropitem != -1)
            {
                Debug.Log(equipindex + " / " + dropitem);    
                if(equipindex == 1) // weapon
                {
                    foreach(var a in InitController.Instance.GameDatas.WeaponDatas.Weapon)
                    {
                        if(a.Index == dropitem)
                        {
                            if(InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword].IsAcquired == true)
                            {
                                int count = InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword].Count + 1;
                                InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword] = new EquipAcquiredCount(true, count);
                                Debug.Log(a.Keyword + " alreay aquired.Current count is " + count);
                            }
                            else
                            {
                                InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[a.Keyword] = new EquipAcquiredCount(true, 1);
                                Debug.Log(a.Keyword + " aquired");
                            }
                            item = a;
                            break;
                        }
                    }
                }
                else if(equipindex == 2) //armor
                {
                    foreach (var a in InitController.Instance.GameDatas.ArmorDatas.Armor)
                    {
                        if (a.Index == dropitem)
                        {
                            if (InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword].IsAcquired == true)
                            {
                                int count = InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword].Count + 1;
                                InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, count);
                                Debug.Log(a.Keyword + " alreay aquired.Current count is " + count);
                            }
                            else
                            {
                                InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, 1);
                                Debug.Log(a.Keyword + " aquired");
                            }
                            item = a;
                            break;
                        }
                    }
                }
                else if(equipindex == 3) // acce
                {
                    foreach (var a in InitController.Instance.GameDatas.AccessoryDatas.Accessory)
                    {
                        if (a.Index == dropitem)
                        {
                            if (InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword].IsAcquired == true)
                            {
                                int count = InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword].Count + 1;
                                InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, count);
                                Debug.Log(a.Keyword + " alreay aquired.Current count is " + count);
                            }
                            else
                            {
                                InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, 1);
                                Debug.Log(a.Keyword + " aquired");
                            }
                            item = a;
                            break;
                        }
                    }
                }
                else if( equipindex == 4) //skillstone
                {
                    foreach (var a in InitController.Instance.GameDatas.SkillStoneDatas.SkillStone)
                    {
                        if (a.Index == dropitem)
                        {
                            if (InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword].IsAcquired == true)
                            {
                                int count = InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword].Count + 1;
                                InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, count);
                                Debug.Log(a.Keyword + " alreay aquired.Current count is " + count);
                            }
                            else
                            {
                                InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[a.Keyword] = new EquipAcquiredCount(true, 1);
                                Debug.Log(a.Keyword + " aquired");
                            }
                            item = a;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("Wrong Items");
                }
            }
            //TODO 보상은 여기서 다 뿌린 후에 리워드창에서는 그냥 보여주기만 해야함. 리워드창 ondisable에서 코루틴을 다 끌 생각이라. 거기서 주게 되면 누락 분명히 발생함.

            yield return StartCoroutine(CalcXPMethod(exp));

            if (CurrentMapWayData.WayPointType == WayPointType.Boss && IsCurrentWayPointCleared == false)
            {
                //보스 첫 킬 시 관련 재화, 스태미나 획득
                StaminaAddictive(Nums.BaseBossFirstClearStaminaReward);
                stamina = - Nums.BaseBossFirstClearStaminaReward;
                soul = CurrentMapWayData.Soul;
                InitController.Instance.SaveDatas.GetSoul(soul);
                IsCurrentWayPointCleared = true;
            }
            InitController.Instance.Sounds.PlayBGM(Constants.Victory, eBackSound.None);

            playerdmgrecord = KilledEnemyList[0].GetHitDmgRecordDic;
        }
        else //패배
        {
            //TODO 아래 리워드 창에서 패배//승리 관련 따로 보여줘야될듯?
            //TODO  스태미너도 여기서 감소?
            StaminaAddictive(Nums.BaseBattleLoseStaminaCost, true);
            InitController.Instance.Sounds.PlayBGM(Constants.Lose,eBackSound.None);

            playerdmgrecord = CurrentEnemyList[0].GetHitDmgRecordDic;

        }
        enemydmgrecord = GetUserCharacter.GetHitDmgRecordDic;

        if (KilledEnemyList!=null)
        {
            foreach (var a in KilledEnemyList)
            {
                Destroy(a.gameObject);
            }
            KilledEnemyList.Clear();
        }

        if (dropitem != -1)
        {
            if(_dropitem != null)
            {
                _dropitem.gameObject.SetActive(true);
            }
            else
            {
                _dropitem = Instantiate(_dropItemPrefab);
                _dropitem.transform.position = EnemyPos;
            }
        }
        yield return new WaitForSeconds(1f);

        InitController.Instance.SaveDatas.SaveGame();
        
        InitController.Instance.SetTimeScale(1);

        afterFadeAction = () =>
        {
            if(CurrentMapWayData.WayPointType != WayPointType.WayPoint)
            {
                CheckStamina(isPlayerWin);
            }
        };
        RewardUIPopup rewardpop = InitController.Instance.UIs.OpenUIPopup<RewardUIPopup>(null, InitController.Instance.UIs.transform);
        rewardpop.InitUIPopup();
        rewardpop.ShowReward(isPlayerWin, item, gold, exp, currentLevel, currentTotal, playerdmgrecord, enemydmgrecord, stamina, soul);
        rewardpop.OnDisableAction = () =>
        {
            InitController.Instance.UIs.LoadingPanel(PermanantUI.BattleUI, PermanantUI.MapUI, null, afterFadeAction);
            InitController.Instance.Sounds.PlayBGM(Constants.Main, InitController.Instance.GamePlays.CurrentMapWayData.BackSound);
        };



        OnBattleEnd?.Invoke();//TODO 이거 순서가 위의 reward보다 위여야될거같긴한데 ...

        #region regacy
        /*
        Debug.Log(_currentState);
        if (_currentState == eState.Stop)
        {
            InitController.Instance.SetTimeScale(1);
            InitController.Instance.UIs.LoadingPanel(PermanantUI.BattleUI, PermanantUI.MapUI);
        }
        else if (_currentState == eState.Repeat)
        {
            SetBattle();
            StartBattle();
            //버튼 없이 바로 시작. 현재스테이지
        }
        else if (_currentState == eState.Continue) //TODO 변경으로 인해서 층 올라가는 부분 지울 예정
        {
            //_currentFloor++;
            SetBattle();
            StartBattle();
            //버튼없이 바로 시작. 스테이지등반

        }


        */
        #endregion
    }
    /*
    public void CalcXP(float xp)
    {
        int currentLv = IngameStat.CurrentLevel;
        float remains = xp - (((currentLv / 10) + 1) * 250 - IngameStat.CurrentEXP);
        if (remains>=0) //잔여 EXP 보다 많은 경험치가 들어온경우
        {
            //레벨업
            IngameStat.CurrentLevel++;
            IngameStat.CurrentEXP = 0;
            IngameStat.FreeStatPoint = IngameStat.FreeStatPoint + Nums.StatPointPerLevelUp;
            Debug.Log($"Level : {IngameStat.CurrentLevel} / EXP : {IngameStat.CurrentEXP} / 잔여획득경험치 : {remains}" );
            CalcXP(remains);
        }
        else
        {
            IngameStat.CurrentEXP += xp;
            Debug.Log($"Level : {IngameStat.CurrentLevel} / EXP : {IngameStat.CurrentEXP}");
        }
    }
    IEnumerator CalcXPRout(float xp)
    {
        bool isDone = true;
        int count = 0;
        while(isDone)
        {
            while(count <20)
            {
                count++;
                xp -= ((IngameStat.CurrentLevel / 10) + 1) * 250 - IngameStat.CurrentEXP;
                if (xp >= 0)
                {
                    IngameStat.CurrentLevel++;
                    IngameStat.CurrentEXP = 0;
                    IngameStat.FreeStatPoint = IngameStat.FreeStatPoint + Nums.StatPointPerLevelUp;
                    Debug.Log($"Level : {IngameStat.CurrentLevel} / EXP : {IngameStat.CurrentEXP} / 잔여획득경험치 : {xp}");
                }
                else
                {
                    xp += ((IngameStat.CurrentLevel / 10) + 1) * 250 - IngameStat.CurrentEXP;
                    IngameStat.CurrentEXP += xp;
                    isDone = false;
                    count = 20;
                    Debug.Log($"Level : {IngameStat.CurrentLevel} / EXP : {IngameStat.CurrentEXP}");
                }
            }
            count = 0;
            yield return null;
        }

    }
    */

    /// <summary>
    /// 예를 들어 경험치 500을 받았을 경우, n^(1.5) < 500 < (n+1)^(1.5) 인 n 값을 찾는 것. 해당 n은 62. 대입하면 488.18... < 500 < 500.04....// 레벨의 1.5승값이 해당 레벨의 누적 경험치가 됨
    /// </summary>
    /// <param name="xp"></param>
    /// <returns></returns>
    IEnumerator CalcXPMethod(float xp)
    {
        int level = IngameStat.CurrentLevel;
        float totalExp = IngameStat.CurrentEXP + xp;

        int levelUp = Mathf.FloorToInt(Mathf.Pow(totalExp, 1/ Nums.ExpModifier)) + 1;
        float remains = totalExp - (Mathf.Pow(levelUp - 1, Nums.ExpModifier));

        Debug.Log($"xp : {xp}, total : {totalExp}, level : {levelUp}, currentExp : {remains} " +
            $"toNextLevel:{Mathf.Pow(levelUp, Nums.ExpModifier) - Mathf.Pow(levelUp-1, Nums.ExpModifier)}, " +
            $"need {Mathf.Pow(levelUp, Nums.ExpModifier) - Mathf.Pow(levelUp - 1, Nums.ExpModifier) - remains}");

        bool isautodistribute;
        if (PlayerPrefs.GetInt(Constants.IsAutoDistributeStat, 0) == 0) isautodistribute = false;
        else isautodistribute = true;
        if (isautodistribute)
        {
            Debug.Log(levelUp - level);

            int freePoint = (levelUp - level) * Nums.StatPointPerLevelUp;

            //int vital = IngameStat.StatDistribitionRatio[(int)eStat.Vitality] * (levelUp - level);
            //int str = IngameStat.StatDistribitionRatio[(int)eStat.Strength] * (levelUp - level);
            //int agi = IngameStat.StatDistribitionRatio[(int)eStat.Agility] * (levelUp - level);
            //int luck = IngameStat.StatDistribitionRatio[(int)eStat.Luck] * (levelUp - level);

            int vital = PlayerPrefs.GetInt(Constants.VitDistribute, 0);
            int str = PlayerPrefs.GetInt(Constants.StrDistribute, 0);
            int agi = PlayerPrefs.GetInt(Constants.AgiDistribute, 0);
            int luck = PlayerPrefs.GetInt(Constants.LucDistribute, 0);

            IngameStat.Vitality += vital * (levelUp - level);
            IngameStat.Strength += str * (levelUp - level);
            IngameStat.Agility += agi * (levelUp - level);
            IngameStat.Luck += luck * (levelUp - level);

            freePoint -= (vital + str + agi + luck) * (levelUp - level);
            IngameStat.FreeStatPoint += freePoint;
        }
        else
        {
            IngameStat.FreeStatPoint += (levelUp - level) * Nums.StatPointPerLevelUp;
        }
        SetLevel(levelUp);
        IngameStat.CurrentEXP = totalExp;
        yield return null;
        //TODO 결정된 경험치량을 이미지로 보여주기
    }

    public int DropItem(float droprate)
    {
        Dictionary<string, float> dropItems = InitController.Instance.GameDatas.GetDropItemDic(CurrentMapWayData, droprate);
        if (dropItems == null) return -1;

        float nodrop = InitController.Instance.GameDatas.DropTableDic[CurrentMapWayData.DropTable].NoDrop;

        #region datacontroller로 이전
        /*
        if (string.IsNullOrEmpty(CurrentMapWayData.DropTable))
        {
            Debug.LogWarning("empty DropTable");
            return -1; 
        }

        DropTableDataEntity drops = InitController.Instance.GameDatas.DropTableDic[CurrentMapWayData.DropTable];

        Dictionary<string, int> dropitems = new Dictionary<string, int>();

        int nodrop = drops.NoDrop;
        if (string.IsNullOrEmpty(drops.DropItem_Weapon) == false)
        {
            string[] weapon = drops.DropItem_Weapon.Split(',');
            int[] proc = Array.ConvertAll(drops.DropWeight_Weapon.Split(','), int.Parse);
            if (weapon.Length != proc.Length) Debug.LogWarning("droptable proc does not exist");
            for (int i = 0; i < weapon.Length; i++)
            {
                dropitems.Add(weapon[i], proc[i]);
                nodrop += proc[i];
            }
        }

        if (string.IsNullOrEmpty(drops.DropItem_Armor) == false)
        {
            string[] armor = drops.DropItem_Armor.Split(',');
            int[] proc = Array.ConvertAll(drops.DropWeight_Armor.Split(','), int.Parse);
            if (armor.Length != proc.Length) Debug.LogWarning("droptable proc does not exist");
            for (int i = 0; i < armor.Length; i++)
            {
                dropitems.Add(armor[i], proc[i]);
                nodrop += proc[i];
            }
        }


        if (string.IsNullOrEmpty(drops.DropItem_Accessory) == false)
        {
            string[] accessory = drops.DropItem_Accessory.Split(',');
            int[] proc = Array.ConvertAll(drops.DropWeight_Accessory.Split(','), int.Parse);
            if (accessory.Length != proc.Length) Debug.LogWarning("droptable proc does not exist");
            for (int i = 0; i < accessory.Length; i++)
            {
                dropitems.Add(accessory[i], proc[i]);
                nodrop += proc[i];
            }
        }

        if (string.IsNullOrEmpty(drops.DropItem_SkillStone) == false)
        {
            string[] skillstone = drops.DropItem_SkillStone.Split(',');
            int[] proc = Array.ConvertAll(drops.DropWeight_SkillStone.Split(','), int.Parse);
            if (skillstone.Length != proc.Length) Debug.LogWarning("droptable proc does not exist");
            for (int i = 0; i < skillstone.Length; i++)
            {
                dropitems.Add(skillstone[i], proc[i]);
                nodrop += proc[i];
            }
        }
        */
        #endregion

        float total = nodrop;

        foreach(var a in dropItems)
        {
            total += a.Value;
        }
        foreach(var a in dropItems)
        {
            Debug.Log(a.Key + "의 확률은 " + ((a.Value) / total).ToString("P1"));
        }


        float chance = UnityEngine.Random.Range(0f, 1f);
        chance *= total;

        if (chance < nodrop)
        {
            Debug.Log("NoItem : " + chance + "<" + nodrop + " / " + total);
            return -1;
        }
        foreach (var a in dropItems)
        {
            nodrop += a.Value;
            if (chance < nodrop)
            {
                Debug.Log(a.Key + "/ " + chance + "<" + nodrop + " / " + total);
                return int.Parse(a.Key);
            }
        }
        return -1;
    }

    /// <summary>
    /// 골드 사용하거나 획득할 때. 이벤트 연결은 관측용만 연결하는걸로. 그 외 사용처는 연결 X
    /// </summary>
    public Action<int> OnGoldUseOrGain { get; set; }
    /// <summary>
    /// 레벨 증가 했을 때. 이벤트 연결은 관측용만 연결하는걸로. 그 외 사용처는 연결 X
    /// </summary>
    public Action<int> OnLevelUP { get; set; }
    /// <summary>
    /// 스태미너 사용할 때. 이벤트 연결은 관측용만 연결하는걸로. 그 외 사용처는 연결 X
    /// </summary>
    public Action<int> OnStaminaUse { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="staminause">소모할 스태미너 수치</param>
    /// <param name="forceUse">강제사용여부. 강제 전투에서는 true</param>
    /// <returns></returns>
    public bool StaminaAddictive(int staminause, bool forceUse = false)
    {
        int current = (int)IngameStat.GetTotalStat(eStatInfo.Stamina);
        if(current >= staminause)
        {
            //IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Stamina, out Dictionary<eStatCalcType, float> staminaDic);
            //if (staminaDic.ContainsKey(eStatCalcType.AddInt))
            //{
            //    staminaDic[eStatCalcType.AddInt] -= staminause;
            //}
            //else
            //{
            //    staminaDic.Add(eStatCalcType.AddInt, -staminause);
            //}

            //OnStaminaUse?.Invoke(current - staminause);

            IngameStat.CurrentBaseStamina -= staminause;

            OnStaminaUse?.Invoke(current-staminause);

            return true;
        }
        else
        {
            if(forceUse == true)
            {
                //staminause = current;
                //IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Stamina, out Dictionary<eStatCalcType, float> staminaDic);
                //if (staminaDic.ContainsKey(eStatCalcType.AddInt))
                //{
                //    staminaDic[eStatCalcType.AddInt] -= staminause;
                //}
                //else
                //{
                //    staminaDic.Add(eStatCalcType.AddInt, -staminause);
                //}
                //OnStaminaUse?.Invoke(current - staminause);

                staminause = current;
                IngameStat.CurrentBaseStamina -= staminause;

                OnStaminaUse?.Invoke(current - staminause);
            }
            return false;
        }
    }
    public void CheckStamina(bool isWin)
    {
        int current = (int)IngameStat.GetTotalStat(eStatInfo.Stamina);

        if(current<=0)
        {
            UIController uicon = InitController.Instance.UIs;
            if (uicon.CurrentPopupStack.Count <= 0)
            {
                uicon.OpenUIPopup<GameOverPopup>(null, InitController.Instance.UIs.transform).InitUIPopup();
            }
            else
            {
                uicon.CurrentPopupStack.Peek().OnClose_Disposable += () =>
                {
                    //HACK 장비 탈착으로 게임오버 당하는 경우 게임 오버 창이 두개 이상 뜰 수 있어서 방지용 조건문임.
                    if (uicon.CurrentPopupStack.Count > 0 && uicon.CurrentPopupStack.Peek().GetType() == typeof(GameOverPopup)) return; 
                    InitController.Instance.UIs.OpenUIPopup<GameOverPopup>(null, InitController.Instance.UIs.transform).InitUIPopup();
                };

            }

            OnAccessoryEquipped = null;
            OnAccessoryUnEquipped = null;
            OnWeaponEquipped = null;
            OnWeaponUnEquipped = null;

            Debug.Log("GameOver");
        }
        else
        {
            if(isWin)
            {
                if (PlayerPrefs.GetInt(Constants.IsOpenStatUI, 1) == 0) // 0 == false
                {

                }
                else
                {
                    InitController.Instance.UIs.OpenUIPopup<StatUIPopup>(null, InitController.Instance.UIs.transform).InitUIPopup();
                }

            }
        }
    }
    public bool UseGold(int gold)
    {
        if(IngameStat.CurrentGold>= gold)
        {
            IngameStat.CurrentGold -= gold;

            OnGoldUseOrGain?.Invoke(IngameStat.CurrentGold);
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool GainGold(int gold)
    {
        IngameStat.CurrentGold += gold;

        OnGoldUseOrGain?.Invoke(IngameStat.CurrentGold);
        return true;
    }
    public bool SetLevel(int level)
    {
        if(IngameStat.CurrentLevel > level)
        {
            Debug.LogError("적은 레벨 들어옴 ");
            return false;
        }
        else if(IngameStat.CurrentLevel == level)
        {
            Debug.Log("레벨업 못함");
            return false;
        }
        else
        {
            IngameStat.CurrentLevel = level;

            OnLevelUP?.Invoke(IngameStat.CurrentLevel);
        }

        return true;
    }
    /// <summary>
    /// 현재 맵 케릭의 위협도 관리에만 사용함.
    /// </summary>
    public Action<AccessoryDataEntity> OnAccessoryEquipped { get; set; }
    public Action<AccessoryDataEntity> OnAccessoryUnEquipped { get; set; }
    public void ApplyAccessoryStatOnEquip(AccessoryDataEntity data)
    {
        if(data.BaseStat != eStatInfo.None)
        {
            IngameStat.AddictiveStatDic.TryGetValue(data.BaseStat, out Dictionary<eStatCalcType, float> addicts);
            float stat = data.BaseValue;


            if(data.IsBaseValuePercent) //값이 %인경우 경험치 획득률 등
            {
                if (data.ValueAddType == eStatAddType.Add)
                {
                    if (addicts.ContainsKey(eStatCalcType.AddPercent))
                    {
                        addicts[eStatCalcType.AddPercent] += stat;
                    }
                    else
                    {
                        addicts.Add(eStatCalcType.AddPercent, stat);
                    }
                }
                else if (data.ValueAddType == eStatAddType.Multiply)
                {
                    Debug.Log("Multiply value is not exist now");
                }
            }
            else //값이 정수인 경우 - 체력, 공격력 등
            {
                if (data.ValueAddType == eStatAddType.Add)
                {
                    if (addicts.ContainsKey(eStatCalcType.AddInt))
                    {
                        addicts[eStatCalcType.AddInt] += stat;
                    }
                    else
                    {
                        addicts.Add(eStatCalcType.AddInt, stat);
                    }
                }
                else if (data.ValueAddType == eStatAddType.Multiply)
                {
                    Debug.Log("Multiply value is not exist now");
                }
            }

        }
        else
        {
            if(data.SecondaryEffect != eSecondaryEffect.None) //스킬 존재시
            {

                if (IngameStat.SecondaryEffectOnHit.ContainsKey(data.SecondaryEffect))
                {
                    IngameStat.SecondaryEffectOnHit[data.SecondaryEffect]+= data.SecondaryEffectLevel;
                }
                else
                {
                    IngameStat.SecondaryEffectOnHit.Add(data.SecondaryEffect, data.SecondaryEffectLevel);
                }
                //Debug.Log(IngameStat.SecondaryEffectOnHit[data.SecondaryEffect]);
                //Debug.Log(data.SecondaryEffect + " skill equipped, level is " + data.SecondaryEffectLevel);
            }
        }
        OnAccessoryEquipped?.Invoke(data);
    }
    public void RemoveAccessoryStatOnUnEquip(AccessoryDataEntity data)
    {
        if(data.BaseStat != eStatInfo.None)
        {
            IngameStat.AddictiveStatDic.TryGetValue(data.BaseStat, out Dictionary<eStatCalcType, float> addicts);

            if (data.IsBaseValuePercent == true) //퍼센트 표현 값이라면
            {
                if (addicts.ContainsKey(eStatCalcType.AddPercent))
                {
                    addicts[eStatCalcType.AddPercent] -= data.BaseValue;
                }
                if (Mathf.Abs(addicts[eStatCalcType.AddPercent]) < 0.0001f)
                    addicts[eStatCalcType.AddPercent] = 0;
            }
            else
            {
                if (addicts.ContainsKey(eStatCalcType.AddInt))
                {
                    addicts[eStatCalcType.AddInt] -= data.BaseValue;
                }
                if (Mathf.Abs(addicts[eStatCalcType.AddInt]) < 0.0001f)
                    addicts[eStatCalcType.AddInt] = 0;
            }
        }
    
        if (data.SecondaryEffect != eSecondaryEffect.None) //스킬 존재시
        {
            if (IngameStat.SecondaryEffectOnHit.ContainsKey(data.SecondaryEffect))
            {
                //Debug.Log(IngameStat.SecondaryEffectOnHit[data.SecondaryEffect]);
                IngameStat.SecondaryEffectOnHit[data.SecondaryEffect] -= data.SecondaryEffectLevel;
                if(IngameStat.SecondaryEffectOnHit[data.SecondaryEffect] <= 0)
                {
                    Debug.Log("current skill, "+data.SecondaryEffect+" is unequipped and level is " + IngameStat.SecondaryEffectOnHit[data.SecondaryEffect]);
                    IngameStat.SecondaryEffectOnHit.Remove(data.SecondaryEffect);
                }
                //Debug.Log(IngameStat.SecondaryEffectOnHit.TryGetValue(data.SecondaryEffect, out int value));
                //Debug.Log(value);
            }
            else
            {
                Debug.LogWarning("skill is not exist in dictionary");
            }
        }
        OnAccessoryUnEquipped?.Invoke(data);
    }
    /// <summary>
    /// 현재는 맵에서 케릭 무기 스프라이트 보여주는 용도로만 사용함
    /// </summary>
    public Action<WeaponDataEntity> OnWeaponEquipped { get; set; }
    public Action<WeaponDataEntity> OnWeaponUnEquipped { get; set; }

    public void ApplyStatOnWeaponEquip(WeaponDataEntity data)
    {
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MinDmg, out Dictionary<eStatCalcType, float> minAddicts);
        if (minAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            minAddicts[eStatCalcType.AddInt] += data.MinDmg;
        }
        else
        {
            minAddicts.Add(eStatCalcType.AddInt, data.MinDmg);
        }
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MaxDmg, out Dictionary<eStatCalcType, float> maxAddicts);
        if (maxAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            maxAddicts[eStatCalcType.AddInt] += data.MaxDmg;
        }
        else
        {
            maxAddicts.Add(eStatCalcType.AddInt, data.MaxDmg);
        }
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Atk, out Dictionary<eStatCalcType, float> atkAddicts);
        if (atkAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            atkAddicts[eStatCalcType.AddInt] += data.SecondStatValue;
        }
        else
        {
            atkAddicts.Add(eStatCalcType.AddInt, data.SecondStatValue);
        }
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.ASpeed, out Dictionary<eStatCalcType, float> aspeedAddicts);
        if(aspeedAddicts.ContainsKey(eStatCalcType.MultiplyPercent))
        {
            aspeedAddicts[eStatCalcType.MultiplyPercent] += data.AspeedModifier;
        }
        else
        {
            aspeedAddicts.Add(eStatCalcType.MultiplyPercent, data.AspeedModifier);
        }
        OnWeaponEquipped?.Invoke(data);
    }
    public void RemoveStatOnUnEquipWeapon(WeaponDataEntity data)
    {
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MinDmg, out Dictionary<eStatCalcType, float> minAddicts);
        if (minAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            minAddicts[eStatCalcType.AddInt] -= data.MinDmg;
        }
        if (Mathf.Abs(minAddicts[eStatCalcType.AddInt]) < 0.0001f)
            minAddicts[eStatCalcType.AddInt] = 0;

        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.MaxDmg, out Dictionary<eStatCalcType, float> maxAddicts);
        if (maxAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            maxAddicts[eStatCalcType.AddInt] -= data.MaxDmg;
        }
        if (Mathf.Abs(maxAddicts[eStatCalcType.AddInt]) < 0.0001f)
            maxAddicts[eStatCalcType.AddInt] = 0;

        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Atk, out Dictionary<eStatCalcType, float> atkAddicts);
        if (atkAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            atkAddicts[eStatCalcType.AddInt] -= data.SecondStatValue;
        }
        if (Mathf.Abs(atkAddicts[eStatCalcType.AddInt]) < 0.0001f)
            atkAddicts[eStatCalcType.AddInt] = 0;

        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.ASpeed, out Dictionary<eStatCalcType, float> aspeedAddicts);
        if (aspeedAddicts.ContainsKey(eStatCalcType.MultiplyPercent))
        {
            aspeedAddicts[eStatCalcType.MultiplyPercent] -= data.AspeedModifier;
        }
        if (Mathf.Abs(aspeedAddicts[eStatCalcType.MultiplyPercent]) < 0.0001f)
            aspeedAddicts[eStatCalcType.MultiplyPercent] = 0;

        OnWeaponUnEquipped?.Invoke(null);
    }
    public void ApplyStatOnArmorEquip(int baseValue, int secondValue)
    {
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Def, out Dictionary<eStatCalcType, float> defAddicts);
        if (defAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            defAddicts[eStatCalcType.AddInt] += baseValue;
        }
        else
        {
            defAddicts.Add(eStatCalcType.AddInt, baseValue);
        }
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.HP, out Dictionary<eStatCalcType, float> hpAddicts);
        if (hpAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            hpAddicts[eStatCalcType.AddInt] += secondValue;
        }
        else
        {
            hpAddicts.Add(eStatCalcType.AddInt, secondValue);
        }
    }
    public void RemoveStatOnUnEquipArmor(int def, int hp)
    {
        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.Def, out Dictionary<eStatCalcType, float> defAddicts);
        if (defAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            defAddicts[eStatCalcType.AddInt] -= def;
        }
        if (Mathf.Abs(defAddicts[eStatCalcType.AddInt]) < 0.0001f)
            defAddicts[eStatCalcType.AddInt] = 0;

        IngameStat.AddictiveStatDic.TryGetValue(eStatInfo.HP, out Dictionary<eStatCalcType, float> hpAddicts);
        if (hpAddicts.ContainsKey(eStatCalcType.AddInt))
        {
            hpAddicts[eStatCalcType.AddInt] -= hp;
        }
        if (Mathf.Abs(hpAddicts[eStatCalcType.AddInt]) < 0.0001f)
            hpAddicts[eStatCalcType.AddInt] = 0;

    }
}
