using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class LobbyUICharacter : MonoBehaviour
{
    private float _maxTop = -2.5f;
    private float _minBottom = -3.5f;
    private float _left = -1.8f;
    private float _right = 1.8f;
    private Vector2 _nextDest = Vector2.zero;
    private Animator _lobbyCharacterAnim;
    private GameObject _characterSpriteObj;
    private ILobbyState _currentState;
    public Vector2 GetNextDest { get{return new Vector2(Random.Range(_left, _right), Random.Range(_minBottom, _maxTop)); } }
    public Action OnIdleAction { get; set; }
    public Animator LobbyCharacterAnim => _lobbyCharacterAnim;
    public GameObject CharacterSpriteObj => _characterSpriteObj;
    private void Awake()
    {
        _characterSpriteObj = transform.Find("Character").gameObject;
        _lobbyCharacterAnim = transform.Find("Character").GetComponent<Animator>();
        transform.position = new Vector2(Random.Range(_left, _right), Random.Range(_minBottom, _maxTop));
        SetState(new IdleState_lobby());
    }
    private void Start()
    {
        WeaponDataEntity currentWeapon = InitController.Instance.SaveDatas.UserData.GetPresetDatas[InitController.Instance.SaveDatas.UserData.CurrentPrestIndex]._currentWeapon;
        SpriteRenderer weaponsprite = Utils.FindChild<SpriteRenderer>(_characterSpriteObj, "Weapon");
        if (currentWeapon == null)
        {
            weaponsprite.sprite = null;
        }
        else
        {
            weaponsprite.sprite = Resources.Load<Sprite>(Paths.WeaponSpriteData + "/" + currentWeapon.Index);
            if (currentWeapon.WeaponSize == eWeaponSize.Large)
                weaponsprite.transform.localScale = new Vector3(2, 2, 1);
            else
                weaponsprite.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
    }
    public void SetState(ILobbyState next)
    {
        //Debug.Log(next);
        if(_currentState != null)
        {
            _currentState.OnExit();
        }
        _currentState = next;
        _currentState.OnEnter(this);
    }

    private void Update()
    {
        _currentState.OnUpdate();
    }
    #region regacy onupdate
    /*
    private void CharacterMoveUpdate()
    {
        _progress += Time.deltaTime;
        if (_progress > 3 && _characterState != eLobbyCharacterState.Attack)
        {
            _characterState = eLobbyCharacterState.Attack;

            int count = Random.Range(0, 2);
            switch (count)
            {
                case 0:
                    _lobbyCharacterAnim.Play("Attack");
                    break;
                case 1:
                    _lobbyCharacterAnim.Play("Attack");
                    break;
                default:
                    break;
            }
        }
        else if (_progress > 5 && _characterState == eLobbyCharacterState.Attack)
        {
            _progress = 0;
            _characterState = eLobbyCharacterState.Move;
            _nextDest = new Vector2(Random.Range(_left, _right), Random.Range(_minBottom, _maxTop));

            if (_nextDest.x < transform.position.x)
            {
                _characterSpriteObj.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _characterSpriteObj.transform.localScale = new Vector3(1, 1, 1);
            }
            _lobbyCharacterAnim.Play("Run");
        }

        if (_characterState == eLobbyCharacterState.Move)
        {
            transform.position = Vector2.MoveTowards(transform.position, _nextDest, Time.deltaTime);

            if (Vector2.Distance(transform.position, _nextDest) <= 0.05f)
            {
                _characterState = eLobbyCharacterState.Idle;
                _lobbyCharacterAnim.Play("Idle");
                //transform.Find("TooltipCanvas").GetComponent<CharacterToolTip>().ShowTooltips();
                OnIdleAction?.Invoke();
            }
        }
        else if (_characterState == eLobbyCharacterState.Idle)
        {
            _lobbyCharacterAnim.Play("Idle");
        }
    }
    */
    #endregion
}

public class IdleState_lobby : ILobbyState
{
    LobbyUICharacter _unit;
    float _progress;
    public void OnEnter(LobbyUICharacter unit)
    {
        _unit = unit;
        _unit.LobbyCharacterAnim.Play(Constants.Idle);
        _unit.OnIdleAction?.Invoke();
        _progress = 0;
    }

    public void OnExit()
    {
        _unit = null;
    }

    public void OnUpdate()
    {
        _progress += Time.deltaTime;
        if(_progress > 3)
        {
            ILobbyState state = new AttackState_lobby();
            _unit.SetState(state);
        }
    }
}
public class AttackState_lobby : ILobbyState
{
    LobbyUICharacter _unit;
    float _progress;
    public void OnEnter(LobbyUICharacter unit)
    {
        _unit = unit;
        _unit.LobbyCharacterAnim.Play(Constants.Attack);
        _progress = 0;
    }

    public void OnExit()
    {
        _unit = null;
    }

    public void OnUpdate()
    {
        _progress += Time.deltaTime;
        if(_progress > 2)
        {
            _unit.SetState(new RunState_lobby());
        }
    }
}
public class RunState_lobby : ILobbyState
{
    LobbyUICharacter _unit;
    Vector2 _nextDest;
    public void OnEnter(LobbyUICharacter unit)
    {
        _unit = unit;
        _unit.LobbyCharacterAnim.Play(Constants.Run);
        _nextDest = _unit.GetNextDest;
        if (_nextDest.x < _unit.transform.position.x)
        {
            _unit.CharacterSpriteObj.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _unit.CharacterSpriteObj.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnExit()
    {
        _unit = null;
    }

    public void OnUpdate()
    {
        _unit.transform.position = Vector2.MoveTowards(_unit.transform.position, _nextDest, Time.deltaTime);
        if (Vector2.Distance(_unit.transform.position, _nextDest) <= 0.05f)
        {
            _unit.SetState(new IdleState_lobby());
        }
    }
}
