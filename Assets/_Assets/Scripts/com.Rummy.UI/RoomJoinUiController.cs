using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static com.Rummy.GameVariable.GameVariables;
using com.Rummy.GameVariable;
using System.Collections.Generic;

namespace com.Rummy.Ui
{
    public class RoomJoinUiController : MonoBehaviour
    {
        [Header("Room Type Selection Screen")]
        [SerializeField] private GameObject roomTypeSelectionScreen;
        [SerializeField] private Button randomRoomButton;
        [SerializeField] private Button customRoomButton;
        [SerializeField] private Button roomTypeSelectionScreenCloseButton;
        [Header("Custom Room Size Selection Screen")]
        [SerializeField] private GameObject customRoomSizeSelectionScreen;
        [SerializeField] private Button roomSize2Button;
        [SerializeField] private Button roomSize6Button;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMP_InputField roomIdInput;
        [SerializeField] private TextMeshProUGUI roomIdInputErrorText;
        [SerializeField] private Button customRoomSizeSelectionScreenCloseButton;
        [Header("Random Room Table Selection Screen")]
        public GameObject randomRoomTableSelectionScreen;
        public GameObject openTablePrefab;
        public Transform twoPlayersScrollRectContent;
        public Transform sixPlayersScrollRectContent;

        [SerializeField] private TextMeshProUGUI gameTypeText;
        [SerializeField] private Toggle twoPlayersToggle;
        [SerializeField] private Toggle sixPlayerToggle;
        [SerializeField] private ScrollRect twoPlayersScrollRect;
        [SerializeField] private ScrollRect sixPlayersScrollRect;
        [SerializeField] private Button randomRoomTableSelectionScreenCloseButton;
        [Header("Room Join Waiting Screen")]
        [SerializeField] private GameObject roomJoinWaitingScreen;
        [SerializeField] private TextMeshProUGUI roomIdText;
        [SerializeField] private TextMeshProUGUI timeRemainingText;
        [SerializeField] private TextMeshProUGUI playersRoomJoinedCountText;
        [SerializeField] private TextMeshProUGUI playersRoomJoinedText;
        [SerializeField] private Button roomJoinWaitingScreenCloseButton;

        internal List<Table> twoPlayersOpenTable = new List<Table>();
        internal List<Table> sixPlayersOpenTable = new List<Table>();

        private void Start()
        {
            randomRoomButton.onClick.AddListener(OnClickRandomRoomButton);
            customRoomButton.onClick.AddListener(OnClickCustomRoomButton);
            roomSize2Button.onClick.AddListener(OnClickRoomSize2Button);
            roomSize6Button.onClick.AddListener(OnClickRoomSize6Button);
            joinButton.onClick.AddListener(OnClickCustomRoomJoinButton);
            roomTypeSelectionScreenCloseButton.onClick.AddListener(OnClickRoomTypeSelectionScreenCloseButton);
            customRoomSizeSelectionScreenCloseButton.onClick.AddListener(OnClickCustomRoomSizeSelectionScreenCloseButton);
            randomRoomTableSelectionScreenCloseButton.onClick.AddListener(OnClickRandomRoomTableSelectionScreenCloseButton);
            roomJoinWaitingScreenCloseButton.onClick.AddListener(OnClickRoomJoinWaitingScreenCloseButton);
            twoPlayersToggle.onValueChanged.AddListener(OnClickTwoPlayersToggle);
            sixPlayerToggle.onValueChanged.AddListener(OnClickSixPlayersToggle);
        }

        private void OnClickRandomRoomButton()
        {
            userSelectedRoomType = RoomType.RandomRoom;
            EnableRandomRoomTableSelectionScreen();
        }

        private void OnClickCustomRoomButton()
        {
            userSelectedRoomType = RoomType.CustomRoom;
            EnableCustomRoomSizeSelectionScreen();
        }

        private void OnClickRoomTypeSelectionScreenCloseButton()
        {
            gameObject.SetActive(false);
            roomTypeSelectionScreen.SetActive(false);
        }

        private void OnClickRoomSize2Button()
        {
            userSelectedRoomSize = RoomSize.Players2;
            UiManager.GetInstance.CreateRoom();
        }

        private void OnClickRoomSize6Button()
        {
            userSelectedRoomSize = RoomSize.Players6;
            UiManager.GetInstance.CreateRoom();
        }

        private void OnClickCustomRoomJoinButton()
        {
            if (string.IsNullOrEmpty(roomIdInput.text) || string.IsNullOrWhiteSpace(roomIdInput.text))
            {
                PrintRoomJoinErrorMessage("Please enter a room id");
            }
            else
            {
                PrintRoomJoinErrorMessage("");
            }
            UiManager.GetInstance.JoinRoom(roomIdInput.text, null, ((short)userSelectedRoomSize).ToString());
        }

        private void OnClickCustomRoomSizeSelectionScreenCloseButton()
        {
            roomTypeSelectionScreen.SetActive(true);
            customRoomSizeSelectionScreen.SetActive(false);
        }

        private void OnClickRandomRoomTableSelectionScreenCloseButton()
        {
            UiManager.GetInstance.StopUpdatingRandomRoomOpenTableData();
            roomTypeSelectionScreen.SetActive(true);
            randomRoomTableSelectionScreen.SetActive(false);
            foreach(var table in twoPlayersOpenTable)
            {
                table.onClickPlayNowButton -= OnClickOpenTablePlayNowButton;
                table.playNowButton.onClick.AddListener(null);
                DestroyImmediate(table.gameObject, true);
            }
            foreach (var table in sixPlayersOpenTable)
            {
                table.onClickPlayNowButton -= OnClickOpenTablePlayNowButton;
                table.playNowButton.onClick.AddListener(null);
                DestroyImmediate(table.gameObject, true);
            }
            twoPlayersOpenTable.Clear();
            sixPlayersOpenTable.Clear();
        }

        private void OnClickRoomJoinWaitingScreenCloseButton()
        {
            UiManager.GetInstance.LeaveSocketRoom();
            customRoomSizeSelectionScreen.SetActive(true);
            roomJoinWaitingScreen.SetActive(false);
        }

        private void OnClickTwoPlayersToggle(bool status)
        {
            if(status)
                userSelectedRoomSize = RoomSize.Players2;
            twoPlayersScrollRect.gameObject.SetActive(status);
            sixPlayersScrollRect.gameObject.SetActive(!status);
        }

        private void OnClickSixPlayersToggle(bool status)
        {
            if(status)
                userSelectedRoomSize = RoomSize.Players6;
            sixPlayersScrollRect.gameObject.SetActive(status);
            twoPlayersScrollRect.gameObject.SetActive(!status);
        }

        internal void OnClickOpenTablePlayNowButton(string entryFee, string maxPlayers)
        {
            UiManager.GetInstance.JoinRoom(null, entryFee, maxPlayers);
        }

        internal void EnableRoomTypeSelectionPanel()
        {
            gameObject.SetActive(true);
            roomTypeSelectionScreen.SetActive(true);
        }

        internal void EnableCustomRoomSizeSelectionScreen()
        {
            customRoomSizeSelectionScreen.SetActive(true);
            roomTypeSelectionScreen.SetActive(false);
        }

        internal void EnableRandomRoomTableSelectionScreen()
        {
            UiManager.GetInstance.StartUpdatingRandomRoomOpenTableDataInIntervals();
            switch(userSelectedGameMode)
            {
                case GameMode.Points:
                    gameTypeText.text = "Points";
                    break;
                case GameMode.Pool101:
                    gameTypeText.text = "101 Pool";
                    break;
                case GameMode.Deals:
                    gameTypeText.text = "Deals";
                    break;
                case GameMode.Pool201:
                    gameTypeText.text = "201 Pool";
                    break;
            }
            randomRoomTableSelectionScreen.SetActive(true);
            roomTypeSelectionScreen.SetActive(false);
        }

        internal void EnableRoomJoinWaitingScreen(bool isHost, string roomCode = null)
        {
            if(isHost)
            {
                roomIdText.text = $"Room ID : {roomCode} \n Waiting for other players to join...";
            }
            else
            {
                roomIdText.text = "Waiting for other players to join...";
            }
            ResetDynamicText();
            roomJoinWaitingScreen.SetActive(true);
            customRoomSizeSelectionScreen.SetActive(false);
        }

        internal void DisableRoomJoinWaitScreen()
        {
            roomJoinWaitingScreen.SetActive(false);
            gameObject.SetActive(false);
        }

        internal void PrintRoomJoinErrorMessage(string message)
        {
            roomIdInputErrorText.text = message;
        }

        internal void PrintRoomJoinedPlayersCount(int count)
        {
            if(count == 1)
            {
                playersRoomJoinedCountText.text = $"{count} player joined room...";
            }
            else
            {
                playersRoomJoinedCountText.text = $"{count} players joined room...";
            }
        }

        internal void PrintRoomJoinedPlayerRoom(string name)
        {
            playersRoomJoinedText.text = "Recently joined player: " + name;
        }

        private void ResetDynamicText()
        {
            playersRoomJoinedText.text = "";
            playersRoomJoinedCountText.text = "";
        }

        private IEnumerator StartRoomJoinRemainingTimer(int remainingTime)
        {
            timeRemainingText.text = $"Time Remaining <color=blue>{remainingTime} sec</color>";
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                remainingTime--;
                timeRemainingText.text = $"Time Remaining <color=blue>{remainingTime} sec</color>";
                if (remainingTime == 0)
                {
                    roomJoinWaitingScreenCloseButton.interactable = true;
                    break;
                }
            }
        }
    }
}
