using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Net.NetworkInformation;
using Ping = System.Net.NetworkInformation.Ping;

public class FrontAds : MonoBehaviour
{

    private void Start()
    {
        LoadInterstitialAd();
    }
    // These ad units are configured to always serve test ads. TODO 현재는 테스트 아이디. 나중에 실 배포때는 내껄로 ㄱㄱ
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif

    private InterstitialAd interstitialAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
                // Raised when the ad closed full screen content.
                RegisterEventHandlers(interstitialAd);
            });
    }
    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowAd()
    {
        if(interstitialAd != null)
        {
            StartCoroutine(ShowInterstitialAd());
            IEnumerator ShowInterstitialAd()
            {
                while(!interstitialAd.CanShowAd())
                {
                    yield return YieldCache.WaitForSeconds(0.2f);
                }
                interstitialAd.Show();
            }
        }
        else
        {
            Debug.Log("광고 없음");
            bool IsInternetConnected()
            {
                Ping ping = new Ping();
                try
                {
                    PingReply reply = ping.Send("www.google.com", 2000);
                    return (reply.Status == IPStatus.Success);
                }
                catch
                {
                    return false;
                }
            }


            if (IsInternetConnected() == false)
            {
                Debug.Log("인터넷 연결 안됨");
                InitController.Instance.UIs.CloseAllUIPopup();
                InitController.Instance.Scenes.ChangeScene(Constants.Lobby); //TODO 인터넷 연결 안하고 하는 사람들도.. 걍 전면광고는 패스 가능하게 하자. 나중에 추가할 보상형 광고만 보상 안주면 되니까.
            }

            return;
        }

        //if (interstitialAd != null && interstitialAd.CanShowAd())
        //{
        //    Debug.Log("Showing interstitial ad.");
        //    interstitialAd.Show();
        //}
        //else
        //{
        //    Debug.LogError("Interstitial ad is not ready yet.");
        //}
    }
    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            InitController.Instance.Sounds.PlaySFX(eSFX.Click_Enterance);
            InitController.Instance.UIs.CloseAllUIPopup();
            InitController.Instance.Scenes.ChangeScene(Constants.Lobby);
            interstitialAd.Destroy();
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            LoadInterstitialAd();
        };
    }
}
