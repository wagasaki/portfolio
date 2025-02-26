using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class TopBanner : MonoBehaviour
{
    Action OnAdsRemove;
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log(InitController.Instance.SaveDatas.UserData.GetIsAdsHide);
            if(InitController.Instance.SaveDatas.UserData.GetIsAdsHide == true)
            {
                Debug.Log("광고제거");
                return;
            }
            else
            {
                CreateBannerView();
                LoadAd();

                OnAdsRemove = () => DestroyAd();
                InitController.Instance.SaveDatas.OnAdsRemove -= OnAdsRemove;
                InitController.Instance.SaveDatas.OnAdsRemove += OnAdsRemove;
            }
        });
    }
    // These ad units are configured to always serve test ads. TODO 현재는 테스트 아이디. 나중에 실 배포때는 내껄로 ㄱㄱ
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
  private string _adUnitId = "unused";
#endif

    BannerView _bannerView;
    (float, float) GetBannerSize { get { return (_bannerView.GetWidthInPixels(), _bannerView.GetHeightInPixels()); } }
    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        //Debug.Log(AdSize.FullWidth);
        //AdSize adaptiveSize =
        //       AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(1080);
        //Debug.Log(adaptiveSize.Height + " ? " + adaptiveSize.Width);
        _bannerView = new BannerView(_adUnitId, AdSize.IABBanner, AdPosition.Top);
        //Debug.Log(_bannerView.GetHeightInPixels() + " / " + _bannerView.GetWidthInPixels());
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
            InitController.Instance.SaveDatas.OnAdsRemove -= OnAdsRemove;
        }
    }
}
