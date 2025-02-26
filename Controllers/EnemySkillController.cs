using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : SkillController
{
    private Enemy _enemyBase;
    private UserCharacter _playerBase;
    private float[] _enemyCoolTimes;
    private eState[] _enemyCurrentState;
    private SkillDataEntity[] _skillDataDntity;
    public override void InitPool()
    {
        base.InitPool();
        _enemyBase = InitController.Instance.GamePlays.CurrentEnemyList[0];
        int enemySkillCount = 0;
        if (_enemyBase != null)
        {
            enemySkillCount = _enemyBase.CurrentEnemyData.Skill.Length; //TODO 이거 string으로 되어있는데, 스트링배열이나 이넘배열로 바꿀거임. 데이터베이스 조정예정
        }
        _enemyCoolTimes = new float[enemySkillCount];
        _enemyCurrentState = new eState[enemySkillCount];
    }
    private void Update()
    {
        if (InitController.Instance.GamePlays.IsBattle)
        {
            EnemyCoolTimeUpdate();
        }
    }
    private void EnemyCoolTimeUpdate()
    {
        for (int i = 0; i < _enemyCoolTimes.Length; i++)
        {
            //Debug.Log($"{_currentState[i]}, {_coolTimes[i]}");
            if (_enemyCoolTimes[i] > 0 && _enemyCurrentState[i] == eState.CoolTime)//현재상태 쿨타임, 쿨 도는중
            {
                _enemyCoolTimes[i] -= Time.deltaTime;
            }
            else if (_enemyCoolTimes[i] <= 0 && _enemyCurrentState[i] == eState.CoolTime) //현재상태 쿨타임, 쿨은 다 돔. => Usable로 변경
            {
                _enemyCurrentState[i] = eState.Usable;
            }
        }
    }
    public void SetSkillPrefabPool(SkillDataEntity[] dataEntity)
    {
        _skillDataDntity = dataEntity;
        _prefab = new SkillObject[dataEntity.Length];
        for (int i = 0; i <dataEntity.Length; i++)
        {
            SkillObject obj = Resources.Load<SkillObject>(Paths.SkillObject + dataEntity[0].Keyword);
            _prefab[i] = obj;
        }
        InitPool();

        for (int i = 0; i < dataEntity.Length; i++)
        {
            if (dataEntity[i] != null)
            {
                _enemyCoolTimes[i] = dataEntity[i].CoolTime / 2f;
                _enemyCurrentState[i] = eState.CoolTime;
            }

        }
    }
    public void EnemyUseSkill(int index)
    {
        if (_enemyCurrentState[index] == eState.CoolTime)
            return;

        SkillDataEntity data = InitController.Instance.GamePlays.SkillDatas[index];
        if (data == null) return;

        _enemyCurrentState[index] = eState.CoolTime;
        //_enemyCoolTimes[index] = 

        if (InitController.Instance.GamePlays.CurrentEnemyList.Count <= 0) return;
        else
        {
            _enemyBase = InitController.Instance.GamePlays.CurrentEnemyList[0];
        }
        _playerBase = InitController.Instance.GamePlays.GetUserCharacter;


        int damage = _enemyBase.GetNormalAttackDamageCalc(1);
        DamageStruct stat = new DamageStruct
        {
            Atk = (int)(_enemyBase.CurrentEnemyData.Atk * _enemyBase.CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Dmg = (int)(damage * _enemyBase.CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Dmg]),
            CritChance = (int)(_enemyBase.CurrentEnemyData.CritChance * 100 * _enemyBase.CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            CritDmg = (int)(_enemyBase.CurrentEnemyData.CritDmg * 100 * _enemyBase.CurrentBattleStat.CalculatedStatModifierDic[eStatInfo.Atk]),
            Key = data.Keyword,
            DamageType = data.DamageType,
            TriggerType = eTriggerType.SkillAttack,
            HitCount = data.HitCount,
        };

    }
}
