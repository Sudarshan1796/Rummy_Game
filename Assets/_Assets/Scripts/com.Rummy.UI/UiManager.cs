using UnityEngine;
using com.Rummy.Network;
using com.Rummy.GameCore;
using System.Collections.Generic;

namespace com.Rummy.Ui
{
    public class UiManager : MonoBehaviour
    {
        private static UiManager instance;
        internal static UiManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UiManager>();
                }
                return instance;
            }
        }

        [SerializeField] private List<UiRotator> loadingUiController;
        [SerializeField] private LoginUiController loginUiController;
        [SerializeField] private MainMenuUiController mainMenuUiController;

        private UiRotator enabledLoadingUi;

        #region LoadingUi

        internal void EnableLoadingUi()
        {
            if(Screen.orientation == ScreenOrientation.Portrait)
            {
                enabledLoadingUi = loadingUiController[0];
                enabledLoadingUi.Enable(true);
            }
            else
            {
                enabledLoadingUi = loadingUiController[1];
                enabledLoadingUi.Enable(true);
            }
        }

        internal void DisableLoadingUi()
        {
            enabledLoadingUi.Enable(false);
        }

        #endregion

        #region LoginUi

        internal void EnableLoginUi()
        {
            loginUiController.EnableMobNumberInputPanel();
        }

        internal void OnSubmitMobNumber(string mobNumber)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserLogin<UserLoginResponse>(mobNumber, SystemInfo.deviceUniqueIdentifier, OnUserLoginSuccess, OnUserLoginFail);
        }

        private void OnUserLoginSuccess(UserLoginResponse userLoginResponse)
        {
            loginUiController.EnableOtpInputPanel(userLoginResponse.next_otp_time_limit);
            DisableLoadingUi();
        }

        private void OnUserLoginFail(string url, string errorMessage)
        {
            loginUiController.ShowErrorMessageInNumberInputPanel("Error Occured! \n Plese Try Again.");
            Debug.Log(url + "\n" + errorMessage);
            DisableLoadingUi();
        }

        internal void OnSubmitOtpNumber(string mobNumber, string otp)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserVerify<UserVerifyResponse>(mobNumber, otp, OnUserVerifySuccess, OnUserVerifyFail);
        }

        private void OnUserVerifySuccess(UserVerifyResponse userVerifyResponse)
        {
            GameManager.GetInstance.StoreLoginCredentials(userVerifyResponse.user_id.ToString(), userVerifyResponse.access_token);
        }

        private void OnUserVerifyFail(string url, string errorMessage)
        {
            loginUiController.ShowErrorMessageInOtpInputPanel("Error Occured! \n Plese Resend OTP.");
            Debug.Log(url + "\n" + errorMessage);
            DisableLoadingUi();
        }

        #endregion

        #region MainMenuUi

        internal void EnableMainMenuUi()
        {
            loginUiController.DisableOtpInputPanel();
            mainMenuUiController.EnableMainMenuPanel();
        }

        #endregion
    }
}
