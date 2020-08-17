using UnityEngine;
using com.Rummy.Network;
using com.Rummy.GameCore;
using System.Collections.Generic;
using com.Rummy.UI;
using com.Rummy.GameVariable;
using com.Rummy.Gameplay;
using System;

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
        [SerializeField] private ConfirmationPopup confirmationPopup;
        private UiRotator enabledLoadingUi;

        #region LoadingUi

        internal void EnableLoadingUi()
        {
            if (Screen.orientation == ScreenOrientation.Portrait)
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
            if(userVerifyResponse.code == GameVariables.CodeType.None)
            {
                GameManager.GetInstance.StoreLoginCredentials(userVerifyResponse.user_id.ToString(), userVerifyResponse.access_token);
            }
            else
            {
                switch (userVerifyResponse.code)
                {
                    case GameVariables.CodeType.InvalidOtp : loginUiController.ShowErrorMessageInOtpInputPanel("Please enter valid OTP");
                        break;
                    case GameVariables.CodeType.RoomIsActive : loginUiController.ShowErrorMessageInOtpInputPanel("Cannot join this room! \n room is active.");
                        break;
                    default : loginUiController.ShowErrorMessageInOtpInputPanel("Error Occured! \n Plese Resend OTP.");
                        break;
                }
            }
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

        internal void DisableMainMenu()
        {
            mainMenuUiController.DisableMainMenuPanel();
        }

        internal void ShowMainMenuUserName()
        {
            mainMenuUiController.ShowUserName();
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
            roomJoinUiController.EnableRoomJoinWaitingScreen(true, roomCreateResponse.room_code);
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
            Debug.Log(roomJoinResponse.room_id+":"+roomJoinResponse.time_remaining);
            roomJoinUiController.EnableRoomJoinWaitingScreen(false);
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
            DisableMainMenu();
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
            if (GamePlayManager.GetInstance.isJoinedRoom)
            {
                PlayerLeftRequest playerLeftRequest = new PlayerLeftRequest
                {
                    room_id = GamePlayManager.GetInstance.roomId,
                    user_id = int.Parse(GameVariables.userId),
                };
                SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.playerLeft, playerLeftRequest);
                GamePlayManager.GetInstance.isJoinedRoom = false;
            }
        }

        #endregion

        #region GameplayUI
        internal void SetRoomJoinDetails(List<Player> players)
        {
            gameplayController.SetScreenData(players);
        }

        internal void OnPlayerJoinRoom(Player player)
        {
            gameplayController.OnPlayerJoin(player);
        }

        internal void EnableGameplayScreen()
        {
            gameplayController.Activate();
        }

        internal void DisableGamplayScreen()
        {
            gameplayController.Deactivate();
        }

        private void OnApplicationQuit()
        {
            LeaveSocketRoom();
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                //LeaveSocketRoom();
            }
        }

        internal void StartTimer(int userId,float timer,Action OnComplete)
        {
            gameplayController.StartPlayerTimer(userId, timer, OnComplete);
        }

        internal void OtherplayerDrawCard()
        {
            gameplayController.OnCardDraw();
        }
        #endregion

        #region ConfirmationPoup
        internal void ConfirmationPoup(string message, string headingText, Action successAction = null, Action failureAction = null)
        {
            confirmationPopup.ShowPopup(message, headingText, successAction, failureAction);
        }
        #endregion
    }
}
