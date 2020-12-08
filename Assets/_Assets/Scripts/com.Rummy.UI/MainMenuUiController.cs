using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using com.Rummy.GameVariable;
using com.Rummy.Network;

namespace com.Rummy.Ui
{
    public class MainMenuUiController : MonoBehaviour
    {
        [Header("Main Menu Panel")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button addCashButton;
        [SerializeField] private Button pool101Button;
        [SerializeField] private TextMeshProUGUI phoneNumberText;
        [Header("Pool 101 Panel")]
        [SerializeField] private TextMeshProUGUI pool101EntryFeeText;
        [SerializeField] private TextMeshProUGUI pool101PlayersOnlineText;

        private void Start()
        {
            pool101Button.onClick.AddListener(OnClickPool101Button);
        }

        private void OnClickPool101Button()
        {
            StopUpdatingGameTypeDynamicData();
            GameVariables.userSelectedGameMode = GameVariables.GameMode.Pool101;
            UiManager.GetInstance.EnableRoomJoinUi();
        }

        internal void EnableMainMenuPanel()
        {
            StartUpdatingGameTypeDynamicData();
            mainMenuPanel.SetActive(true);
        }

        internal void DisableMainMenuPanel()
        {
            mainMenuPanel.SetActive(false);
        }

        internal void ShowUserName()
        {
            phoneNumberText.text = GameVariables.UserProfile.user_name;
        }

        internal void StartUpdatingGameTypeDynamicData()
        {
            UiManager.GetInstance.StartUpdatingMainMenuGameTypeDynamicDataInIntervals();
        }

        internal void StopUpdatingGameTypeDynamicData()
        {
            UiManager.GetInstance.StopUpdatingMainMenuGameTypeDynamicDataInIntervals();
        }

        internal void UpdateGameTypeDynamicData(RoomListResponse roomListResponse)
        {
            if (roomListResponse.practiceGameInfo != null)
            {
                foreach (var gameinfo in roomListResponse.practiceGameInfo)
                {
                    if(gameinfo.gameMode == GameVariables.GameMode.Pool101)
                    {
                        pool101EntryFeeText.text = $"EntryFee {gameinfo.minFee} to {gameinfo.maxFee}";
                        pool101PlayersOnlineText.text = $"{gameinfo.activePlayers} Players Online";
                    }
                }
            }
            else
            {
                Debug.LogError("roomListResponse.practiceGameInfo is empty!");
            }
        }
    }
}
