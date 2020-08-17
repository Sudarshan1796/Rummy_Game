using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.Rummy.GameVariable;
using com.Rummy.Ui;
using com.Rummy.Network;

namespace com.Rummy.GameCore
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        internal static GameManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("isLoggedIn", 0) == 0)
            {
                SetScreenOrientation(ScreenOrientation.Portrait);
                UiManager.GetInstance.EnableLoginUi();
            }
            else
            {
                SetScreenOrientation(ScreenOrientation.LandscapeRight);
                GameVariables.userId = PlayerPrefs.GetString("userId");
                GameVariables.AccessToken = PlayerPrefs.GetString("accessToken");
                Debug.Log($"<color=blue>User Id : {GameVariables.userId} || Access Token : { GameVariables.AccessToken}</color>");
                UiManager.GetInstance.EnableMainMenuUi();
                UserGetProfile();
            }
        }

        internal void StoreLoginCredentials(string userID, string accessToken)
        {
            PlayerPrefs.SetInt("isLoggedIn", 1);
            GameVariables.userId = userID;
            PlayerPrefs.SetString("userId", userID);
            GameVariables.AccessToken = accessToken;
            PlayerPrefs.SetString("accessToken", accessToken);
            Debug.Log($"<color=blue>User Id : {GameVariables.userId} || Access Token : { GameVariables.AccessToken}</color>");
            UserGetProfile();
        }

        internal void SetScreenOrientation(ScreenOrientation orientationType)
        {
            if(Screen.orientation == orientationType)
            {
                return;
            }
            Screen.orientation = orientationType;
        }

        private void UserGetProfile()
        {
            UiManager.GetInstance.EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserGetProfile<UserGetProfile>(OnGetUserProfileSuccess, OnGetUserProfileFail);
        }

        private void OnGetUserProfileSuccess(UserGetProfile userGetProfileResponse)
        {
            Debug.Log("<color=green>User Profile Loaded</color>");
            GameVariables.UserProfile = userGetProfileResponse;
            UiManager.GetInstance.DisableLoadingUi();
            SetScreenOrientation(ScreenOrientation.LandscapeRight);
            UiManager.GetInstance.EnableMainMenuUi();
            UiManager.GetInstance.ShowMainMenuUserName();
        }

        private void OnGetUserProfileFail(string url, string errorMessage)
        {
            Debug.Log(url + "\n" + errorMessage);
        }
    }
}
