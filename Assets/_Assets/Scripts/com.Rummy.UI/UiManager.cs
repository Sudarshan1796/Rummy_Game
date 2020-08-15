using UnityEngine;
using com.Rummy.Network;
using com.Rummy.GameCore;
using System.Collections.Generic;
using com.Rummy.UI;
using com.Rummy.GameVariable;
using com.Rummy.Gameplay;

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
        [SerializeField] private RoomJoinUiController roomJoinUiController;
        [SerializeField] private GameplayController gameplayController;

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
            loginUiController.ShowErrorMessageInNumberInputPanel("Error Occured!, Plese Try Again.");
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

        #region RoomJoinUi

        internal void EnableRoomJoinUi()
        {
            roomJoinUiController.EnableRoomTypeSelectionPanel();
        }

        internal void CreateRoom()
        {
            EnableLoadingUi();
            roomJoinUiController.PrintRoomJoinErrorMessage("");
            RESTApiConnectionManager.GetInstance.RoomCreate<RoomCreateResponse>(false, ((short)GameVariables.userSelectedGameMode).ToString(), ((short)GameVariables.userSelectedRoomSize).ToString(), OnSuccessRoomCreate, OnFailRoomCreate);
        }

        private void OnSuccessRoomCreate(RoomCreateResponse roomCreateResponse)
        {
            GameVariables.roomId = roomCreateResponse.room_id;
            roomJoinUiController.EnableRoomJoinWaitingScreen(roomCreateResponse.time_remaining);
            GamePlayManager.GetInstance.SocketRoomJoin(roomCreateResponse.room_id);
            DisableLoadingUi();
        }

        private void OnFailRoomCreate(string url, string errorMessage)
        {
            roomJoinUiController.PrintRoomJoinErrorMessage("Failed to join room!, Please try again.");
            DisableLoadingUi();
        }

        internal void JoinRoom(string roomId)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.RoomJoin<RoomJoinResponse>(roomId, OnSuccessRoomJoin, OnFailRoomJoin);
        }

        private void OnSuccessRoomJoin(RoomJoinResponse roomJoinResponse)
        {
            roomJoinUiController.EnableRoomJoinWaitingScreen(roomJoinResponse.time_remaining);
            GamePlayManager.GetInstance.SocketRoomJoin(roomJoinResponse.room_id);
            DisableLoadingUi();
        }

        private void OnFailRoomJoin(string url, string errorMessage)
        {
            Debug.Log(url + "\n" + errorMessage);
            DisableLoadingUi();
        }

        internal void DisableRoomJoinWaitScreen()
        {
            roomJoinUiController.DisableRoomJoinWaitScreen();
        }

        internal void PrintRoomJoinedPlayersCount(int count)
        {
            roomJoinUiController.PrintRoomJoinedPlayersCount(count);
        }

        internal void PrintRoomJoinedPlayerRoom(string name)
        {
            roomJoinUiController.PrintRoomJoinedPlayerRoom(name);
        }

        internal void LeaveSocketRoom()
        {

        }

        #endregion
        #region GameplayUI
        internal void EnableGameplayScreen(List<Player> players)
        {
            gameplayController.Activate();
            gameplayController.SetScreenData(players);
        }
        #endregion
    }
}
