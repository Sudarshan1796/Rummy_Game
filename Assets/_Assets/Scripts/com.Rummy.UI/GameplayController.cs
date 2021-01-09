using com.Rummy.Constants;
using com.Rummy.Gameplay;
using com.Rummy.GameVariable;
using com.Rummy.Network;
using com.Rummy.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Card = com.Rummy.Network.Card;

namespace com.Rummy.UI
{
    public class GameplayController : MonoBehaviour
    {
        private static GameplayController instance;

        [SerializeField] private GameObject gameplayObject;
        [SerializeField] private GameObject[] cardPosition;
        [SerializeField] private GameObject cardInitPosition;
        [SerializeField] private Button btnDrop, btnExit;
        [SerializeField] private GameObject dropPanel;

        //this is for the first person this will be our player
        [SerializeField] private PlayerUIController playerController;
        //this contains all other players
        [SerializeField] private List<PlayerUIController> gamePlayers;
        // [SerializeField] private GameObject closedCard;
        // [SerializeField] private GameObject discardPile;
        [SerializeField] private CardGroupController cardGroupController;
        //Result screen
        [SerializeField] private ResultScreen resultScreen;

        [SerializeField] private UICard showCardsCard;
        //Dummy Movable card
        // [SerializeField] private Gameplay.Card movableCard;

        //List of all players
        private Dictionary<int, PlayerUIController> activePlayers;
        private List<Gameplay.Card> cards;
        private List<GameObject> cardgameObject;

        internal bool isPlayerTurn;

        public static GameplayController GetInstance
        {
            get
            {
                if (instance==null)
                {
                    instance = FindObjectOfType<GameplayController>();
                }

                return instance;
            }
        }
        private void Awake()
        {
            cards = new List<Gameplay.Card>();
            cardgameObject = new List<GameObject>();
            activePlayers = new Dictionary<int, PlayerUIController>();
        }
        private void OnEnable()
        {
            AddListners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListners()
        {
            GamePlayManager.GetInstance.OnGameStart += createCard;
            CardGroupController.onCardSelect += CreateCard;
            btnDrop.onClick.AddListener(OnDropClick);
            btnExit.onClick.AddListener(OnRoomExit);
            CardGroupController.onCardDiscard += RemoveCard;
        }
        private void RemoveListeners()
        {
            if (GamePlayManager.GetInstance)
                GamePlayManager.GetInstance.OnGameStart -= createCard;
            CardGroupController.onCardSelect -= CreateCard;
            btnDrop.onClick.RemoveListener(OnDropClick);
            btnExit.onClick.RemoveListener(OnRoomExit);
            CardGroupController.onCardDiscard -= RemoveCard;
        }

        /// <summary>
        /// Activate Gameplay screen
        /// </summary>
        internal void Activate()
        {
            gameplayObject.SetActive(true);
            ResetPlayerTimer();
        }

        private void ResetPlayerTimer()
        {
            playerController.ResetTimerColor();
            foreach (var player in gamePlayers)
            {
                if (player.gameObject.activeSelf)
                {
                    player.ResetTimerColor();
                }
            }
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
            showCardsCard.gameObject.SetActive(false);
            DestroyAllCards();
            cardGroupController.DeactivateGroupText();
            cardGroupController.DeactivateAllButtons();
            UpdateDropPanel(false);
            for (int i = 0; i < playerCards.Count; i++)
            {
                var _gameObject = CardController.GetInstance.GetObject(cardInitPosition.transform);
                var _card = _gameObject.GetComponent<Gameplay.Card>();
                _card.Init(playerCards[i].cardValue, playerCards[i].suitValue);
                cards.Add(_card);
                cardgameObject.Add(_gameObject);
            }
            StartCoroutine(PlayCardAnimation(() =>
            {
                cardGroupController.InitilizeGroup(cardgameObject, cards);
            }));
        }

        private void DestroyAllCards()
        {
            for (int i = 0; i < cardgameObject.Count; i++)
            {
                Destroy(cardgameObject[i]);
            }
            cards.Clear();
            cardgameObject.Clear();
        }

        private void CreateCard(PlayerCard playerCard)
        {
            Debug.Log("player cards are" + playerCard.suitValue + "-" + playerCard.cardValue);
            var _gameObject = CardController.GetInstance.GetObject(cardInitPosition.transform);
            var _card = _gameObject.GetComponent<Gameplay.Card>();
            _card.Init(playerCard.cardValue, playerCard.suitValue);
            //cards.Add(_card);
            //cardgameObject.Add(_gameObject);
            CardGroupController.GetInstance.AddCardToGroup(_gameObject, _card);
        }


        private void RemoveCard(Gameplay.Card card)
        {
            if (cards.Contains(card))
            {
                var cardGameobject = cardgameObject[cards.IndexOf(card)];
                cardgameObject.RemoveAt(cards.IndexOf(card));
                cards.RemoveAt(cards.IndexOf(card));
                Destroy(cardGameobject);
            }
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
            yield return new WaitForSeconds(1.0f);
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
                    int index = players[i].position - GamePlayManager.GetInstance.playerPosition;
                    AddPlayer(players[i], gamePlayers[_opponentPlayerCount]);
                    _opponentPlayerCount++;
                }
            }

            void AddPlayer(Player player, PlayerUIController playerUiController)
            {
                playerUiController.SetDetails(player);
                if (!activePlayers.ContainsKey(player.userId))
                {
                    activePlayers.Add(player.userId, playerUiController);
                }
            }

        }

        /// <summary>
        ///  On individual player joins
        /// </summary>
        /// <param name="player"></param>
        internal void OnPlayerJoin(Player player)
        {
            if (!activePlayers.ContainsKey(player.userId))
            {
                var index = activePlayers.Count;
                if (index > 0)
                {
                    index = index - 1;
                }
                gamePlayers[index].SetDetails(player);

                activePlayers.Add(player.userId, gamePlayers[index]);
            }
        }



        private void OnDropClick()
        {

        }

        private void OnRoomExit()
        {
            UiManager.GetInstance.EnableMainMenuUi();
            UiManager.GetInstance.DisableGamplayScreen();
            UiManager.GetInstance.LeaveSocketRoom();
        }

        public void OnCardDraw(CardDrawRes response)
        {
            var player = playerController;
            if (response.userId != int.Parse(GameVariables.userId))
            {
                player = gamePlayers.Find(x => x.userId == response.userId);
            }
            cardGroupController.MoveCardDraw(response, player.transform);
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

        internal void StartPlayerTimer(int id, float timer, Action onComplete)
        {
            //this is for the first player 
            playerController.SetTimer(id, timer, onComplete);
            //this is for the other player
            gamePlayers[0].SetTimer(id, timer, onComplete);
            if (GamePlayManager.GetInstance.roomPlayers.Count < 2)
            {
                return;
            }
            gamePlayers[1].SetTimer(id, timer, onComplete);
            gamePlayers[2].SetTimer(id, timer, onComplete);
            gamePlayers[3].SetTimer(id, timer, onComplete);
            gamePlayers[4].SetTimer(id, timer, onComplete);

        }

        internal void OnPlayerLeft(int userid)
        {
            if(userid==int.Parse(GameVariables.userId))
            {
                playerController.GrayOutTimer();
                return;
            }
            foreach(var player in gamePlayers)
            {
                if(player.userId==userid)
                {
                    player.GrayOutTimer();
                }
            }
        }

        internal void MoveDiscardedCard(PlayerCard playerCard, int userId)
        {
            if (userId != int.Parse(GameVariables.userId))
            {
                var player = gamePlayers.Find(x => x.userId == userId);
                cardGroupController.MoveDiscardedCard(playerCard, player.transform);
            }
            else
            {
                cardGroupController.MoveUserCard(playerCard);
            }
        }

        internal void EnableResultScreen()
        {
            resultScreen.UpdateState(true);
        }

        internal void DisableResultScreen()
        {
            resultScreen.UpdateState(false);
        }

        internal void SetRoundCompleteData(RoundCompleteResponse response)
        {
            resultScreen.OnRoundComplete(response);
        }

        internal void SetDeclaredata(DeclarResponse result)
        {
            resultScreen.OnDeclareComplete(result);
        }

        internal void SetShowCard(Card card)
        {
            showCardsCard.gameObject.SetActive(true);
            showCardsCard.SetDetail(card.cardValue, card.suitValue);
        }
        internal void DisablePlayerTimer()
        {
            foreach(var player in gamePlayers)
            {
                player.DisableTimer();
            }
        }

        internal void UpdateDropPanel(bool value)
        {
            dropPanel.SetActive(value);
        }

        internal void SetPlayerLeftStatus(RoomStatusResponse response)
        {
            //GameConstants.SOCKET_URL_PREFIX;
            foreach (var player in response.roomInfo)
            {
                PlayerUIController v_Player = null;
                if (player.userId == int.Parse(GameVariables.userId))
                {
                    v_Player = playerController;
                }
                else
                {
                    v_Player = gamePlayers.Find(x => x.userId == player.userId);
                }
                if(v_Player== null)
                {
                    Debug.LogError("Setting Player status null error");
                    continue;
                }
                if (player.status == GameConstants.ROOM_PLAYER_ACTIVE)
                {
                    v_Player.ResetTimerColor();
                }
                else
                {
                    v_Player.GrayOutTimer();
                }
            }
        }
    }
}