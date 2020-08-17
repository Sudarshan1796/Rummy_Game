using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using com.Rummy.GameVariable;

namespace com.Rummy.Ui
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button addCashButton;
        [SerializeField] private Button pool101Button;
        [SerializeField] private TextMeshProUGUI phoneNumberText;

        private void Start()
        {
            pool101Button.onClick.AddListener(OnClickPool101Button);
        }

        private void OnClickPool101Button()
        {
            GameVariables.userSelectedGameMode = GameVariables.GameMode.Pool101;
            UiManager.GetInstance.EnableRoomJoinUi();
        }

        internal void EnableMainMenuPanel()
        {
            mainMenuPanel.SetActive(true);
        }

        internal void ShowUserName()
        {
            phoneNumberText.text = GameVariables.UserProfile.mob_no.ToString();
        }
    }
}
