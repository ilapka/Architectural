using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Infrastructure.Services.Ads
{
    public class AdsService : IAdsService, IUnityAdsListener
    {
        private const string AndroidGameId = "5290123";
        private const string IOSGameId = "5290122";
        
        private const string RewardedVideoPlacementId = "Rewarded_Android";
        
        private string _gameId;
        private Action _onVideoFinished;
        
        public event Action RewardedVideoReady;
        public int Reward => 13;

        public void Initialize()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _gameId = AndroidGameId;
                    break;
                
                case RuntimePlatform.IPhonePlayer:
                    _gameId = IOSGameId;
                    break;
                
                case RuntimePlatform.WindowsEditor:
                    _gameId = AndroidGameId;
                    break;
                
                default:
                    Debug.LogError($"Unsupported platform for ads {Application.platform}");
                    break;
            }
            
            Advertisement.AddListener(this);
            Advertisement.Initialize(_gameId);
        }

        public void ShowRewardedVideo(Action onVideoFinished)
        {
            Advertisement.Show(RewardedVideoPlacementId);
            _onVideoFinished = onVideoFinished;
        }

        public bool IsRewardedVideoReady =>
            Advertisement.IsReady(RewardedVideoPlacementId);
        
        public void OnUnityAdsReady(string placementId)
        {
            Debug.Log($"OnUnityAdsReady {placementId}");
            
            if(placementId == RewardedVideoPlacementId)
                RewardedVideoReady?.Invoke();
        }

        public void OnUnityAdsDidError(string message) =>
            Debug.LogError($"OnUnityAdsDidError {message}");

        public void OnUnityAdsDidStart(string placementId) =>
            Debug.Log($"OnUnityAdsDidStart {placementId}");

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            switch (showResult)
            {
                case ShowResult.Failed:
                    Debug.LogError($"OnUnityAdsDidFinish {showResult}");
                    break;
                case ShowResult.Skipped:
                    Debug.LogError($"OnUnityAdsDidFinish {showResult}");
                    break;
                case ShowResult.Finished:
                    Debug.Log($"OnUnityAdsDidFinish {showResult}");
                    if(placementId == RewardedVideoPlacementId)
                        _onVideoFinished?.Invoke();
                    break;
                default:
                    Debug.LogError($"OnUnityAdsDidFinish {showResult}");
                    break;
            }

            _onVideoFinished = null;
        }
    }
}