using com.Rummy.Gameplay;
using com.Rummy.GameVariable;
using com.Rummy.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.UI
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private GameObject[] cardPosition;
        [SerializeField] private GameObject cardInitPosition;
        [SerializeField] private List<Text> userNames;
        [SerializeField] private List<Image> userProfileImages;
        [SerializeField] private Button btnDrop, btnExit;
        private List<Gameplay.Card> cards;
        private List<GameObject> cardgameObject;

        private void Awake()
        {
            cards = new List<Gameplay.Card>();
            cardgameObject = new List<GameObject>();
        }

        private void AddListners()
        {
            GamePlayManager.GetInstance.OnGameStart += createCard;
            btnDrop.onClick.AddListener(OnDropClick);
            btnExit.onClick.AddListener(OnRoomExit);
        }
        private void removeListeners()
        {
            GamePlayManager.GetInstance.OnGameStart -= createCard;
            btnDrop.onClick.RemoveListener(OnDropClick);
            btnExit.onClick.RemoveListener(OnRoomExit);
        }

        /// <summary>
        /// this is Temp card Creation script
        /// </summary>
        private void createCard(List<PlayerCard> playerCards)
        {
            for (int i = 0; i < 13; i++)
            {
                var _gameObject = CardController.GetInstance.GetObject(cardInitPosition.transform);
                var _card = _gameObject.GetComponent<Gameplay.Card>();
                _card.Init(i, playerCards[i].cardValue, playerCards[i].suitValue);
                cards.Add(_card);
                cardgameObject.Add(_gameObject);
            }
            StartCoroutine(PlayCardAnimation(()=>
            {
                CardGroupController.GetInstance.InitilizeGroup(cardgameObject, cards);
            }));
        }

        /// <summary>
        /// This is the Starting Card draw animation
        /// </summary>
        /// <param name="isDone"></param>
        /// <returns></returns>
        private IEnumerator PlayCardAnimation(Action isDone)
        {
            int i = 0;
            while (i < cards.Count)
            {
                cards[i].Move(cardPosition[i].transform.position);
                i++;
                yield return new WaitForSeconds(0.10f);
            }
            yield return new WaitForSeconds(0.50f);
            isDone.Invoke();
        }

        /// <summary>
        /// Set Gameplay related  Data
        /// </summary>
        /// <param name="players"></param>
        private void SetScreenData(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                userNames[i].text = players[i].userName;
            }
        }

        private void OnDropClick()
        {

        }

        private void OnRoomExit()
        {

        }

        private void OnCardDraw()
        {

        }
    }
}