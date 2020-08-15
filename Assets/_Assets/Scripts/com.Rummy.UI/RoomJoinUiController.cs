using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static com.Rummy.GameVariable.GameVariables;
using com.Rummy.GameVariable;

namespace com.Rummy.Ui
{
    public class RoomJoinUiController : MonoBehaviour
    {
        [Header("Room Type Selection Screen")]
        [SerializeField] private GameObject roomTypeSelectionScreen;
        [SerializeField] private Button randomRoomButton;
        [SerializeField] private Button customRoomButton;
        [SerializeField] private Button roomTypeSelectionScreenCloseButton;
        [Header("Room Size Selection Screen")]
        [SerializeField] private GameObject roomSizeSelectionScreen;
        [SerializeField] private Button roomSize2Button;
        [SerializeField] private Button roomSize6Button;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMP_InputField roomIdInput;
        [SerializeField] private TextMeshProUGUI roomIdInputErrorText;
        [SerializeField] private Button roomSizeSelectionScreenCloseButton;
        [Header("Room Join Waiting Screen")]
        [SerializeField] private GameObject roomJoinWaitingScreen;
        [SerializeField] private TextMeshProUGUI roomIdText;
        [SerializeField] private TextMeshProUGUI timeRemainingText;
        [SerializeField] private TextMeshProUGUI playersRoomJoinedCountText;
        [SerializeField] private TextMeshProUGUI playersRoomJoinedText;
        [SerializeField] private Button roomJoinWaitingScreenCloseButton;

        private void Start()
        {
            randomRoomButton.onClick.AddListener(OnClickRandomRoomButton);
            customRoomButton.onClick.AddListener(OnClickCustomRoomButton);
            roomSize2Button.onClick.AddListener(OnClickRoomSize2Button);
            roomSize6Button.onClick.AddListener(OnClickRoomSize6Button);
            joinButton.onClick.AddListener(OnClickCustomRoomJoinButton);
            roomTypeSelectionScreenCloseButton.onClick.AddListener(OnClickRoomTypeSelectionScreenCloseButton);
            roomSizeSelectionScreenCloseButton.onClick.AddListener(OnClickRoomSizeSelectionScreenCloseButton);
            roomJoinWaitingScreenCloseButton.onClick.AddListener(OnClickRoomJoinWaitingScreenCloseButton);
        }

        private void OnClickRandomRoomButton()
        {
            userSelectedRoomType = RoomType.RandomRoom;
            EnableRoomSizeSelectionScreen();
        }

        private void OnClickCustomRoomButton()
        {
            userSelectedRoomType = RoomType.CustomRoom;
            EnableRoomSizeSelectionScreen();
        }

        private void OnClickRoomTypeSelectionScreenCloseButton()
        {
            gameObject.SetActive(false);
            roomTypeSelectionScreen.SetActive(false);
        }

        private void OnClickRoomSize2Button()
        {
            userSelectedRoomSize = RoomSize.players2;
            UiManager.GetInstance.CreateRoom();
        }

        private void OnClickRoomSize6Button()
        {
            userSelectedRoomSize = RoomSize.players6;
            UiManager.GetInstance.CreateRoom();
        }

        private void OnClickCustomRoomJoinButton()
        {
            if(string.IsNullOrEmpty(roomIdInput.text) || string.IsNullOrWhiteSpace(roomIdInput.text))
            {
                PrintRoomJoinErrorMessage("Please enter a room id");
            }
            else
            {
                PrintRoomJoinErrorMessage("");
            }
            UiManager.GetInstance.JoinRoom(roomIdInput.text);
        }

        private void OnClickRoomSizeSelectionScreenCloseButton()
        {
            roomTypeSelectionScreen.SetActive(true);
            roomSizeSelectionScreen.SetActive(false);
        }

        private void OnClickRoomJoinWaitingScreenCloseButton()
        {
            UiManager.GetInstance.LeaveSocketRoom();
            roomSizeSelectionScreen.SetActive(true);
            roomJoinWaitingScreen.SetActive(false);
        }

        internal void EnableRoomTypeSelectionPanel()
        {
            gameObject.SetActive(true);
            roomTypeSelectionScreen.SetActive(true);
        }

        internal void EnableRoomSizeSelectionScreen()
        {
            roomSizeSelectionScreen.SetActive(true);
            roomTypeSelectionScreen.SetActive(false);
        }

        internal void EnableRoomJoinWaitingScreen(bool isHost)
        {
            if(isHost)
            {
                roomIdText.text = $"Room ID : {GameVariables.roomId} \n Waiting for other players to join...";
            }
            else
            {
                roomIdText.text = "Waiting for other players to join...";
            }
            ResetDynamicText();
            roomJoinWaitingScreen.SetActive(true);
            roomSizeSelectionScreen.SetActive(false);
        }

        internal void DisableRoomJoinWaitScreen()
        {
            roomJoinWaitingScreen.SetActive(false);
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
