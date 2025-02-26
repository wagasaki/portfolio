using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserCharacter : UnitBase
{
    private Animator _playerAnim;
    [SerializeField]
    private StatueBar _statueBar;
    public SpriteRenderer WeaponSpriteRenderer { get; set; }
    private SpriteRenderer _playerRenderer;
    private Coroutine _playerHitRed;
    public IngameStatData PlayerIngameStatData { get; set; }
    private UserData userdata;

    private ePlayerState _currentState;
    private float CurrentHP
    {
        get
        {
            return _currentHP;
        }
        set
        {
            _currentHP = value;
            OnHPChange_Display?.Invoke(_currentHP,CurrentShield, BaseMaxHP);
            if (_currentHP <= 0)
            {
                IsAlive = false;
                InitController.Instance.GamePlays.PlayerDie(this);
                return;
            }
        }
    }
    public Action<float,float, float> OnHPChange_Display { get; set; }
    public Action<float, float> OnMPChange_Display { get; set; }
    public Action<eAbsoluteTarget, string, eDamageType, float> OnDamage_Display { get; set; }

    private float _currentMana;
    private float CurrentMana
    {
        get { return _currentMana; }
        set
        {
            _currentMana = value;
            OnMPChange_Display?.Invoke(_currentMana, BaseMaxMana);
        }
    }
    private float _manaRegen;
    public float BaseMaxHP { get; private set; }
    public float BaseMaxMana { get; private set; }
    public float BaseAtk { get; private set; }
    public float BaseDef { get; private set; }
    public float BaseCritChance { get; private set; }
    public float BaseCritDmg { get; private set; }
    public float BaseHealingModifier { get; private set; }
    public float CurrentShield { get; private set; }

    public float BaseAccuracy { get; private set; }
    public float BaseResist { get; private set; }
    public float BaseEvasion { get; private set; }
    public float BaseCoolTime { get; private set; }

    //public float BuffedAtk { get; private set; }
    //public float BuffedDef { get; private set; }
    //public float BuffedCritChance { get; private set; }
    //public float BuffedCritDmg{ get; private set; }
    //public float BuffedDmg { get; private set; }


    private Enemy _target;
    public Enemy Target => _target;
    private float _attackDelayProc;
    private float _attackDelay;

    private OneTimeAnimationPool _oneTimeAnimationPool;
    private OneTimeAnimation _slash;

    private DamageTextEffectPool _textEffectPool;

    public Dictionary<string, float> GetHitDmgRecordDic { get; private set; }


    private void Awake()
    {
        WeaponSpriteRenderer = Utils.FindChild<SpriteRenderer>(this.gameObject, "Weapon");
    }
    public override void InitUnit(object dataDeepCopy, float hpmodifier)
    {
        Debug.Log(dataDeepCopy.GetType());
        userdata = InitController.Instance.SaveDatas.UserData;
        _playerAnim = GetComponent<Animator>();
        IsAlive = true;
        PlayerIngameStatData = dataDeepCopy as IngameStatData;

        CurrentBattleStat = new BattleStat();
        CurrentBattleStat.InitData();
        //===
        //배틀 스탯의 추가는 여기에서. 시작시 버프 같은 경우
        //===
        //초기 시작시 배틀 스탯 곱해주고, 추가로 버프나 디버프로 변경이 있을 경우 해당 스탯을 다시 한번 계산한다
        //ex) BaseMaxHP = PlayerIngameStatData.GetTotalStat(eStatInfo.HP) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HP];이걸 다시 해줌
        BaseMaxHP = PlayerIngameStatData.GetTotalStat(eStatInfo.HP) * hpmodifier * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HP];
        CurrentHP = BaseMaxHP;

        BaseMaxMana = PlayerIngameStatData.GetTotalStat(eStatInfo.Mana) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Mana];
        CurrentMana = BaseMaxMana;
        _manaRegen = PlayerIngameStatData.GetTotalStat(eStatInfo.ManaRegen) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.ManaRegen];

        BaseAtk = PlayerIngameStatData.GetTotalStat(eStatInfo.Atk) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk];
        BaseDef = PlayerIngameStatData.GetTotalStat(eStatInfo.Def) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Def];
        BaseCritChance = PlayerIngameStatData.GetTotalStat(eStatInfo.CritChance) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritChance];
        BaseCritDmg = PlayerIngameStatData.GetTotalStat(eStatInfo.CritDmg) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritDmg];

        BaseAccuracy = PlayerIngameStatData.GetTotalStat(eStatInfo.Accuracy) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Accuracy];
        BaseResist = PlayerIngameStatData.GetTotalStat(eStatInfo.Resist) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Resist];
        BaseEvasion = PlayerIngameStatData.GetTotalStat(eStatInfo.Evasion) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Evasion];
        BaseCoolTime = PlayerIngameStatData.GetTotalStat(eStatInfo.CoolTime) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CoolTime];

        Debug.Log(string.Format("User atk : {0}, def : {1}, critC : {2}, critD : {3}, acc : {4}, res  {5}, eva : {6}, cool : {7}", BaseAtk, BaseDef, BaseCritChance, BaseCritDmg, BaseAccuracy, BaseResist, BaseEvasion, BaseCoolTime));

        BaseHealingModifier = PlayerIngameStatData.GetTotalStat(eStatInfo.HealingModifier) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HealingModifier];

        _attackDelay = PlayerIngameStatData.GetTotalStat(eStatInfo.ASpeed);

        _currentState = ePlayerState.NotReadyForBattle;


        if (_oneTimeAnimationPool == null)
        {
            _oneTimeAnimationPool = Instantiate(Resources.Load<OneTimeAnimationPool>(Paths.OneTimeAnimationPool), transform);
            _slash = Resources.Load<OneTimeAnimation>("RPG/Prefab/Slash");
            _oneTimeAnimationPool.SetPrefab(_slash);
            _oneTimeAnimationPool.InitPool();
            Debug.Log("slash pool ins");
        }
        _textEffectPool = InitController.Instance.GamePlays.GetTextEffectPool;

        _hptimeProc = 0;

        GetHitDmgRecordDic = new Dictionary<string, float>();

        float vampirevalue = PlayerIngameStatData.GetTotalStat(eStatInfo.LifeDrain) * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.LifeDrain];
        void Vampire(eTriggerType type, float finalDmg)
        {
            if (vampirevalue <= 0) return;
            if (type == eTriggerType.NormalAttack)
            {
                Debug.Log(finalDmg + " ? " + vampirevalue);
                float healvalue = Mathf.Max(1, finalDmg * vampirevalue);
                GetHeal(healvalue, eNumType.Constant);
            }
        }
        OnDoHitEnd = (eTriggerType type, float value) => Vampire(type, value);

        _playerRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartBattle()
    {
        _currentState = ePlayerState.Idle;
    }

    protected override void UpdateMethod()
    {
        if(IsAlive && InitController.Instance.GamePlays.IsBattle)
        {
            base.UpdateMethod();
            UpdateAttack();
            UpdateHPMP();
            UpdateAutoSkill();
        }
    }
    void UpdateHPMP()
    {
        _hptimeProc += Time.deltaTime;
        if(_hptimeProc>1)
        {
            _hptimeProc = 0;
            GetMana(_manaRegen, eNumType.Constant);
        }
    }
    public void UpdateAttack()
    {
        if(InitController.Instance.GamePlays.CurrentEnemyList.Count <= 0) return;

        if (/*_isAttackable*/ _currentState == ePlayerState.Idle)
        {
            _attackDelayProc += Time.deltaTime * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.ASpeed]; ;
            _statueBar.Turn(_attackDelayProc / _attackDelay);
            if (_attackDelay < _attackDelayProc)
            {
                OnDoHitStart?.Invoke(eTriggerType.NormalAttack);
                _attackDelayProc = 0;
                _currentState = ePlayerState.Attack;
                Attack();
                //CurrentBattleStat.ModifyStat(eStatInfo.Atk, Time.deltaTime.ToString(), 0.5f); // test
            }
        }
    }

    private float _autoSkillTimeGap = 1;
    public void UpdateAutoSkill()
    {
        for(int i = 0; i < Nums.SkillStoneCount;i++)
        {
            if(userdata.AutoCastEnabled[i] == true && _autoSkillTimeGap <= 0)
            {
                //Debug.Log(i + " ? " + _autoSkillTimeGap);
                if(InitController.Instance.GamePlays.GetSkillController.PlayerUseSkill(i))
                {
                    _autoSkillTimeGap = 1;
                    break;
                }
            }
        }
        _autoSkillTimeGap = Mathf.Max(0, _autoSkillTimeGap - Time.deltaTime);
        //Debug.Log(_autoSkillTimeGap);
    }
    /// <summary>
    /// NormalAttack. 다른 공격타입에서 이 메서드 절대 사용 불가
    /// </summary>
    public void Attack()
    {
        //_isAttackable = false;

        if (_target == null)
        {
            if (InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
            {
                _target = InitController.Instance.GamePlays.CurrentEnemyList[0];
            }
        }
        _playerAnim.SetTrigger("Attack");
    }

    public override int GetNormalAttackDamageCalc(float pow)
    {
        int dmg = Mathf.RoundToInt(Random.Range(PlayerIngameStatData.GetTotalStat(eStatInfo.MinDmg), PlayerIngameStatData.GetTotalStat(eStatInfo.MaxDmg)) * pow);
        return dmg;
    }

    public void SlashEffect()
    {
        if (_target == null || IsAlive == false || InitController.Instance.GamePlays.IsBattle == false)
            return;
        OneTimeAnimation slash = _oneTimeAnimationPool.GetFromPool(0, this.transform);
        slash.transform.position = _target.transform.position;
        slash.gameObject.SetActive(true);
        slash.OnAnimationComplete = null;
        slash.OnAnimationComplete = (string ar) =>
        {
            if (_target == null || IsAlive == false || InitController.Instance.GamePlays.IsBattle == false)
                return;
            

            DamageStruct attackStat = GetUserCharacterDamageStruct();

            _target.GetHit(attackStat, OnDoHitEnd);

            foreach (var a in InitController.Instance.GamePlays.IngameStat.SecondaryEffectOnHit)//TODO 이거랑 위에 온두힛엔드랑 둘중 하나만 써. 정리하자.
            {
                float proc = Nums.AccessorySecondaryEffectProc * (1 + BaseAccuracy);
                Debug.Log("디버프 발동확률 : " + proc);
                if (Random.Range(0,100)< proc)
                {
                    SkillController.AddSecondaryEffect(attackStat, a.Key, a.Value, _target.gameObject);
                }
            }

            slash.gameObject.SetActive(false);

            _currentState = ePlayerState.Idle;
        };
        InitController.Instance.Sounds.PlaySFX(eSFX.Slash);
    }
    public DamageStruct GetUserCharacterDamageStruct()
    {
        int damage = GetNormalAttackDamageCalc(1);

        DamageStruct attackStat = new DamageStruct
        {
            Atk = (int)(BaseAtk * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Dmg = (int)(damage * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Dmg]),
            CritChance = (int)(100 * (BaseCritChance * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritChance])),
            CritDmg = (int)(100 * (BaseCritDmg * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritDmg])),
            Key = "NormalAttack",
            DamageType = eDamageType.Physics,
            TriggerType = eTriggerType.NormalAttack,
            HitCount = 1
        };
        return attackStat;
    }
    public DamageStruct GetUserCharacterSkillDamageStruct(SkillDataEntity data)
    {
        int damage = GetNormalAttackDamageCalc(data.Power);
        DamageStruct attackStat = new DamageStruct
        {
            Atk = (int)(BaseAtk * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Dmg = (int)(damage * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Dmg]),
            CritChance = (int)(100 * (BaseCritChance * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritChance])),
            CritDmg = (int)(100 * (BaseCritDmg * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.CritDmg])),
            Key = data.Keyword,
            DamageType = data.DamageType,
            TriggerType = eTriggerType.SkillAttack,
            HitCount = data.HitCount,
        };
        return attackStat;
    }
    public override void GetHit(in DamageStruct attackStat, Action<eTriggerType, float> OnHitEnd, WaitForSeconds tick = null)
    {
        if(IsAlive && InitController.Instance.GamePlays.IsBattle)
        {
            Debug.Log(BaseEvasion);
            if (attackStat.TriggerType == eTriggerType.NormalAttack && Random.Range(0,1f)< BaseEvasion)
            {
                Debug.Log(BaseEvasion);
                DamageTextEffect effect = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas._textEffectParent);
                effect.gameObject.SetActive(true);
                effect.SetAttackStateText(Constants.Miss, eTextEffectMoveType.UpBounce, transform.position);
                return;
            }
            OnGetHitStart?.Invoke(attackStat.TriggerType);
            InitController.Instance.Sounds.PlaySFX(eSFX.Hit);

            StartCoroutine(GetHitRout(attackStat, OnHitEnd, tick));
        }
    }
    IEnumerator GetHitRout(DamageStruct attackStat, Action<eTriggerType, float> OnHitEnd, WaitForSeconds tick)
    {
        int hitCount = attackStat.HitCount;
        float defcoef = Atk_Def_Calc(attackStat.Atk, BaseDef * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Def]);
        while (hitCount > 0&& IsAlive && InitController.Instance.GamePlays.IsBattle)
        {
            float finalDmg = attackStat.Dmg;
            bool isCrit = false;
            if (Random.Range(0, 1f) <= attackStat.CritChance * 0.01f)
            {
                finalDmg = finalDmg * attackStat.CritDmg * 0.01f;
                isCrit = true;
            }
            finalDmg = finalDmg * defcoef;

            if (CurrentShield >= finalDmg)
            {
                CurrentShield -= finalDmg;
            }
            else
            {
                finalDmg -= CurrentShield;
                CurrentShield = 0;

                if (CurrentHP < finalDmg)
                    finalDmg = CurrentHP;
            }


            DamageTextEffect effect = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas._textEffectParent);
            effect.gameObject.SetActive(true);
            effect.SetDmgText(finalDmg.ToString("N0"), eTextEffectMoveType.UpBounce, transform.position, isCrit);

            OnDamage_Display?.Invoke(eAbsoluteTarget.Player, attackStat.Key, attackStat.DamageType, finalDmg);

            if (GetHitDmgRecordDic.ContainsKey(attackStat.Key))
                GetHitDmgRecordDic[attackStat.Key] += finalDmg;
            else
                GetHitDmgRecordDic.Add(attackStat.Key,finalDmg);
            OnHitEnd?.Invoke(attackStat.TriggerType, finalDmg);

            hitCount--;
            CurrentHP -= finalDmg;


            if (_playerHitRed != null)
                StopCoroutine(_playerHitRed);
            _playerHitRed = null;
            _playerRenderer.color = Color.white;
            _playerHitRed = StartCoroutine(HitRed());
            IEnumerator HitRed()
            {
                bool isred = false;
                while(isred== false)
                {
                    _playerRenderer.color = Color.Lerp(_playerRenderer.color, Color.red, Time.deltaTime*10);
                    if (_playerRenderer.color.g <= 0.1f)
                    {
                        _playerRenderer.color = Color.red;
                        isred = true;
                    }
                    yield return null;
                }
                while (isred == true)
                {
                    _playerRenderer.color = Color.Lerp(_playerRenderer.color, Color.white, Time.deltaTime * 10);
                    if (_playerRenderer.color.g >= 0.9f)
                    {
                        _playerRenderer.color = Color.white;
                        isred = false;
                    }
                    yield return null;
                }
            }

            if (hitCount>0)
                yield return tick;
        }
    }
    public override void GetHeal(float healValue, eNumType type)
    {
        if (type == eNumType.Percent)
            healValue = (healValue * BaseMaxHP) * 0.01f;

        healValue = healValue * BaseHealingModifier * CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.HealingModifier];
        Debug.Log(healValue);
        if (CurrentHP + healValue > BaseMaxHP)
        {
            healValue = BaseMaxHP - CurrentHP;
        }
        CurrentHP += healValue;
        if(healValue>0)
        {
            DamageTextEffect effect2 = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas.transform);
            effect2.gameObject.SetActive(true);
            effect2.SetHealText(healValue.ToString("N0"), eTextEffectMoveType.UpBounce, transform.position);
        }
    }
    public bool UseMana(float cost,eNumType type, bool isForced = false)
    {
        if (type == eNumType.Percent)
        {
            cost = (cost * BaseMaxMana) * 0.01f;
        }
        if (CurrentMana >= cost)
        {
            CurrentMana -= cost;
            return true;
        }
        else
        {
            if(isForced == true)
            {
                CurrentMana = 0;
                return true;
            }
            return false;
        }


    }
    public override void GetMana(float getValue, eNumType type)
    {
        if (CurrentMana >= BaseMaxMana)
            return;
        else
        {
            if (type == eNumType.Percent)
            {
                getValue = (getValue * BaseMaxMana) * 0.01f;
            }
            float total = CurrentMana + getValue;
            if (total>= BaseMaxMana)
            {
                CurrentMana = BaseMaxMana;
            }
            else
            {
                CurrentMana = total;
            }
            return;
        }
    }
    public void StopAttack()
    {
        _currentState = ePlayerState.NotReadyForBattle;
    }

    public void ResistDebuff()
    {
        DamageTextEffect effect = _textEffectPool.GetFromPool(0, InitController.Instance.UIs.GetBattlelUICanvas._textEffectParent);
        effect.gameObject.SetActive(true);
        effect.SetAttackStateText(Constants.Resist, eTextEffectMoveType.UpBounce, transform.position);
    }



    #region AttackType
    //public void LineAttack()//대상과 그 위아래줄 쭉
    //{
    //    if(InitController.Instance.GamePlays.CurrentEnemyList.Count>0)
    //    {
    //        List<Enemy> targetlist = new List<Enemy>();
    //        _target = InitController.Instance.GamePlays.CurrentEnemyList[0];
    //        targetlist.Add(_target);
    //        for (int i = 0; i< InitController.Instance.GamePlays.CurrentEnemyList.Count;i++)
    //        {
    //            if(InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.x== _target.CurrentPos.x && InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.y != _target.CurrentPos.y)
    //            {
    //                targetlist.Add(InitController.Instance.GamePlays.CurrentEnemyList[i]);
    //            }
    //        }

    //        _player.PlayAnimation("2_Attack_Normal");

    //        foreach(Enemy a in targetlist)
    //        {
    //            a.GetHit(PlayerIngameStatData.Atk);
    //        }
    //    }
    //}
    //public void PierceAttack()//직선 관통
    //{
    //    if (InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
    //    {
    //        List<Enemy> targetlist = new List<Enemy>();
    //        _target = InitController.Instance.GamePlays.CurrentEnemyList[0];
    //        targetlist.Add(_target);
    //        for (int i = 0; i < InitController.Instance.GamePlays.CurrentEnemyList.Count; i++)
    //        {
    //            if (InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.y == _target.CurrentPos.y && InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.x != _target.CurrentPos.x)
    //            {
    //                targetlist.Add(InitController.Instance.GamePlays.CurrentEnemyList[i]);
    //            }
    //        }

    //        _player.PlayAnimation("2_Attack_Normal");

    //        foreach (Enemy a in targetlist)
    //        {
    //            a.GetHit(PlayerIngameStatData.Atk);
    //        }
    //    }
    //}
    //public void RandomAttack(int maxcount)
    //{
    //    if (InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
    //    {

    //    }
    //}
    //public void AllAttack()//전체
    //{
    //    if (InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
    //    {
    //        List<Enemy> targetlist = new List<Enemy>();
    //        foreach (Enemy a in InitController.Instance.GamePlays.CurrentEnemyList)
    //        {
    //            targetlist.Add(a);
    //        }
    //        _player.PlayAnimation("2_Attack_Normal");
    //        foreach (Enemy a in targetlist)
    //        {
    //            a.GetHit(PlayerIngameStatData.Atk);
    //        }
    //    }
    //}
    //public void RoundAttack()//대상 과 그 주변
    //{
    //    if (InitController.Instance.GamePlays.CurrentEnemyList.Count > 0)
    //    {
    //        List<Enemy> targetlist = new List<Enemy>();
    //        _target = InitController.Instance.GamePlays.CurrentEnemyList[0];
    //        targetlist.Add(_target);
    //        for (int i = 0; i < InitController.Instance.GamePlays.CurrentEnemyList.Count; i++)
    //        {
    //            if (InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.y == _target.CurrentPos.y)
    //            {
    //                if (InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.x != _target.CurrentPos.x)
    //                {
    //                    targetlist.Add(InitController.Instance.GamePlays.CurrentEnemyList[i]);
    //                }
    //            }
    //            else if( Mathf.Abs(InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.y - _target.CurrentPos.y) == 1)
    //            {
    //                if(InitController.Instance.GamePlays.CurrentEnemyList[i].CurrentPos.x == _target.CurrentPos.x)
    //                {
    //                    targetlist.Add(InitController.Instance.GamePlays.CurrentEnemyList[i]);
    //                }
    //            }
    //        }

    //        _player.PlayAnimation("2_Attack_Normal");

    //        foreach (Enemy a in targetlist)
    //        {
    //            a.GetHit(PlayerIngameStatData.Atk);
    //        }
    //    }
    //}

    #endregion
    #region PassiveSkill

    #endregion
    #region ActiveSkill

    #endregion
}
