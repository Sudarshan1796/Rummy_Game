using com.Rummy.GameVariable;
using com.Rummy.Network;
using com.Rummy.Ui;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.Gameplay
{
    public class CardGroupController : MonoBehaviour
    {
        public delegate void OnCardselect(PlayerCard card);
        public static event OnCardselect onCardSelect;

        public delegate void OnCardDeselect(Card card);
        public static event OnCardDeselect onCardDiscard;

        [SerializeField] private GameObject tempParemtObject;
        [SerializeField] private List<GameObject> cardGroupGameobject;
        [SerializeField] private List<CardGroup> cardGroups;
        [SerializeField] private Button createGroupBtn, sortCardBtn,discardBtn;
        [SerializeField] private GameObject createGroupPanel;
        [SerializeField] private RectTransform[] parentReactTans;
        [SerializeField] private TMP_Text[] groupSetText;
        [SerializeField] private GameObject declarePanel;
        [SerializeField] private Transform openDeckTransform;
        [SerializeField] private Transform closedDeckTransform;
        [SerializeField] private GameObject playerCardSelectTransform;
        [SerializeField] private GameObject movingCard;
        [SerializeField] private Card movingCardController;
        [SerializeField]private Transform prifileDestination;
        [SerializeField] private Card backCardController;
        [SerializeField] private Card OpenTileCard;

        private List<GameObject> cardGameobject;
        private List<Card> cards;
        private List<GameObject> inActiveGroups;
        private Dictionary<Card, GameObject> selectedObject;
        private Dictionary<GameObject, GameObject> childParentObject;
        private static CardGroupController instance;

        private GamePlayManager gameplayManager;
        public static CardGroupController GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CardGroupController>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
            selectedObject = new Dictionary<Card, GameObject>();
            cards = new List<Card>();
            cardGameobject = new List<GameObject>();
            childParentObject = new Dictionary<GameObject, GameObject>();
            inActiveGroups = new List<GameObject>();
            gameplayManager = GamePlayManager.GetInstance;
        }

        private void OnEnable()
        {
            createGroupBtn.onClick.AddListener(CheckCanCreateGroup);
            sortCardBtn.onClick.AddListener(SortCards);
            discardBtn.onClick.AddListener(OnCardDiscard);
        }

        private void OnDisable()
        {
            createGroupBtn.onClick.RemoveListener(CheckCanCreateGroup);
            sortCardBtn.onClick.RemoveListener(SortCards);
            discardBtn.onClick.RemoveListener(OnCardDiscard);
        }

        public void InitilizeGroup(List<GameObject> _cardObject, List<Card> _cards)
        {
            //this.cards.Clear();
            //this.cardGameobject.Clear();
            //selectedObject.Clear();
            RemoveAllSelectedCard();
            childParentObject.Clear();
            OpenTileCard.gameObject.SetActive(false);
            inActiveGroups.Clear();

            this.cards = _cards;
            this.cardGameobject = _cardObject;
            cardGroupGameobject[0].SetActive(true);
            for (int i = 0; i < _cardObject.Count; i++)
            {
                childParentObject.Add(_cardObject[i], cardGroupGameobject[0]);
                _cardObject[i].transform.SetParent(cardGroupGameobject[0].transform, false);
            }
            for (int i = 1; i < cardGroupGameobject.Count; i++)
            {
                cardGroupGameobject[i].SetActive(false);
                cardGroupGameobject[i].transform.SetSiblingIndex(i);
                inActiveGroups.Add(cardGroupGameobject[i]);
            }
            sortCardBtn.gameObject.SetActive(true);
        }

        public void AddCardToGroup(GameObject cardGameObj, Card cardController)
        {
            int index = -1;
            GameObject parentObject = null;

            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                if (cardGroupGameobject[i].activeSelf)
                {
                    if (cardGroupGameobject[i].transform.GetSiblingIndex() > index)
                    {
                        index = cardGroupGameobject[i].transform.GetSiblingIndex();
                        parentObject = cardGroupGameobject[i];
                    }
                }
            }
            childParentObject.Add(cardGameObj, parentObject);
            cardGameObj.transform.SetParent(parentObject.transform, false);
            cards.Add(cardController);
            cardGameobject.Add(cardGameObj);
        }

        internal void OnDragBegin(Card card, GameObject cardGameObject)
        {
            createGroupPanel.SetActive(CanCreateGroup());
            cardGameObject.transform.SetParent(tempParemtObject.transform, false);
            DeactivateGroupText();
        }

        /// <summary>
        /// OnCard drag end 
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardGameObject"></param>
        internal void OnCardDragEnd(Card _card, GameObject _cardGameObject)
        {
            //if(isPlayerDeclare(_cardGameObject,ref declarePanel))
            //{
            //    UiManager.GetInstance.ConfirmationPoup("Alert", "Are you sure you want to Declare", Declare, null);
            //}
            //else 
            if (IsAboveNewGroupPanel(_card, _cardGameObject))
            {
                CreateNewGroup(_card, _cardGameObject);
            }
            else
            {
                var _oldParentObject = childParentObject[_cardGameObject];
                var _distanceMidVectot = _oldParentObject.transform.localPosition;
                _distanceMidVectot.x += (_oldParentObject.GetComponent<RectTransform>().rect.width / 2);
                var tempValue = Vector2.Distance(_distanceMidVectot, _card.transform.localPosition);
                float distance = 0;
                AddCardToNearestGroup(_card, _cardGameObject, ref _oldParentObject, ref tempValue, ref distance);
            }
            createGroupPanel.SetActive(false);
            RemoveAllSelectedCard();
            CheckForInActivaGroup();
            setGroupText();
        }

        private void setGroupText()
        {
            return;
            List<Card> groupCards = new List<Card>();
            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                if (cardGroupGameobject[i].activeSelf)
                {
                    foreach (var item in childParentObject)
                    {
                        if (item.Value == cardGroupGameobject[i])
                        {
                            var card = item.Key.GetComponent<Card>();
                            groupCards.Add(card);
                        }
                    }
                    SetGroupInfo(groupCards, cardGroupGameobject[i], groupSetText[i]);
                }
            }
        }

        private Vector3 tempVector;
        /// <summary>
        /// Find distance between card and the group and add it to the nearest group
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardGameObject"></param>
        /// <param name="_parentObject"></param>
        /// <param name="_tempValue"></param>
        /// <param name="_distance"></param>
        private void AddCardToNearestGroup(Card _card, GameObject _cardGameObject, ref GameObject _parentObject, ref float _tempValue, ref float _distance)
        {
            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                if (cardGroupGameobject[i].activeSelf)
                {
                    tempVector = cardGroupGameobject[i].transform.localPosition;
                    tempVector.x += (parentReactTans[i].rect.width / 2);
                    _distance = Vector2.Distance(tempVector, _card.transform.localPosition);
                    if (_distance < _tempValue)
                    {
                        _tempValue = _distance;
                        _parentObject = cardGroupGameobject[i];
                    }
                }
            }
            childParentObject[_cardGameObject] = _parentObject;
            _cardGameObject.transform.SetParent(childParentObject[_cardGameObject].transform, false);
        }

        /// <summary>
        /// Above create panel create new group
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardGameObject"></param>
        private void CreateNewGroup(Card _card, GameObject _cardGameObject)
        {
            selectedObject.Clear();
            selectedObject.Add(_card, _cardGameObject);
            CheckCanCreateGroup();
        }

        /// <summary>
        /// check is it above nwe group panel create new group if it is
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardObject"></param>
        /// <returns></returns>
        private bool IsAboveNewGroupPanel(Card _card, GameObject _cardObject)
        {
            if (!createGroupPanel.activeSelf)
                return false;
            if (Vector3.Distance(_cardObject.transform.localPosition, createGroupPanel.transform.localPosition) <= 200.0f)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// on each card selected add it to the new List to keep track of all selected item
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cardObject"></param>
        /// <param name="isselected"></param>
        public void OnCardSelect(Card card, GameObject cardObject, bool isselected)
        {
            if (isselected)
            {
                if (!selectedObject.ContainsKey(card))
                {
                    selectedObject.Add(card, cardObject);
                }
            }
            else
            {
                if (selectedObject.ContainsKey(card))
                {
                    selectedObject.Remove(card);
                }
            }

            discardBtn.gameObject.SetActive(selectedObject.Count == 1 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && gameplayManager.isCardDrawn);
            if (CanCreateGroup())
            {
                createGroupBtn.gameObject.SetActive(selectedObject.Count > 1);
            }
            else
            {
                createGroupBtn.gameObject.SetActive(CheckForMergeGroup());
            }
        }

        /// <summary>
        /// Remove all selected form the List
        /// </summary>
        public void RemoveAllSelectedCard()
        {
            foreach (var item in selectedObject)
            {
                item.Key.DeselectCard();
            }
            selectedObject.Clear();
            createGroupBtn.gameObject.SetActive(selectedObject.Count > 0);
        }

        private void CheckCanCreateGroup()
        {
            if (CanCreateGroup())
            {
                var _gObject = CreateGroup();
                foreach (var item in selectedObject)
                {
                    item.Value.transform.SetParent(_gObject.transform, false);
                    childParentObject[item.Value] = _gObject;
                }
            }
            else
            {
                CreateMergeGroup();
            }
            CheckForInActivaGroup();
            RemoveAllSelectedCard();
            setGroupText();
        }

        private bool CanCreateGroup()
        {
            if (inActiveGroups.Count > 0)
            {
                return true;
            }
            return false;
        }

        private GameObject CreateGroup()
        {
            GameObject _gObject = inActiveGroups[0];
            inActiveGroups.RemoveAt(0);
            _gObject.SetActive(true);
            return _gObject;
        }

        /// <summary>
        /// check if all the elements contans any child Object
        /// </summary>
        private void CheckForInActivaGroup()
        {
            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                if (cardGroupGameobject[i].activeSelf)
                {
                    if (cardGroupGameobject[i].transform.childCount <= 0)
                    {
                        ReleaseGroup(cardGroupGameobject[i]);
                    }
                }
            }
            if (inActiveGroups.Count >= 5)
            {
                sortCardBtn.gameObject.SetActive(true);
            }
            else
            {
                sortCardBtn.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Release the group if there is no element in the group and add it to the Last element
        /// </summary>
        /// <param name="_gObject"></param>
        private void ReleaseGroup(GameObject _gObject)
        {
            inActiveGroups.Add(_gObject);
            _gObject.transform.SetSiblingIndex(cardGroupGameobject.Count - 1);
            _gObject.SetActive(false);
        }

        /// <summary>
        /// Release all created group
        /// </summary>
        private void ReleaseAllGroup()
        {
            inActiveGroups.Clear();
            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                ReleaseGroup(cardGroupGameobject[i]);
            }
        }

        /// <summary>
        /// This is used when you have six group Active and you tried to merge 2 groups
        /// </summary>
        /// <returns></returns>
        private bool CheckForMergeGroup()
        {
            if (selectedObject.Count <= 1)
            {
                return false;
            }
            Dictionary<GameObject, int> _parentObjects = new Dictionary<GameObject, int>();
            ListAllParentsAndChild(_parentObjects);
            foreach (var item in _parentObjects)
            {
                if (item.Key.transform.childCount <= item.Value)
                {
                    return true;
                }
            }
            return false;
        }

        private void ListAllParentsAndChild(Dictionary<GameObject, int> _parentObjects)
        {
            foreach (var item in selectedObject)
            {
                if (!_parentObjects.ContainsKey(childParentObject[selectedObject[item.Key]]))
                {
                    _parentObjects.Add(childParentObject[selectedObject[item.Key]], 1);
                }
                else
                {
                    var _tempValue = _parentObjects[childParentObject[selectedObject[item.Key]]];
                    _parentObjects[childParentObject[selectedObject[item.Key]]] = _tempValue + 1;
                }
            }
        }

        private void CreateMergeGroup()
        {
            GameObject _parentObject = null;
            Dictionary<GameObject, int> _parentObjects = new Dictionary<GameObject, int>();
            ListAllParentsAndChild(_parentObjects);
            foreach (var item in _parentObjects)
            {
                if (item.Key.transform.childCount <= item.Value)
                {
                    _parentObject = item.Key;
                }
            }
            if (_parentObject != null)
            {
                foreach (var item in selectedObject)
                {
                    item.Value.transform.SetParent(_parentObject.transform, false);
                    childParentObject[item.Value] = _parentObject;
                }
            }
        }

        private void SortCards()
        {
            var _spadeDeck = new List<GameObject>();
            var _heartDeck = new List<GameObject>();
            var _clubDeck = new List<GameObject>();
            var _diamondDeck = new List<GameObject>();
            ReleaseAllGroup();
            Debug.Log("COunt"+cards.Count);
            for (int i = 0; i < cards.Count; i++)
            {
                Debug.Log(cards[i]);
                SortDeck(_spadeDeck, _heartDeck, _clubDeck, _diamondDeck, i);
            }
            CreateSortedDeck(_spadeDeck);
            CreateSortedDeck(_heartDeck);
            CreateSortedDeck(_clubDeck);
            CreateSortedDeck(_diamondDeck);

            sortCardBtn.gameObject.SetActive(false);
            setGroupText();
        }

        /// <summary>
        /// Create group for each deck
        /// </summary>
        /// <param name="deck"></param>
        private void CreateSortedDeck(List<GameObject> deck)
        {
            if (deck.Count > 0)
            {
                var _gObject = CreateGroup();
                foreach (var item in deck)
                {
                    item.transform.SetParent(_gObject.transform, false);
                    childParentObject[item] = _gObject;
                }
            }
        }

        /// <summary>
        /// Based on deck suit Type sort 
        /// </summary>
        /// <param name="_spadeDeck"></param>
        /// <param name="_heartDeck"></param>
        /// <param name="_clubDeck"></param>
        /// <param name="_diamondDeck"></param>
        /// <param name="i"></param>
        private void SortDeck(List<GameObject> _spadeDeck, List<GameObject> _heartDeck, List<GameObject> _clubDeck, List<GameObject> _diamondDeck, int i)
        {
            if (cards[i].suitType == GameVariables.SuitType.Spades)
            {
                _spadeDeck.Add(cardGameobject[i]);
            }
            if (cards[i].suitType == GameVariables.SuitType.Hearts)
            {
                _heartDeck.Add(cardGameobject[i]);
            }
            if (cards[i].suitType == GameVariables.SuitType.Clubs)
            {
                _clubDeck.Add(cardGameobject[i]);
            }
            if (cards[i].suitType == GameVariables.SuitType.Diamonds || cards[i].suitType == GameVariables.SuitType.Joker)
            {
                _diamondDeck.Add(cardGameobject[i]);
            }
        }

        private void SetGroupInfo(List<Card> cards, GameObject parentObject, TMP_Text text)
        {
            float _position = parentObject.transform.localPosition.x + 10;
            //_position += (parentObject.GetComponent<RectTransform>().rect.width);
            text.transform.localPosition = new Vector3(_position, text.transform.localPosition.y, text.transform.localPosition.z);
            text.text = CheckForValidCardGroup(cards).ToString();
            text.gameObject.SetActive(true);
        }

        private GameVariables.SetType CheckForValidCardGroup(List<Card> cards)
        {
            if (cards.Count < 3)
            {
                return GameVariables.SetType.Invalid;
            }
            if (CheckSet(cards))
            {
                return GameVariables.SetType.Set;
            }
            if (CheckPureSequence(cards))
            {
                return GameVariables.SetType.pureSequence;
            }
            if (CheckForStraightImpureSequence(cards))
            {
                return GameVariables.SetType.ImpureSequence;
            }
            return GameVariables.SetType.Invalid;
        }

        /// <summary>
        /// checks for the pure sequence in the group
        /// </summary>
        /// <returns></returns>
        private bool CheckPureSequence(List<Card> cards)
        {
            if (!IsSameSuitType(cards))
            {
                return false;
            }
            var orderCards = cards.OrderBy(x => (int)x.cardValue).ToList();
            if (orderCards[0].cardValue == GameVariables.CardType.Ace)
            {
                if (CheckForStraightSequence(orderCards))
                {
                    return true;
                }
                else
                {
                    return CheckForInbetweenSequence(orderCards);
                }
            }
            else
            {
                return CheckForStraightSequence(orderCards);
            }
        }

        private bool CheckForInbetweenSequence(List<Card> orderCards)
        {
            int _indexCount = 0;
            for (int i = 0; i < orderCards.Count; i++)
            {
                if (((int)orderCards[i].cardValue + 1) != (int)(orderCards[i + 1].cardValue))
                {
                    _indexCount++;
                    if (_indexCount >= 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// this check for 
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private bool IsSameSuitType(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].suitType != cards[i].suitType)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// this return false if it not sequence
        /// </summary>
        /// <param name="orderCards"></param>
        /// <returns></returns>
        private bool CheckForStraightSequence(List<Card> orderCards)
        {
            for (int i = 0; i < orderCards.Count - 1; i++)
            {
                if (((int)orderCards[i].cardValue + 1) != (int)(orderCards[i + 1].cardValue))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks for the Impure sequence in the current group
        /// </summary>
        private bool CheckImpureSequence(List<Card> cards)
        {
            //chec for direct sequence
            var orderCards = cards.OrderBy(x => (int)x.cardValue).ToList();

            return true;
        }

        private bool CheckForStraightImpureSequence(List<Card> orderCards)
        {
            int _impureCount = 0;
            for (int i = 0; i < orderCards.Count - 1; i++)
            {
                if (orderCards[i].cardValue == GameVariables.CardType.Joker)
                {
                    _impureCount++;
                    if (_impureCount > 2)
                        return false;
                    continue;
                }
                else if (orderCards[i + 1].cardValue == GameVariables.CardType.Joker)
                {
                    _impureCount++;
                    if (_impureCount > 2)
                        return false;
                    continue;
                }
                if (((int)orderCards[i].cardValue + 1) != (int)(orderCards[i + 1].cardValue))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checkd for Set in the list of groups
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private bool CheckSet(List<Card> cards)
        {
            Debug.Log("cards" + cards.Count + ":" + cards[0].cardValue);
            var initialCardValue = cards[0].cardValue;
            foreach (var item in cards)
            {
                if (item.cardValue != initialCardValue)
                {
                    return false;
                }
            }
            return true;
        }

        internal void DeactivateGroupText()
        {
            for (int i = 0; i < groupSetText.Length; i++)
            {
                groupSetText[i].gameObject.SetActive(false);
            }
        }

        private bool isPlayerDeclare(GameObject cardObject, ref GameObject dropPanel)
        {

            var _distance = Vector3.Distance(cardObject.transform.localPosition, dropPanel.transform.localPosition);
            if (_distance < 10.0f)
            {
                return true;
            }
            return false;
        }

        private void CheckForDropPanel(GameObject cardObject, ref GameObject dropPanel)
        {
            var _distance = Vector3.Distance(cardObject.transform.localPosition, dropPanel.transform.localPosition);
            if (_distance > 10.0f)
            {
            }

        }
        private void Declare()
        {

        }

        public void OnCloseDeckClick()
        {
            if (gameplayManager.playerTurn == int.Parse(GameVariables.userId))
            {
                if (gameplayManager.isCardDrawn)
                    return;
                movingCardController.gameObject.transform.localPosition = closedDeckTransform.localPosition;
                movingCardController.Init(gameplayManager.closedCard.cardValue, gameplayManager.closedCard.suitValue);
                movingCardController.Activate();
                movingCardController.Move(playerCardSelectTransform.transform.position, OnCloseddeckMoveComplete);
                gameplayManager.DrawCard(false);
            }
        }
        void OnCloseddeckMoveComplete()
        {
            movingCardController.Deactivate();
            PlayerCard _playerCard = new PlayerCard
            {
                suitValue = gameplayManager.closedCard.suitValue,
                cardValue = gameplayManager.closedCard.cardValue
            };
            onCardSelect?.Invoke(_playerCard);
        }

        public void OnOpenDeckClick()
        {
            if (gameplayManager.playerTurn == int.Parse(GameVariables.userId))
            {
                if (gameplayManager.isCardDrawn)
                    return;
                movingCardController.gameObject.transform.localPosition = openDeckTransform.localPosition;
                movingCardController.Init(gameplayManager.discardedCard.cardValue, gameplayManager.discardedCard.suitValue);
                movingCardController.Move(playerCardSelectTransform.transform.localPosition, OnOpendeckMoveComplete);
                gameplayManager.DrawCard(true);
            }
        }

        void OnOpendeckMoveComplete()
        {
            movingCardController.Deactivate();
            PlayerCard _playerCard = new PlayerCard
            {
                suitValue = gameplayManager.discardedCard.suitValue,
                cardValue = gameplayManager.discardedCard.cardValue
            };
            onCardSelect?.Invoke(_playerCard);
        }

        public void MoveDrawCard()
        {
            backCardController.gameObject.SetActive(true);
            backCardController.gameObject.transform.localPosition = closedDeckTransform.localPosition;
            backCardController.Move(prifileDestination.transform.position, OnDrawMoveComplete, 0.70f);
        }

        public void MoveDiscardedCard(PlayerCard playerCard)
        {
            discardeCard = playerCard;
            movingCardController.gameObject.transform.localPosition = prifileDestination.transform.localPosition;
            movingCardController.Init(playerCard.cardValue, playerCard.suitValue);
            movingCardController.Activate();
            movingCardController.Move(openDeckTransform.transform.position, OnDicardMoveComplete, 0.70f);
        }

        void OnDrawMoveComplete()
        {
            backCardController.gameObject.SetActive(false);
        }

        private void OnCardDiscard()
        {
            PlayerCard _playerCard = new PlayerCard
            {
                cardValue = selectedObject.Keys.First().cardValue,
                suitValue = selectedObject.Keys.First().suitType,
            };
            onCardDiscard?.Invoke(selectedObject.Keys.First());
            MoveCardToOpenPile(_playerCard);
            RemoveAllSelectedCard();
            discardBtn.gameObject.SetActive(false);
        }

        PlayerCard discardeCard;
        private void MoveCardToOpenPile(PlayerCard player)
        {
            movingCardController.gameObject.transform.localPosition = playerCardSelectTransform.transform.localPosition;
            movingCardController.Init(player.cardValue, player.suitValue);
            discardeCard = player;
            movingCardController.Activate();
            movingCardController.Move(openDeckTransform.transform.position, OnDicardMoveComplete);
            Network.Card card = new Network.Card
            {
                suitValue = player.suitValue,
                cardValue = player.cardValue,
            };
            gameplayManager.DiscardCard(card);
        }
        private void OnDicardMoveComplete()
        {
            movingCardController.Deactivate();
            OpenTileCard.Init(discardeCard.cardValue, discardeCard.suitValue);
            OpenTileCard.gameObject.SetActive(true);
        }
    }
}