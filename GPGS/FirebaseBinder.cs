using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Services.Analytics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseBinder : MonoBehaviour
{
    [SerializeField] Animator _loadingImage;
    [SerializeField] Button _googleLogingButton;
    [SerializeField] GameObject _loginResult, _panelImage;
    [SerializeField] TextMeshProUGUI _loginResultText;
    [SerializeField] Canvas _canvas;
    string _authCode;
    private void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    public void InitFirebaseAuth(Action OnLoginSuccess)
    {
        DontDestroyOnLoad(this.gameObject);
        Action panelInactive = () => _panelImage.SetActive(false);
        OnLoginSuccess -= panelInactive;
        OnLoginSuccess += panelInactive;
        _googleLogingButton.onClick.AddListener(() =>
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            _googleLogingButton.gameObject.SetActive(false);
            GoogleLogin(OnLoginSuccess);
        });
        _googleLogingButton.gameObject.SetActive(false);

        _loadingImage.gameObject.SetActive(false);


        _loginResult.SetActive(false);
        _googleLogingButton.gameObject.SetActive(true);
    }
    void GoogleLogin(Action OnLoginSuccess)
    {
        _loginResult.SetActive(false);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowLoginErrorResult("No internet connection");
            return;
        }
#if UNITY_EDITOR

        OnLoginSuccess?.Invoke();
#elif UNITY_ANDROID
        try
        {
            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                if (status == SignInStatus.Success)
                {
                    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.Result == Firebase.DependencyStatus.Available)
                        {
                            Debug.Log("Available");
                            ConnectToFireBase(OnLoginSuccess);
                        }
                        else
                        {
                            ShowLoginErrorResult("Dependency Error");
                        }
                    });
                }
                else
                {
                    ShowLoginErrorResult("Authenticate failed");
                }
            });
        }
        catch (Exception e)
        {
            ShowLoginErrorResult("Exception " + e.ToString());
        }
#endif
    }

    void ConnectToFireBase(Action OnLoginSuccess)
    {
        _loadingImage.gameObject.SetActive(true);


        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            if(!string.IsNullOrEmpty(code))
            {
                _authCode = code;
                FirebaseAuth fbAuth = FirebaseAuth.DefaultInstance;
                Credential fbCred = PlayGamesAuthProvider.GetCredential(_authCode);

                fbAuth.SignInWithCredentialAsync(fbCred).ContinueWithOnMainThread(task =>
                {
                    _loadingImage.gameObject.SetActive(false);
                    if (task.IsCanceled)
                    {
                        ShowLoginErrorResult("Sign in Canceled");
                        return;
                    }
                    else if (task.IsFaulted)
                    {
                        int id = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (Exception innerEx in task.Exception.InnerExceptions)
                        {
                            sb.AppendLine(id + "::Error::" + innerEx.Message);
                            id++;
                        }
                        ShowLoginErrorResult(sb.ToString());
                        return;
                    }
                    FirebaseUser user = fbAuth.CurrentUser;
                    if (user != null)
                    {
                        Debug.Log(user.DisplayName + " =---------------------------");
                        OnLoginSuccess?.Invoke();
                        //AnalyticsService.Instance.CustomData("Login", new Dictionary<string, object>
                        //{
                        //    {"DisplayName", user.DisplayName },
                        //    {"SigninStamp", user.Metadata.LastSignInTimestamp }
                        //});
                        FirebaseAnalytics.LogEvent("Login");
                        Parameter[] param = {
                            new Parameter("sessionid", user.UserId),
                            new Parameter("logindata", Time.time )
                        };
                        FirebaseAnalytics.LogEvent("id", param);
                    }
                    else
                    {
                        ShowLoginErrorResult("Error in userdata");
                    }
                    Debug.Log("---------------------3");
                });
            }
            else
            {
                ShowLoginErrorResult("RequestServerSideAccess failed. Code is null or empty");
            }

        });
    }


    void ShowLoginErrorResult(string result)
    {
        _loginResult.SetActive(true);
        _loginResultText.text = result;
        _googleLogingButton.gameObject.SetActive(true);
        Debug.Log(result);
    }
}
