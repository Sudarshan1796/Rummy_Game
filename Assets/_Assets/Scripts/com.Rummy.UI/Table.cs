using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.Ui
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI entryFeeText;
        [SerializeField] private TextMeshProUGUI openTableText;
        [SerializeField] private TextMeshProUGUI onlinePlayersText;

        public Button playNowButton;

        internal Action<string, string> onClickPlayNowButton;

        private string entryFee;
        private string maxPlayersPerTable;

        private void Start()
        {
            playNowButton.onClick.AddListener(OnClickPlayNowButton);
        }

        private void OnClickPlayNowButton()
        {
            onClickPlayNowButton?.Invoke(entryFee, maxPlayersPerTable);
        }

        internal void UpdateData(string entryFee, int userInTable, int maxPlayersPerTable, string onLinePlayers)
        {
            this.entryFee = entryFee;
            this.maxPlayersPerTable = maxPlayersPerTable.ToString();

            entryFeeText.text = entryFee;
            openTableText.text = $"Open Table: openTable: {userInTable}/{maxPlayersPerTable}";
            onlinePlayersText.text = $"{onLinePlayers} players online";
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
