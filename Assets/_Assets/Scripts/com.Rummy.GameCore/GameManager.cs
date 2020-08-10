using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.Rummy.GameVariable;
using com.Rummy.Ui;

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
                //UiManager.GetInstance.EnableLoginUi();
            }
            else
            {
                GameVariables.userId = PlayerPrefs.GetString("userId");
                GameVariables.AccessToken = PlayerPrefs.GetString("accessToken");
            }
        }

        internal void StoreLoginCredentials(string userID, string accessToken)
        {
            PlayerPrefs.SetInt("isLoggedIn", 1);
            GameVariables.userId = userID;
            PlayerPrefs.SetString("userId", userID);
            GameVariables.AccessToken = accessToken;
            PlayerPrefs.SetString("accessToken", accessToken);
        }
    }
}
