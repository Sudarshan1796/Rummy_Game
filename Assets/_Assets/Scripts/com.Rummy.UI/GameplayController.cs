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
        [SerializeField] private GameObject gameplayObject;
        [SerializeField] private GameObject[] cardPosition;
        [SerializeField] private GameObject cardInitPosition;
        [SerializeField] private Button btnDrop, btnExit;
        //this is for the first person this will be our player
        [SerializeField] private PlayerUIController playerController;
        //this contains all other players
        [SerializeField] private List<PlayerUIController> gamePlayers;
        [SerializeField] private GameObject closedCard;
        [SerializeField] private GameObject discardPile;

        //List of all players
        private Dictionary<int, PlayerUIController> activePlayers;
        private List<Gameplay.Card> cards;
        private List<GameObject> cardgameObject;

        internal bool isPlayerTurn;
        private void Awake()
        {
            cards = new List<Gameplay.Card>();
            cardgameObject = new List<GameObject>();
            activePlayers = new Dictionary<int, PlayerUIController>();
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
        /// Activate Gameplay screen
        /// </summary>
        internal void Activate()
        {
            gameplayObject.SetActive(true);
        }

        /// <summary>
        /// Deactivate Gameplay Screen
        /// </summary>
        internal void Deactivate()
        {
            gameplayObject.SetActive(false);
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
        internal void SetScreenData(List<Player> players)
        {
            SetPlayerDetail(players);
        }

        private void SetPlayerDetail(List<Player> players)
        {
            int _opponentPlayerCount = 0;
            activePlayers.Clear();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].userId == int.Parse(GameVariables.userId))
                {
                    AddPlayer(players[i], playerController);
                }
                else
                {
                    AddPlayer(players[i], gamePlayers[_opponentPlayerCount]);
                    _opponentPlayerCount++;
                }
            }

            void AddPlayer(Player player, PlayerUIController playerUiController)
            {
                playerUiController.SetDetails(player);
                if (!activePlayers.ContainsKey(player.userId))
                {
                    activePlayers.Add(player.userId, playerController);
                }
            }

        }

        /// <summary>
        ///  On individual player joins
        /// </summary>
        /// <param name="player"></param>
        internal void OnPlayerJoin(Player player)
        {
            var index = activePlayers.Count;
            if(index>0)
            {
                index = index - 1;
            }
            gamePlayers[index].SetDetails(player);
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

        /// <summary>
        /// this Functione is added to the discard pile panel 
        /// </summary>
        public void OnPlayerCardSelect()
        {
            if (isPlayerTurn)
            {

            }
        }
        internal void PlayerDrawCard(Player player)
        {

        }

        private void MoveCard(GameObject card, Vector3 destinationPosition)
        {
            var leanTweenObject = LeanTween.move(gameObject, destinationPosition, 1.0f).setEase(LeanTweenType.linear);
        }
    }
}