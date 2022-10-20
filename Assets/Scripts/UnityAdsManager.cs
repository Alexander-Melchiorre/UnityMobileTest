using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine.UI;
using TMPro;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class UnityAdsManager : MonoBehaviour
{
    public string GAME_ID = "4975382"; //optional, we will autofetch the gameID if the project is linked in the dashboard
    IRewardedAd m_AD;
    IInterstitialAd n_AD;
    public TextMeshProUGUI counter_field;
    int counter = 0;

    public void OnInitializeClicked()
    {
        Initialize();
    }

    public async void Initialize()
    {
        try
        {
            InitializationOptions opt = new InitializationOptions();
            opt.SetGameId(GAME_ID);

            await UnityServices.InitializeAsync(opt);
            OnInitializationComplete();
        }
        catch (Exception e)
        {
            OnInitializationFailed(e);
        }
    }

    public async void LoadRewardedAd()
    {
        string rewardedAdUnitId = "myRewardedAdUnitId";
        IRewardedAd rewardedAd;

        rewardedAd = MediationService.Instance.CreateRewardedAd(rewardedAdUnitId);
        m_AD = rewardedAd;
        try
        {
            rewardedAd.OnLoaded += OnUnityAdsAdLoaded;
            rewardedAd.OnFailedLoad += OnUnityAdsFailedToLoad;
            rewardedAd.OnShowed += OnUnityAdsShowStart;
            rewardedAd.OnFailedShow += OnUnityAdsShowFailure;
            rewardedAd.OnClicked += OnUnityAdsShowClick;
            rewardedAd.OnClosed += OnUnityAdsClosed;
            rewardedAd.OnUserRewarded += OnUserRewarded;
            await rewardedAd.LoadAsync();
        }
        catch (LoadFailedException) { }
    }

    //this would likely be hooked to a UI button or a game event
    public async void ShowRewardedAd()
    {
        // Ensure the ad has loaded, then show it
        if (m_AD.AdState == AdState.Loaded)
        {
            try
            {
                await m_AD.ShowAsync();
            }
            catch (Exception e)
            {
                //handle failure here
            }
        }
    }

    //Interstitial Ads
    public async void LoadInterstitialAd()
    {
        string interstitialAdUnitId = "myRewardedAdUnitId";
        IInterstitialAd interstitialAd;

        interstitialAd = MediationService.Instance.CreateInterstitialAd(interstitialAdUnitId);
        n_AD = interstitialAd;
        try
        {
            interstitialAd.OnLoaded += OnUnityAdsAdLoaded;
            interstitialAd.OnFailedLoad += OnUnityAdsFailedToLoad;
            interstitialAd.OnShowed += OnUnityAdsShowStart;
            interstitialAd.OnFailedShow += OnUnityAdsShowFailure;
            interstitialAd.OnClicked += OnUnityAdsShowClick;
            interstitialAd.OnClosed += OnUnityAdsClosed;
            await interstitialAd.LoadAsync();
        }
        catch (LoadFailedException) { }
    }

    //this would likely be hooked to a UI button or a game event
    public async void ShowInterstitialAd()
    {
        // Ensure the ad has loaded, then show it
        if (n_AD.AdState == AdState.Loaded)
        {
            try
            {
                await n_AD.ShowAsync();
            }
            catch (Exception e)
            {
                //handle failure here
            }
        }
    }

    //----------------------

    public void OnInitializationComplete()
    {
        // We recommend to load right after initialization according to docs
        LoadRewardedAd();
        Debug.Log("Init Success");
    }

    public void OnInitializationFailed(Exception e)
    {
        SdkInitializationError initializationError = SdkInitializationError.Unknown;
        if (e is InitializeFailedException initializeFailedException)
        {
            initializationError = initializeFailedException.initializationError;
        }

        Debug.Log($"{initializationError}:{e.Message}");
    }


    public void OnUnityAdsAdLoaded(object sender, EventArgs e)
    {
        Debug.Log($"Load Success");
    }

    public void OnUnityAdsFailedToLoad(object sender, LoadErrorEventArgs e)
    {
        Debug.Log($"{e.Error}:{e.Message}");
    }

    public void OnUnityAdsShowFailure(object sender, ShowErrorEventArgs args)
    {
        Debug.Log($"Ad failed to show: {args.Error}");
    }

    public void OnUnityAdsShowStart(object sender, EventArgs args)
    {
        Debug.Log("Ad shown successfully.");
    }

    public void OnUnityAdsShowClick(object sender, EventArgs e)
    {
        Debug.Log("Ad show clicked");
    }

    public void OnUnityAdsClosed(object sender, EventArgs e)
    {
        Debug.Log("Ad Closed");
    }

    void OnUserRewarded(object sender, RewardEventArgs e)
    {
        counter++;
        counter_field.text = "Rewarded videos watched: " + counter;
        Debug.Log($"Received reward: type:{e.Type}; amount:{e.Amount}");
    }
}