using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelCanvas : UICanvas
{

    [SerializeField]
    private Image _panelImage;
    private WaitForSeconds _tics = new WaitForSeconds(0.5f);

    public Action _doneOnLoading;
    private Coroutine _fadeInOutRout;
    public override void InitUICanvas()
    {
        base.InitUICanvas();
        //gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (_fadeInOutRout != null)
        {
            StopCoroutine(_fadeInOutRout);
            _fadeInOutRout = null;
        }
        if(InitController.Instance !=null)
        {
            InitController.Instance.UIs.IsESC_Usable = true;
        }
    }
    public void FadeInOout(Action middleAction, Action endAction) 
    {
        _panelImage.color = Color.clear;
        _panelImage.gameObject.SetActive(true);

        _fadeInOutRout = StartCoroutine(FadeInOutRout(middleAction, endAction));
    }
    IEnumerator FadeInOutRout(Action middleAction, Action endAction)
    {
        Color color = Color.clear;
        yield return _tics;

        while (_panelImage.color.a <= 1)
        {
            color.a += Time.deltaTime * 5;
            _panelImage.color = color;
            yield return null;
        }
        middleAction?.Invoke();
        yield return _tics;

        while (_panelImage.color.a >= 0)
        {
            color.a -= Time.deltaTime * 5;
            _panelImage.color = color;
            yield return null;
        }

        gameObject.SetActive(false);
        endAction?.Invoke();
    }

    public void FadeOut()
    {
        _panelImage.color = Color.black;
        _panelImage.gameObject.SetActive(true);

        _fadeInOutRout = StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            Color color = Color.black;
            while (_panelImage.color.a >= 0)
            {
                color.a -= Time.deltaTime * 2;
                _panelImage.color = color;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
