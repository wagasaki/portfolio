using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private bool _isReady = false;
#pragma warning disable 0649
    [SerializeField]
    private CanvasGroup mFadeImage;
    private float mFadeTime = 0.5f;
    [SerializeField]
    private GameObject mLoading;
    [SerializeField]
    private Text mLoadingPercentText, mLoadingText, mLoadingToolTipText;
    [SerializeField]
    private Image mQuitPanel;
#pragma warning restore 0649

    public delegate void SceneNameInputCallback(string sceneName, eBackSound backSound, bool isDecrescendo = true);
    public SceneNameInputCallback OnSceneLoadCompleted { get; set; }
    public SceneNameInputCallback OnSceneExitStarted{get;set;}
    public string GetActiveScene { get { return SceneManager.GetActiveScene().name; } }

    public bool InitSceneController()
    {
        if (_isReady) return _isReady;

        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _isReady = true;
        return _isReady;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        mFadeImage.DOFade(0, mFadeTime)
        .OnStart(() => {

        })
        .OnComplete(() => {
            mLoading.SetActive(false);
            mFadeImage.blocksRaycasts = false;
        });
    }
    public void ChangeScene(string id, bool isnew = false)
    {
        Debug.Log("ChangeScene");
        mFadeImage.DOFade(1, mFadeTime).
            OnStart(() =>
            {
                mLoading.SetActive(true);
                mLoadingPercentText.text = "";
                mLoadingToolTipText.text = "";
                mFadeImage.blocksRaycasts = true;
            }).
            OnComplete(() =>
            {
                StartCoroutine(LoadScene(id, isnew));
                mLoadingText.DOText("Now Loading...", 1f).SetLoops(1).OnKill(() => mLoadingText.text = ""); //HACK 이거 OnStart에서 비워도되는데 걍 써본거임. 혹시 버그나면 옮기고
                mLoadingText.color = Color.white;
                mLoadingText.DOFade(0.5f, 0.5f).SetLoops(1).OnKill(() => mLoadingText.color = Color.white); //HACK 마찬가지
                //mLoadingToolTipText.text = "ToolTips  :  " + mToolTips[Random.Range(0, mToolTips.Length)].Contents;
            });
    }
    IEnumerator LoadScene(string id, bool isnew = false)
    {
        mLoading.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(id);
        async.allowSceneActivation = false;
        float past_time = 0;
        float percentage = 0;

        OnSceneExitStarted?.Invoke(id, eBackSound.None);
        Debug.Log("LoadScene : "+id);

        while ((async.isDone==false))
        {
            yield return null;

            past_time += Time.deltaTime;

            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if (percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료

                    if (async.isDone)
                    {
                        InitController.Instance.OnChangeSceneInitContorller(id, isnew);

                        if (id == Constants.Main)
                        {
                            OnSceneLoadCompleted?.Invoke(id, InitController.Instance.GamePlays.CurrentMapWayData.BackSound);
                        }
                        else
                        {
                            OnSceneLoadCompleted?.Invoke(id, eBackSound.None);
                        }

                        Debug.Log($"{id} 씬전환완료");
                        yield break;
                    }
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }
            mLoadingPercentText.text = percentage.ToString("N0") + " %";
        }
        //TextPopupManager.Instance.gameObject.SetActive(true);
    }


}
