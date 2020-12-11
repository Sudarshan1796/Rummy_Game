using UnityEngine;
using com.Rummy.Network;
using com.Rummy.GameCore;
using System.Collections.Generic;
using com.Rummy.UI;
using com.Rummy.GameVariable;
using com.Rummy.Gameplay;
using System;
using System.Collections;

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
        [SerializeField] private ProfileScreenController profileScreenController;
        [SerializeField] private RoomJoinUiController roomJoinUiController;
        [SerializeField] private GameplayController gameplayController;
        [SerializeField] private CommonPopUpUiController commonPopUpUiController;
        private UiRotator enabledLoadingUi;
        private Coroutine mainMenuGameTypeDataUpdatingCoroutine;
        private Coroutine randomRoomOpenTableDataUpdatingCoroutine;
        private readonly WaitForSeconds waitForOneSecond = new WaitForSeconds(3);
        internal bool isOpenTablesInstantiated = false;

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
            if (loginUiController.gameObject.activeSelf)
            {
                loginUiController.DisableOtpInputPanel();
            }
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

        internal void StartUpdatingMainMenuGameTypeDynamicDataInIntervals()
        {
            if (mainMenuGameTypeDataUpdatingCoroutine != null)
            {
                StopCoroutine(mainMenuGameTypeDataUpdatingCoroutine);
            }
            mainMenuGameTypeDataUpdatingCoroutine = StartCoroutine(GetMainMenuGameTypeDataInterval());
        }

        internal void StopUpdatingMainMenuGameTypeDynamicDataInIntervals()
        {
            StopCoroutine(mainMenuGameTypeDataUpdatingCoroutine);
            mainMenuGameTypeDataUpdatingCoroutine = null;
        }

        private IEnumerator GetMainMenuGameTypeDataInterval()
        {
            while (true)
            {
                RESTApiConnectionManager.GetInstance.RoomList<RoomListResponse>(OnSuccessRoomListForMainMenuUi);
                yield return waitForOneSecond;
            }
        }

        private void OnSuccessRoomListForMainMenuUi(RoomListResponse roomListResponse)
        {
            if (roomListResponse != null)
            {
                mainMenuUiController.UpdateGameTypeDynamicData(roomListResponse);
            }
            else
            {
                Debug.LogError("RoomListResponse is empty!");
            }
        }

        #endregion

        #region ProfileScreenUi

        internal void UpdateUserProfileData()
        {
           profileScreenController.UpdatePersonalDetails(string.IsNullOrEmpty(GameVariables.UserProfile.mob_no.ToString()) ? GameVariables.UserProfile.temp_mob_no.ToString() : GameVariables.UserProfile.mob_no.ToString(), !string.IsNullOrEmpty(GameVariables.UserProfile.mob_no.ToString()),
           string.IsNullOrEmpty(GameVariables.UserProfile.email) ? GameVariables.UserProfile.temp_email : GameVariables.UserProfile.email, !string.IsNullOrEmpty(GameVariables.UserProfile.email),
           GameVariables.UserProfile.user_name,
           string.IsNullOrEmpty(GameVariables.UserProfile.first_name) ? "-" : GameVariables.UserProfile.first_name,
           string.IsNullOrEmpty(GameVariables.UserProfile.last_name) ? "-" : GameVariables.UserProfile.last_name);
        }

        internal void UpdateEmail(string email)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserUpdateProfile<UserUpdateProfileResponse>(email, "", "", "", OnUserUpdateProfileSuccess, OnFailUserProfileUpdate);
        }

        internal void UpdateDisplayName(string displayName)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserUpdateProfile<UserUpdateProfileResponse>("", displayName, "", "", OnUserUpdateProfileSuccess, OnFailUserProfileUpdate);
        }

        internal void UpdateFullName(string firstName, string lastName)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.UserUpdateProfile<UserUpdateProfileResponse>("", "", firstName, lastName, OnUserUpdateProfileSuccess, OnFailUserProfileUpdate);
        }

        private void OnUserUpdateProfileSuccess(UserUpdateProfileResponse userUpdateProfileResponse)
        {
            GameVariables.UserProfile.email = userUpdateProfileResponse.email;
            GameVariables.UserProfile.temp_email = userUpdateProfileResponse.temp_email;
            GameVariables.UserProfile.user_name = userUpdateProfileResponse.user_name;
            GameVariables.UserProfile.first_name = userUpdateProfileResponse.first_name;
            GameVariables.UserProfile.last_name = userUpdateProfileResponse.last_name;
            mainMenuUiController.ShowUserName();

            profileScreenController.UpdatePersonalDetails(null, false,
                string.IsNullOrEmpty(userUpdateProfileResponse.email)?userUpdateProfileResponse.temp_email:userUpdateProfileResponse.email, !string.IsNullOrEmpty(userUpdateProfileResponse.email),
                userUpdateProfileResponse.user_name,
                string.IsNullOrEmpty(userUpdateProfileResponse.first_name)?"-":userUpdateProfileResponse.first_name,
                string.IsNullOrEmpty(userUpdateProfileResponse.last_name)?"-":userUpdateProfileResponse.last_name);

            profileScreenController.GoToOverviewScreen();
            DisableLoadingUi();
        }

        private void OnFailUserProfileUpdate(string url, string errorMessage)
        {
            profileScreenController.GoToOverviewScreen();
            DisableLoadingUi();
        }

        #endregion

        #region RoomJoinUi

        internal void EnableRoomJoinUi()
        {
            roomJoinUiController.EnableRoomTypeSelectionPanel();
        }

        internal void StartUpdatingRandomRoomOpenTableDataInIntervals()
        {
            if (randomRoomOpenTableDataUpdatingCoroutine != null)
            {
                StopCoroutine(randomRoomOpenTableDataUpdatingCoroutine);
            }
            isOpenTablesInstantiated = false;
            randomRoomOpenTableDataUpdatingCoroutine = StartCoroutine(GetRandomRoomTableDataInInterval());
        }

        internal void StopUpdatingRandomRoomOpenTableData()
        {
            StopCoroutine(randomRoomOpenTableDataUpdatingCoroutine);
            randomRoomOpenTableDataUpdatingCoroutine = null;
        }

        private IEnumerator GetRandomRoomTableDataInInterval()
        {
            while (true)
            {
                if (!isOpenTablesInstantiated)
                {
                    EnableLoadingUi();
                }
                RESTApiConnectionManager.GetInstance.RoomList<RoomListResponse>(OnSuccessRoomListForRoomJoinUi);
                yield return waitForOneSecond;
            }
        }

        private void OnSuccessRoomListForRoomJoinUi(RoomListResponse roomListResponse)
        {
            if (roomListResponse != null)
            {
                roomJoinUiController.UpdateRandomRoomTablesData(roomListResponse);
            }
            else
            {
                Debug.LogError("RoomListResponse is empty!");
            }
        }

        internal void CreateRoom()
        {
            EnableLoadingUi();
            roomJoinUiController.PrintRoomJoinErrorMessage("");
            RESTApiConnectionManager.GetInstance.RoomCreate<RoomCreateResponse>(true, ((short)GameVariables.userSelectedGameMode).ToString(), ((short)GameVariables.userSelectedRoomSize).ToString(), OnSuccessRoomCreate, OnFailRoomCreate);
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

        internal void JoinRoom(string roomId, string entryFee, string maxPlayers)
        {
            EnableLoadingUi();
            RESTApiConnectionManager.GetInstance.RoomJoin<RoomJoinResponse>(true, ((short)GameVariables.userSelectedGameMode).ToString(), maxPlayers, roomId, entryFee, OnSuccessRoomJoin, OnFailRoomJoin);
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
            GamePlayManager.GetInstance.IPlayinGame = true;
            gameplayController.Activate();
        }

        internal void EnableResultScreen()
        {
            gameplayController.EnableResultScreen();
        }

        internal void DisableResultScreen()
        {
            gameplayController.DisableResultScreen();
        }

        internal void SetResultScreeenData(RoundCompleteResponse response)
        {
            gameplayController.SetRoundCompleteData(response);
        }

        internal void SetDeclareScreeenData(DeclarResponse result)
        {
            gameplayController.SetDeclaredata(result);
        }

        internal void MoveDiscardedCard(PlayerCard playerCard,int userId)
        {
            gameplayController.MoveDiscardedCard(playerCard, userId);
        }

        internal void DisableGamplayScreen()
        {
            GamePlayManager.GetInstance.IPlayinGame = false;
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

        internal void OnCardDraw(CardDrawRes  response)
        {
            gameplayController.OnCardDraw(response);
        }
        #endregion

        #region ConfirmationPoup
        internal void ConfirmationPoup(string message, string headingText, Action successAction = null, Action failureAction = null)
        {
            commonPopUpUiController.ShowPopUp(headingText, message, true, true, successAction, failureAction);
        }
        #endregion
    }
}
