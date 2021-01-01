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
            Application.targetFrameRate = Screen.currentResolution.refreshRate;

            if (PlayerPrefs.GetInt("isLoggedIn", 0) == 0)
            {
                SetScreenOrientation(ScreenOrientation.Portrait);
                UiManager.GetInstance.EnableLoginUi();
            }
            else
            {
                SetScreenOrientation(ScreenOrientation.LandscapeLeft);
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

            if(orientationType == ScreenOrientation.LandscapeLeft)
            {
                Screen.autorotateToPortrait = false;
                Screen.autorotateToLandscapeLeft = true;
            }
            else if(orientationType == ScreenOrientation.Portrait)
            {
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToPortrait = true;
            }
            Screen.orientation = orientationType;
        }

        private void UserGetProfile()
        {
            UiManager.GetInstance.EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserGetProfile<UserGetProfileResponse>(OnGetUserProfileSuccess, OnGetUserProfileFail);
        }

        private void OnGetUserProfileSuccess(UserGetProfileResponse userGetProfileResponse)
        {
            Debug.Log("<color=green>User Profile Loaded</color>");
            GameVariables.UserProfile = userGetProfileResponse;
            SetScreenOrientation(ScreenOrientation.LandscapeLeft);
            UiManager.GetInstance.EnableMainMenuUi();
            UiManager.GetInstance.ShowMainMenuUserName();
            UiManager.GetInstance.UpdateUserProfileData();
            Invoke(nameof(BufferTimeToRotateScreen), 1);
        }

        private void BufferTimeToRotateScreen()
        {
            UiManager.GetInstance.DisableLoadingUi();
        }

        private void OnGetUserProfileFail(string url, string errorMessage)
        {
            Debug.Log(url  + "\n" + errorMessage);
        }
    }
}
