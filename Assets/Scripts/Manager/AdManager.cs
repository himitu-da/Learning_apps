using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
    #if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-8970659787384533/4949730579";
    #elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
    #else
        private string _adUnitId = "unused";
    #endif

    BannerView _bannerView;

    void Start()
    {
        MobileAds.Initialize(initStatus => {
            Debug.Log("Creating banner view");

            // Create a 320x50 banner at top of the screen
            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
        });


    }
}
