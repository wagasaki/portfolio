using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public enum eSkillActionType
{
    DmgSkill,
    DmgAddDebuffSkill,
    DmgAddBuffSkill,
    DmgExplodeDebuffSkill,
    SelfBuffSkill,
    DebuffSkill,
}
public enum eSecondaryEffect
{
    None = -1,
    Burn,
    ElectricShock,
    FrostBite,
    Poison,
    Bleeding,
    Shield,
    Enchant,
    Reduction,
    Cleansing,
    Burst,
    Test5,
}
/// <summary>
/// atk, dmg, hitcount, iscrit, key, triggertype, damagetype
/// </summary>
public struct DamageStruct
{
    public int Atk;
    public int Dmg;
    public int HitCount;
    public int CritChance;
    public int CritDmg;
    public string Key;
    public eTriggerType TriggerType;
    public eDamageType DamageType;
}

public class PlayerSkillController : SkillController
{
    private Enemy _enemyBase;
    private UserCharacter _playerBase;
    //private SkillStoneDataEntity[] _equipedSkillDatas = new SkillStoneDataEntity[Nums.SkillStoneCount];
    private float[] _playerCoolTimes;
    private float[] _playerBaseCoolTimes;
    private eState[] _playerCurrentState;
    private SkillDataEntity[] _skilldatas;
    private SkillUseElem[] _skillElems;
    private UserCharacter _user;
    private float _coolTimeCoef;
    private void Awake()
    {
        _playerCoolTimes = new float[Nums.SkillStoneCount];
        _playerBaseCoolTimes = new float[Nums.SkillStoneCount];
        _playerCurrentState = new eState[Nums.SkillStoneCount];
        _prefab = new SkillObject[Nums.SkillStoneCount];
    }

    public override void InitPool()
    {
        base.InitPool();
    }
    private void Update()
    {
        if(InitController.Instance.GamePlays.IsBattle)
        {
            PlayerCoolTimeUpdate();
        }
    }
    private void PlayerCoolTimeUpdate()
    {
        for (int i = 0; i < _playerCoolTimes.Length; i++)
        {
            if (InitController.Instance.GamePlays.SkillDatas[i] != null)
            {

                //Debug.Log($"CoolTime : {_playerBaseCoolTimes[i]}");
                if (_playerCoolTimes[i] > 0 && _playerCurrentState[i] == eState.CoolTime)//현재상태 쿨타임, 쿨 도는중
                {
                    _playerCoolTimes[i] -= Time.deltaTime;
                    _skillElems[i].RefreshCoolTime(_playerCoolTimes[i] / _playerBaseCoolTimes[i]);
                }
                else if (_playerCoolTimes[i] <= 0 && _playerCurrentState[i] == eState.CoolTime) //현재상태 쿨타임, 쿨은 다 돔. => Usable로 변경
                {
                    _playerCurrentState[i] = eState.Usable;
                    _skillElems[i].RefreshCoolTime(_playerCoolTimes[i] / _playerBaseCoolTimes[i]);
                }

            }
        }
    }
    private void InitializeSkill_Data_Elem_State_Cooltime_prefab()
    {
        _skilldatas = InitController.Instance.GamePlays.SkillDatas;
        _skillElems = InitController.Instance.UIs.GetBattlelUICanvas.GetSkillUseElems;

        for (int i = 0; i < Nums.SkillStoneCount; i++)
        {
            _playerCoolTimes[i] = int.MaxValue;

            _playerCurrentState[i] = eState.CoolTime;
            _prefab[i] = null;
        }
    }
    public void SetSkillPrefabPool()
    {
        InitializeSkill_Data_Elem_State_Cooltime_prefab();
        float baseCooltime = InitController.Instance.GamePlays.GetUserCharacter.BaseCoolTime;
        _coolTimeCoef = Mathf.Clamp(1 - baseCooltime, 0, 1); //basecooltime은 0~0.2f범위. 쿨타임 감소.
        for (int i = 0; i < Nums.SkillStoneCount; i++)
        {
            if(_skilldatas[i]==null)
            {
                _prefab[i]= null;
            }
            else
            {
                string skillKeyword = _skilldatas[i].Keyword;
                SkillObject obj = InitController.Instance.GameDatas.SkillObjectDic[skillKeyword];
                _playerBaseCoolTimes[i] = _skilldatas[i].CoolTime * _coolTimeCoef;
                _prefab[i] = obj;
            }
        }
        InitPool();


        if(transform.childCount> 50)
        {
            foreach (Transform a in transform)
            {
                Destroy(a.gameObject);
            }
        }
        else
        {
            foreach (Transform a in transform)
            {
                if (a.gameObject.activeInHierarchy)
                    a.gameObject.SetActive(false);
            }
        }
        for(int i = 0; i< Nums.SkillStoneCount;i++)
        {
            if (InitController.Instance.GamePlays.SkillDatas[i] != null)
            {
                _playerCoolTimes[i] = _playerBaseCoolTimes[i] / 2f;
                _playerCurrentState[i] = eState.CoolTime;
                _skillElems[i].SkillNotUsable();
                //Debug.Log($"{_playerCurrentState[i]}, {_playerCoolTimes[i]}");
            }
        }
    }

    public bool PlayerUseSkill(int index)
    {
        if (_skilldatas[index] == null 
            || _playerCurrentState[index] == eState.CoolTime 
            || InitController.Instance.GamePlays.GetUserCharacter.UseMana(_skilldatas[index].Cost, eNumType.Constant) == false
            ) return false;

        SkillDataEntity data = _skilldatas[index];

        _playerCurrentState[index] = eState.CoolTime;
        _playerCoolTimes[index] = _skilldatas[index].CoolTime * _coolTimeCoef;
        _skillElems[index].SkillNotUsable();

        if (InitController.Instance.GamePlays.CurrentEnemyList.Count <= 0) return false;
        else
        {
            _enemyBase = InitController.Instance.GamePlays.CurrentEnemyList[0];
        }
        _playerBase = InitController.Instance.GamePlays.GetUserCharacter;

        InitController.Instance.Sounds.PlaySFX(data.Sound, index);

        DamageStruct stat = _playerBase.GetUserCharacterSkillDamageStruct(data);

        InstantiateSkillObject(stat, index, data, _playerBase, _enemyBase);


        {
            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(stat.Key, out string[] names);
            string name = names[(int)InitController.Instance.Locales.CurrentLocaleID];

            Color color = Color.white;
            switch (stat.DamageType)
            {
                case eDamageType.None:
                    Debug.LogWarning("None");
                    break;
                case eDamageType.Physics:
                    ColorUtility.TryParseHtmlString(Constants.PhysicsColor, out color);
                    break;
                case eDamageType.Fire:
                    color = Color.red;
                    break;
                case eDamageType.Ice:
                    color = Color.blue;
                    break;
                case eDamageType.Electric:
                    color = Color.yellow;
                    break;
                case eDamageType.Poison:
                    color = Color.green;
                    break;
                default:
                    color = Color.white;
                    break;
            }
            InitController.Instance.UIs.GetBattlelUICanvas.SkillNameDisplay(color, name);
        }

        return true;
    }
}

