using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeAnimation : MonoBehaviour
{
    public Action<string> OnAnimationStart { get; set; }
    public Action<string> OnAnimationComplete { get; set; }

    Animator _animator;
    private bool _isReady;

    private void Awake()
    {
        InitAnimation();
    }
    public void InitAnimation()
    {
        if (_isReady) return;
        _animator = GetComponent<Animator>();

        for (int i = 0; i < _animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            AnimationClip clip = _animator.runtimeAnimatorController.animationClips[i];
            if (clip.events.Length >= 2) continue;
            AnimationEvent animationStartEvent = new AnimationEvent
            {
                time = 0,
                functionName = "AnimationStartHandler",
                stringParameter = clip.name
            };

            AnimationEvent animationEndEvent = new AnimationEvent
            {
                time = clip.length,
                functionName = "AnimationCompleteHandler",
                stringParameter = clip.name
            };

            clip.AddEvent(animationStartEvent);
            clip.AddEvent(animationEndEvent);

            OnAnimationComplete = (string ar) => gameObject.SetActive(false);
        }
        _isReady = true;
    }



    public void AnimationStartHandler(string name)
    {
        //Debug.Log($"{name} animation start.");
        OnAnimationStart?.Invoke(name);
    }
    public void AnimationCompleteHandler(string name)
    {
        //Debug.Log($"{name} animation complete.");
        OnAnimationComplete?.Invoke(name);
    }
}
