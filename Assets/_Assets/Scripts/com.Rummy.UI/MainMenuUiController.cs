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
        [SerializeField] private TextMeshProUGUI phoneNumberText;

        internal void EnableMainMenuPanel()
        {
            phoneNumberText.text = GameVariables.UserProfile.mob_no.ToString();
            mainMenuPanel.SetActive(true);
        }
    }
}
