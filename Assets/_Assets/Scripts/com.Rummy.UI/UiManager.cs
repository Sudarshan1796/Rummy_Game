using UnityEngine;
using com.Rummy.Network;
using com.Rummy.GameCore;

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

        [SerializeField] private LoginUiController loginUiController;

        #region LoginUi

        internal void EnableLoginUi()
        {
            loginUiController.EnableMobNumberInputPanel();
        }

        internal void OnSubmitMobNumber(string mobNumber)
        {
            RESTApiConnectionManager.GetInstance.UserLogin<UserLoginResponse>(mobNumber, SystemInfo.deviceUniqueIdentifier, OnUserLoginSuccess, OnUserLoginFail);
        }

        private void OnUserLoginSuccess(UserLoginResponse userLoginResponse)
        {
            loginUiController.EnableOtpInputPanel(userLoginResponse.next_otp_time_limit);
        }

        private void OnUserLoginFail(string url, string errorMessage)
        {
            loginUiController.ShowErrorMessageInNumberInputPanel("Error Occured! \n Plese Try Again.");
            Debug.Log(url + "\n" + errorMessage);
        }

        internal void OnSubmitOtpNumber(string mobNumber, string otp)
        {
            RESTApiConnectionManager.GetInstance.UserVerify<UserVerifyResponse>(mobNumber, otp, OnUserVerifySuccess, OnUserVerifyFail);
        }

        private void OnUserVerifySuccess(UserVerifyResponse userVerifyResponse)
        {
            GameManager.GetInstance.StoreLoginCredentials(userVerifyResponse.user_id.ToString(), userVerifyResponse.access_token);
            loginUiController.DisableOtpInputPanel();
        }

        private void OnUserVerifyFail(string url, string errorMessage)
        {
            loginUiController.ShowErrorMessageInOtpInputPanel("Error Occured! \n Plese Resend OTP.");
            Debug.Log(url + "\n" + errorMessage);
        }

        #endregion
    }
}
