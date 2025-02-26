using DG.Tweening;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Enemy : UnitBase
{

    public EnemyDataEntity CurrentEnemyData { get; private set; }
    [SerializeField]
    private StatueBar _statueBar;
    private Animator _animator;
    [SerializeField]
    private Transform _dropPos;
    [SerializeField]
    private Transform _centerPos;
    private SpriteRenderer _spriteRen;
    private Coroutine _spriteHitRed;
    private Color _color = Color.white;
    private Camera _mainCam;
    public Transform DropPos=> _dropPos;
    public Transform CenterPos => _centerPos;

    private AnimatorOverrideController _animatorOverrideController;

    private UserCharacter _target;
    public UserCharacter Target => _target;

    private float _maxHP;
    public Action<float, float> OnHPChange_Display { get; set; }
    public Action<float, float> OnMPChange_Display { get; set; }
    public Action<eAbsoluteTarget, string, eDamageType, float> OnDamage_Display { get; set; }

    public Dictionary<string, float> GetHitDmgRecordDic { get; private set; }
    private float CurrentHP { 
        get 
        { 
            return _currentHP; 
        }
        set
        {
            _currentHP = value;
            OnHPChange_Display?.Invoke(_currentHP , _maxHP);
            if (_currentHP <= 0)
            {
                IsAlive = false;
                StartCoroutine(fade());
            }
        }
    }
    IEnumerator fade()
    {
        while(_color.a>0)
        {
            _color.a -= Time.deltaTime;
            _spriteRen.color = _color;
            yield return null;
        }
        Debug.Log($"적 죽음 /남은 HP{_currentHP}");
        InitController.Instance.GamePlays.EnemyDie(this);
    }

    private DamageTextEffectPool _textEffectPool;
    private OneTimeAnimationPool _oneTimeAnimationPool;
    private OneTimeAnimation _slash;
    public Vector2 CurrentPos { get { return transform.position; } }
    private bool _isAttackable;

    private float _attackDelayProc;
    private float _attackDelay;
    public Transform EffectTransform { get; private set; }

    public SkillDataEntity[] EnemySkillDatas { get; private set; }
    //public EnemySkillController GetEnemySkillController { get; private set; }//TODO 추후 보스몹이나 다른 컨텐츠의 보스들만 스킬 사용하게 만들자. 일반맵에선 스킬까진 쓰지 말고
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animatorOverrideController;
        EffectTransform = Utils.FindChild<Transform>(this.gameObject, "EffectTransform");
        _spriteRen = GetComponent<SpriteRenderer>();
        _mainCam = Camera.main;
    }
    private void OnEnable()
    {
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public override void InitUnit(object data, float hpmodifierbymapeffect)
    {
        EnemyDataEntity dataEntity = data as EnemyDataEntity;
        //Debug.Log(dataEntity.Keyword + dataEntity.EnemyType.ToString());
        string paths = string.Format("RPG/EnemyData/EnemyAnimationClip/{0}/{1}", dataEntity.EnemyType.ToString(),dataEntity.Keyword);
        string attackPaths = string.Concat(paths, "/Attack");
        string idlePaths = string.Format("{0}/Idle", paths);
        _animatorOverrideController["Attack"] = Resources.Load<AnimationClip>(attackPaths);
        _animatorOverrideController["Idle"] = Resources.Load<AnimationClip>(idlePaths);

        foreach (var a in _animator.runtimeAnimatorController.animationClips)
        {
            if (a.name.Equals("Attack"))
            {
                //if (a.isLooping)//HACK 이거 어드레서블 빌드시에 오류 뜨는데 왜 뜨는지 모르겠음. AnimationUtility사용하는 부분은 다 뜨던데? 
                //{
                //    var settings = AnimationUtility.GetAnimationClipSettings(a);
                //    settings.loopTime = false;
                //    AnimationUtility.SetAnimationClipSettings(a, settings);
                //}
                //if (a.events.Length >= 2) continue;
                if (a.events.IsNullOrEmpty())
                {
                    AnimationEvent animationMiddleEvent = new AnimationEvent
                    {
                        time = a.length / 2,
                        functionName = "AttackMiddle",
                        stringParameter = a.name
                    };

                    AnimationEvent animationEndEvent = new AnimationEvent
                    {
                        time = a.length,
                        functionName = "AttackEnd",
                        stringParameter = a.name
                    };

                    a.AddEvent(animationMiddleEvent);
                    a.AddEvent(animationEndEvent);
                }
            }
            //else if (a.name.Equals("Idle"))
            //{
            //    if (a.isLooping == false)
            //    {
            //        var settings = AnimationUtility.GetAnimationClipSettings(a);
            //        settings.loopTime = true;
            //        AnimationUtility.SetAnimationClipSettings(a, settings);
            //    }
            //}
        }

        _target = InitController.Instance.GamePlays.GetUserCharacter;
        int level = InitController.Instance.GamePlays.CurrentMapWayData.WayPointLevel;

        CurrentEnemyData = dataEntity.Clone();

        CurrentBattleStat = new BattleStat();
        CurrentBattleStat.InitData();
        //=== 배틀 스탯의 추가는 여기에서. 시작시 버프 같은 경우. enemydataentity 혹은 mapwaydataentity에서 보유 특성 가지고 있고, 그걸 읽어서 출력

        StringBuilder abilnamelist = new StringBuilder();
        StringBuilder abilinfolist = new StringBuilder();
        void CheckAbil(eAbilities abil, int abilLevel)
        {
            if (abil == eAbilities.None) return;
            string abilname = abil.ToString();

            AbilityDataEntity abildata = InitController.Instance.GameDatas.EnemyAbilityDataDic[abilname];
            abilLevel = Mathf.Max(1, abilLevel);
            int finalvalue = abildata.BaseValue + (abilLevel - 1) * (abildata.MaxValue - abildata.BaseValue) / (abildata.MaxLevel - 1);
            
            if (abildata.AbilityType == eAbilityType.Stat)
            {
                if(gameObject.TryGetComponent(out AbilityAssigner_Stat ability))
                {
                    if (ability.CurrentAbility == abil)
                        return;
                }

                AbilityAssigner_Stat abilassigner = gameObject.AddComponent<AbilityAssigner_Stat>();
                abilassigner.Excute(abil, finalvalue);
            }
            else if (abildata.AbilityType == eAbilityType.OnHitEnd)
            {

                if (gameObject.TryGetComponent(out AbilityAssigner_OnHitEnd ability))
                {
                    Debug.Log(ability);
                    if (ability.CurrentAbility == abil)
                        return;
                }

                AbilityAssigner_OnHitEnd abilassigner = gameObject.AddComponent<AbilityAssigner_OnHitEnd>();
                abilassigner.Excute(abil, finalvalue);
            }
            else if(abildata.AbilityType == eAbilityType.OnUpdate)
            {
                if (gameObject.TryGetComponent(out AbilityAssigner_OnUpdate ability))
                {
                    if (ability.CurrentAbility == abil)
                        return;
                }

                AbilityAssigner_OnUpdate abilassigner = gameObject.AddComponent<AbilityAssigner_OnUpdate>();
                abilassigner.Excute(abil, finalvalue);
            }
            string key = string.Concat("ab_", abilname);
            abilnamelist.Append( string.Format("[lv.{0} {1}] ", abilLevel, InitController.Instance.GameDatas.LocalizationDataDic[key][(int)InitController.Instance.Locales.CurrentLocaleID]));
            key = string.Concat("info_", abilname);
            abilinfolist.AppendLine(string.Format(InitController.Instance.GameDatas.LocalizationDataDic[key][(int)InitController.Instance.Locales.CurrentLocaleID], (finalvalue*0.01f).ToString("P0")));
        }
        int abilitylevel = Mathf.FloorToInt(Mathf.Log10(level));
        CheckAbil(dataEntity.Abil_0, abilitylevel);
        CheckAbil(dataEntity.Abil_1, abilitylevel);
        CheckAbil(dataEntity.Abil_2, abilitylevel);
        InitController.Instance.UIs.GetBattlelUICanvas.EnemyAbilityTextRefresh(abilnamelist.ToString(), abilinfolist.ToString());
        abilnamelist = null;
        //===
        //초기 시작시 배틀 스탯 곱해주고, 추가로 버프나 디버프로 변경이 있을 경우 해당 스탯을 다시 한번 계산한다
        //ex) BaseMaxHP = PlayerIngameStatData.GetTotalStat(eStatInfo.HP) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HP];이걸 다시 해줌
        float hpmodifierbyMapData = Mathf.Max(1, InitController.Instance.GamePlays.CurrentMapWayData.EnemyHPModifier); //이건 mapeffect랑 다른 종류임. 헷갈리면 안됨
        Utils.CalcStat(false, level, dataEntity.StrengthRatio, dataEntity.AgilityRatio, dataEntity.VitalityRatio, dataEntity.LuckRatio, out float[] stats);
        CurrentEnemyData.HP = (int)(stats[0] * hpmodifierbyMapData * hpmodifierbymapeffect * UnityEngine.Random.Range(0.9f, 1.1f)*CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HP]);
        CurrentEnemyData.Atk = (int)(stats[1] * UnityEngine.Random.Range(0.9f, 1.1f) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]);
        CurrentEnemyData.Def = (int)(stats[2] * UnityEngine.Random.Range(0.9f, 1.1f) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Def]);
        CurrentEnemyData.ASpeed = dataEntity.ASpeed * UnityEngine.Random.Range(0.9f, 1.1f);// * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.ASpeed];
        CurrentEnemyData.MinDmg = (int)(CurrentEnemyData.DmgModifier * 0.01f*stats[3] * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.MinDmg]);
        CurrentEnemyData.MaxDmg = (int)(CurrentEnemyData.DmgModifier * 0.01f*stats[4] * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.MaxDmg]);
        CurrentEnemyData.HealingModifier = 100 * 0.01f;
        Debug.Log($"{CurrentEnemyData.HP} {CurrentEnemyData.Atk} {CurrentEnemyData.Def} {CurrentEnemyData.MinDmg} {CurrentEnemyData.MaxDmg}");


        _maxHP = CurrentEnemyData.HP;
        _currentHP = _maxHP;


        _attackDelayProc = 0;
        _attackDelay = CurrentEnemyData.ASpeed;

        IsAlive = true;

        _textEffectPool = InitController.Instance.GamePlays.GetTextEffectPool;

        if (_oneTimeAnimationPool == null)
        {
            _oneTimeAnimationPool = Instantiate(Resources.Load<OneTimeAnimationPool>(Paths.OneTimeAnimationPool), transform);
            _slash = Resources.Load<OneTimeAnimation>("RPG/Prefab/Slash");
            _oneTimeAnimationPool.SetPrefab(_slash);
            _oneTimeAnimationPool.InitPool();
            Debug.Log("slash pool ins");
        }
        _hptimeProc = 0;


        OnHPChange_Display?.Invoke(_currentHP, _maxHP);

        GetHitDmgRecordDic = new Dictionary<string, float>();



    }
    #region 안쓰는 init 및 sortingorder. 참고용
    /*
    public void InitEnemy(EnemyDataEntity data, UserCharacter target, int level, int sortingOrder)
    {
        _target = target;
        _Data = new EnemyDataEntity();
        _Data = data;


        Utils.CalcStat(false, level, _Data.StrengthRatio, _Data.AgilityRatio, _Data.VitalityRatio, _Data.LuckRatio, out float[] stats);
        _Data.HP = stats[0] * Random.Range(0.9f, 1.1f);
        _Data.Atk = stats[1] * Random.Range(0.9f, 1.1f);
        _Data.Def = stats[2] * Random.Range(0.9f, 1.1f);
        _Data.ASpeed = data.ASpeed * Random.Range(0.9f, 1.1f);


        _maxHP = _Data.HP;
        _currentHP = _maxHP;
        
        InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyHP(_currentHP, _maxHP);
        InitController.Instance.UIs.GetBattlelUICanvas.RefreshEnemyMP(50, 50);
        _currentTurn = 0;
        IsAlive = true;
        //SetSortingOrder(sortingOrder);

        _textEffectPool = InitController.Instance.GamePlays.GetTextEffectPool;
    }
    public void SetSortingOrder(int sortingOrder)
    {
        this.transform.Find("Sprite").GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        this.transform.Find("HPCanvas").GetComponent<Canvas>().sortingOrder = sortingOrder;
    }
    */
    #endregion

    protected override void UpdateMethod()
    {
        if (IsAlive && InitController.Instance.GamePlays.IsBattle)
        {
            base.UpdateMethod();
            UpdateAttack();
            UpdateHPMP();
        }
    }
    void UpdateHPMP()
    {
        _hptimeProc += Time.deltaTime;
        if (_hptimeProc > 1)
        {
            _hptimeProc = 0;
            //GainMana(PlayerIngameStatData.ManaRegen);
        }
    }
    public void UpdateAttack()
    {
        if (_isAttackable && InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
        {
            _attackDelayProc += Time.deltaTime * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.ASpeed];
            _statueBar.Turn(_attackDelayProc / _attackDelay);
            if (_attackDelay < _attackDelayProc)
            {
                OnDoHitStart?.Invoke(eTriggerType.NormalAttack);
                _attackDelayProc = 0;
                Attack();
            }
        }
    }
    public void StartBattle()
    {
        _isAttackable = true;
    }

    public bool Attack()
    {
        if (_target == null)
        {
            return false;
        }
        else
        {
            _isAttackable = false;
            _animator.Play("Attack");
            return true;
        }
    }
    public override int GetNormalAttackDamageCalc(float pow)
    {
        int dmg = Mathf.RoundToInt(UnityEngine.Random.Range(CurrentEnemyData.MinDmg, CurrentEnemyData.MaxDmg) * pow);
        return dmg;
    }
    public void AttackMiddle()
    {
        if (_target== null || IsAlive == false || InitController.Instance.GamePlays.IsBattle == false)
            return;
        OneTimeAnimation slash = _oneTimeAnimationPool.GetFromPool(0, this.transform);
        slash.transform.position = _target.transform.position;
        slash.GetComponent<SpriteRenderer>().flipX = true;
        slash.gameObject.SetActive(true);
        slash.OnAnimationComplete = (string ar) => slash.gameObject.SetActive(false);
        InitController.Instance.Sounds.PlaySFX(eSFX.Slash);
    }
    /// <summary>
    /// NormalAttack. 다른 공격에서 이 메서드 절대 사용 불가
    /// </summary>
    public void AttackEnd()
    {
        if (_target == null || IsAlive == false || InitController.Instance.GamePlays.IsBattle == false)
            return;

        _isAttackable = true;
        _target.GetHit(GetEnemyDamageStruct(), OnDoHitEnd);
    }
    public DamageStruct GetEnemyDamageStruct()
    {
        int damage = GetNormalAttackDamageCalc(1);
        DamageStruct attackStat = new DamageStruct
        {
            Atk = (int)(CurrentEnemyData.Atk * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Dmg = (int)(damage * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Dmg]),
            CritChance = (int)(CurrentEnemyData.CritChance * 100 * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            CritDmg = (int)(CurrentEnemyData.CritDmg * 100 * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Key = "NormalAttack",
            DamageType = eDamageType.Physics,
            TriggerType = eTriggerType.NormalAttack,
            HitCount = 1
        };
        return attackStat;
    }
    public override void GetHit(in DamageStruct attackStat, Action<eTriggerType, float> OnHitEnd, WaitForSeconds tick = null)//float dmg, bool isCrit, int hitCount)
    {
        if (IsAlive && InitController.Instance.GamePlays.IsBattle)
        {
            OnGetHitStart?.Invoke(attackStat.TriggerType);
            InitController.Instance.Sounds.PlaySFX(eSFX.Hit);

            StartCoroutine(GetHitRout(attackStat, OnHitEnd, tick));
        }
    }
    IEnumerator GetHitRout(DamageStruct attackStat, Action<eTriggerType, float> OnHitEnd, WaitForSeconds tick)
    {
        int hitcount = attackStat.HitCount;
        float defcoef = Atk_Def_Calc(attackStat.Atk, CurrentEnemyData.Def * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]);

        while (hitcount > 0 && IsAlive&&InitController.Instance.GamePlays.IsBattle)
        {
            float finalDmg = attackStat.Dmg;
            bool isCrit = false;
            if (UnityEngine.Random.Range(0, 1f) <= attackStat.CritChance * 0.01f)
            {
                finalDmg = finalDmg * attackStat.CritDmg * 0.01f;
                isCrit = true;
            }
            finalDmg = finalDmg * defcoef;

            hitcount--;

            DamageTextEffect effect = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas._textEffectParent);
            effect.gameObject.SetActive(true);
            effect.SetDmgText(finalDmg.ToString("N0"), eTextEffectMoveType.UpBounce, EffectTransform.position, isCrit);

            if (CurrentHP < finalDmg) finalDmg = CurrentHP;
            CurrentHP -= finalDmg;
            OnDamage_Display?.Invoke(eAbsoluteTarget.Enemy, attackStat.Key, attackStat.DamageType, finalDmg);

            if (GetHitDmgRecordDic.ContainsKey(attackStat.Key))
                GetHitDmgRecordDic[attackStat.Key] += finalDmg;
            else
                GetHitDmgRecordDic.Add(attackStat.Key, finalDmg);

            _mainCam.transform.position = new Vector3(0, 0, -10);
            _mainCam.DOShakePosition(0.5f, 0.05f, 10, 90);

            OnHitEnd?.Invoke(attackStat.TriggerType, finalDmg);

            if (_spriteHitRed != null)
                StopCoroutine(_spriteHitRed);
            _spriteHitRed = null;
            _spriteRen.color = Color.white;
            _spriteHitRed = StartCoroutine(HitRed());
            IEnumerator HitRed()
            {
                _spriteRen.color = Color.red;
                yield return YieldCache.WaitForSeconds(0.3f);
                _spriteRen.color = Color.white;
            }


            if (hitcount>0)
              yield return tick;
        }
    }

    public override void GetHeal(float healValue, eNumType type)
    {
        if (type == eNumType.Percent)
            healValue = (healValue * _maxHP)* 0.01f;

        healValue = healValue * CurrentEnemyData.HealingModifier * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HealingModifier];
        //Debug.Log(string.Format("heal {0}", healValue));
        
        if (CurrentHP + healValue > _maxHP)
            healValue = _maxHP - CurrentHP;
        CurrentHP += healValue;

        DamageTextEffect effect2 = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas.transform);
        effect2.gameObject.SetActive(true);
        effect2.SetHealText(healValue.ToString("N0"), eTextEffectMoveType.UpBounce, transform.position);
    }
    public override void GetMana(float gain, eNumType numType)
    {
        return;
    }
}
