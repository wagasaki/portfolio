using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    Animator _anim;
    float _animPlayTime;
    private Coroutine _attackRoutine;
    public Action<string> OnAnimationComplete { get; set; }
    private void OnEnable()
    {
        if(_attackRoutine!=null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }
    private void OnDisable()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        RuntimeAnimatorController ac = _anim.runtimeAnimatorController;
        _animPlayTime = ac.animationClips[0].length;

        //AnimationClip clip = _anim.runtimeAnimatorController.animationClips[0];
        //if(clip.events.Length<1)
        //{
        //    AnimationEvent animationEndEvent = new AnimationEvent();
        //    animationEndEvent.time = clip.length;
        //    animationEndEvent.functionName = "AnimationCompleteHandler";
        //    animationEndEvent.stringParameter = clip.name;
        //    OnAnimationComplete = (string ar) => { Debug.Log("스킬끝"); gameObject.SetActive(false); };

        //    clip.AddEvent(animationEndEvent);
        //}


        #region 요런식으로 하면 될 듯. init에서 받게 될거임
        //TODO  일단, 스킬은 그냥 모두 단일 객체로 쓸거임. 예전처럼 여러개의 자식오브젝트로(메테오같은거) 제작하는건 최대한 지양함. 그래서 그냥 index0번 하나만 있을거라 이렇게 해도 될듯?
        //혹시 나중에 불가피하게 추가해야될 경우에는
        //RuntimeAnimatorController ac = _anim.runtimeAnimatorController;
        //for (int i = 0; i < ac.animationClips.Length; i++)
        //{
        //    if (ac.animationClips[i].name == data.Keyword)
        //    {
        //        time = ac.animationClips[i].length;
        //    }
        //}
        //Debug.Log(time);
        #endregion


    }
    public void InitAttackSkillObj(in DamageStruct attackstat, UnitBase target)
    {
        OnAnimationComplete = (string ar) => { Debug.Log("스킬끝"); gameObject.SetActive(false); };
        transform.position = target.transform.position;
        WaitForSeconds tick = new WaitForSeconds(_animPlayTime / attackstat.HitCount);

        if (target is UserCharacter || target is Enemy)
        {
            target.GetHit(attackstat,null, tick);
        }
        else
        {
            Debug.Log("target is neither player nor enemy.");
        }
    }

    public void InitBuffSkillObj(in DamageStruct attackstat, UnitBase target)
    {

    }
    public void BuffSkillObj()
    {

    }
    public void AnimationCompleteHandler(string name)
    {
        //Debug.Log($"{name} animation complete.");
        if(OnAnimationComplete==null)
        {
            Debug.Log("스킬끝_종료이벤트 비어있는 문제"); 
            gameObject.SetActive(false);
        }
        else
        {
            OnAnimationComplete.Invoke(name);
        }
    }

    public void FadeOut()
    {
        gameObject.SetActive(false);
    }
}
