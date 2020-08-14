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
            PlayerPrefs.DeleteAll();
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
            UserGetProfile();
        }

        internal void SetScreenOrientation(ScreenOrientation orientationType)
        {
            Screen.orientation = orientationType;
        }

        private void UserGetProfile()
        {
            RESTApiConnectionManager.GetInstance.UserGetProfile<UserGetProfile>(OnGetUserProfileSuccess, OnGetUserProfileSuccessFail);
        }

        private void OnGetUserProfileSuccess(UserGetProfile userGetProfileResponse)
        {
            Debug.Log("User Profile Loaded");
        }

        private void OnGetUserProfileSuccessFail(string url, string errorMessage)
        {
            Debug.Log(url + "\n" + errorMessage);
        }
    }
}
