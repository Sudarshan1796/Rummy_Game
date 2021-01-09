using com.Rummy.GameVariable;
using com.Rummy.Network;
using com.Rummy.Ui;
using com.Rummy.UI;
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
        [SerializeField] private Button createGroupBtn, sortCardBtn, discardBtn, dropBtn, DeclareBtn;
        [SerializeField] private GameObject createGroupPanel;
        [SerializeField] private RectTransform[] parentReactTans;
        [SerializeField] private GameObject[] groupSetText;
        [SerializeField] private GameObject declarePanel;
        [SerializeField] private Transform openDeckTransform;
        [SerializeField] private Transform closedDeckTransform;
        [SerializeField] private GameObject playerCardSelectTransform;
        [SerializeField] private GameObject movingCard;
        [SerializeField] private Card movingCardController;
        [SerializeField]private Transform prifileDestination;
        [SerializeField] private Card backCardController;
        [SerializeField] private Card OpenTileCard;
        [SerializeField] private float cardLenght;
        private List<GameObject> cardGameobject; 
        private List<Card> cards;
        private List<GameObject> inActiveGroups;
        private Dictionary<Card, GameObject> selectedObject;
        private Dictionary<GameObject, GameObject> childParentObject;
        private List<TextValidationController> textValidation;

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
            textValidation = new List<TextValidationController>();
            foreach(var text in groupSetText)
            {
                var textVal = text.GetComponent<TextValidationController>();
                textValidation.Add(textVal);
            }
        }

        private void OnEnable()
        {
            createGroupBtn.onClick.AddListener(CheckCanCreateGroup);
            sortCardBtn.onClick.AddListener(SortCards);
            discardBtn.onClick.AddListener(OnCardDiscard);
            dropBtn.onClick.AddListener(OnDropBtnClick);
            DeclareBtn.onClick.AddListener(OnDeclareBtnClick);

        }

        private void OnDisable()
        {
            createGroupBtn.onClick.RemoveListener(CheckCanCreateGroup);
            sortCardBtn.onClick.RemoveListener(SortCards);
            discardBtn.onClick.RemoveListener(OnCardDiscard);
            dropBtn.onClick.RemoveListener(OnDropBtnClick);
            DeclareBtn.onClick.RemoveListener(OnDeclareBtnClick);
        }

        public void InitilizeGroup(List<GameObject> _cardObject, List<Card> _cards)
        {
            //this.cards.Clear();
            //this.cardGameobject.Clear();
            selectedObject.Clear();
            RemoveAllSelectedCard();
            childParentObject.Clear();
            //OpenTileCard.gameObject.SetActive(false);
            inActiveGroups.Clear();
            DeactivateGroupText();
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
            GetGroupValidation();
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
            GetGroupValidation();
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
                   // SetGroupInfo(groupCards, cardGroupGameobject[i], groupSetText[i]);
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
        /// This is when player drops the card to the top of the drop panel
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardObject"></param>
        /// <returns></returns>
        private bool IsAboveDropPanel(Card _card, GameObject _cardObject)
        {
            if (!declarePanel.activeSelf)
                return false;
            if (Vector3.Distance(_cardObject.transform.localPosition, declarePanel.transform.localPosition) <= 200.0f)
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
            if (selectedObject.Count == 0)
            {
                GetGroupValidation();
            }
            EnableCardSelction();
            if (CanCreateGroup())
            {
                createGroupBtn.gameObject.SetActive(selectedObject.Count > 1);
            }
            else
            {
                createGroupBtn.gameObject.SetActive(CheckForMergeGroup());
            }

            DeactivateGroupText();
        }

        private void EnableCardSelction()
        {
            dropBtn.gameObject.SetActive(selectedObject.Count == 0 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && !gameplayManager.isCardDrawn);
            discardBtn.gameObject.SetActive(selectedObject.Count == 1 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && gameplayManager.isCardDrawn && !gameplayManager.CanPlayerMakeCardMovement);
            DeclareBtn.gameObject.SetActive(selectedObject.Count == 1 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && gameplayManager.isCardDrawn);
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
            GetGroupValidation();
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

            for (int i = 0; i < cards.Count; i++)
            {
                Debug.Log(cards[i]);
                SortDeck(_spadeDeck, _heartDeck, _clubDeck, _diamondDeck, i);
            }

            CreateSortedDeck(_heartDeck);
            CreateSortedDeck(_diamondDeck);
            CreateSortedDeck(_clubDeck);
            CreateSortedDeck(_spadeDeck);

            sortCardBtn.gameObject.SetActive(false);
            //setGroupText();
            GetGroupValidation();
        }

        /// <summary>
        /// Create group for each deck
        /// </summary>
        /// <param name="deck"></param>
        private void CreateSortedDeck(List<GameObject> deck)
        {
            deck = deck.OrderBy(o => o.GetComponent<Card>().cardValueIndex).ToList();
            if (deck.Count > 0)
            {
                var _gObject = CreateGroup();
                foreach (var item in deck)
                {
                    item.transform.SetParent(_gObject.transform);
                    item.transform.SetSiblingIndex(_gObject.transform.childCount - 1);
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
            float _position = parentObject.transform.localPosition.x + parentObject.GetComponent<RectTransform>().rect.width / 2;
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

        public void OnCloseDeckClick()
        {
            if (gameplayManager.playerTurn == int.Parse(GameVariables.userId))
            {
                if (gameplayManager.isCardDrawn)
                    return;
                gameplayManager.DrawCard(false);
            }
        }



        void OnCloseddeckMoveComplete()
        {
            movingCardController.Deactivate();
            PlayerCard _playerCard = new PlayerCard
            {
                suitValue = gameplayManager.selectedCard.suitValue,
                cardValue = gameplayManager.selectedCard.cardValue
            };
            onCardSelect?.Invoke(_playerCard);
        }

        public void OnOpenDeckClick()
        {
            if (gameplayManager.playerTurn == int.Parse(GameVariables.userId))
            {
                if (gameplayManager.isCardDrawn)
                    return;
                //MoveOpenCard();
                gameplayManager.DrawCard(true);
            }
        }

        private void MoveOpenCard()
        {

        }

        void OnOpendeckMoveComplete()
        {
            movingCardController.Deactivate();
            PlayerCard _playerCard = new PlayerCard
            {
                suitValue = gameplayManager.selectedCard.suitValue,
                cardValue = gameplayManager.selectedCard.cardValue
            };
            onCardSelect?.Invoke(_playerCard);
        }

        /// <summary>
        /// On card draw
        /// </summary>
        /// <param name="response"></param>
        public void MoveCardDraw(CardDrawRes response,Transform destination)
        {
            if (response.userId != int.Parse(GameVariables.userId))
            {
                MoveOtherPlayerSelectedCard(response, destination);
            }
            else
            {
                MoveSelectedCard(response);
            }
            EnableCardSelction();
            GetGroupValidation();
        }

        /// <summary>
        /// For other player selected card
        /// </summary>
        /// <param name="response"></param>
        public void MoveOtherPlayerSelectedCard(CardDrawRes response,Transform destinationTransform)
        {
            if (response.isFromDiscardPile)
            {
                movingCardController.gameObject.transform.localPosition = openDeckTransform.localPosition;
                movingCardController.Init(response.card.cardValue, response.card.suitValue);
                movingCardController.Activate();
                movingCardController.Move(destinationTransform.transform.position, OnOpenDeckMoveComplete, 0.70f);
                if (gameplayManager.discardedCard==null)
                {
                    OpenTileCard.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log(response.discardPile.cardValue + ":" + response.discardPile.suitValue);
                    OpenTileCard.gameObject.SetActive(true);
                    OpenTileCard.Init(response.discardPile.cardValue, response.discardPile.suitValue);
                }
            }
            else
            {

                backCardController.gameObject.SetActive(true);
                backCardController.gameObject.transform.localPosition = closedDeckTransform.localPosition;
                backCardController.Move(destinationTransform.transform.position, OnDrawMoveComplete, 0.70f);
            }
        }
        /// <summary>
        /// For Our own selected Card
        /// </summary>
        /// <param name="response"></param>
        public void MoveSelectedCard(CardDrawRes response)
        {
            if (!response.isFromDiscardPile)
            {
                movingCardController.gameObject.transform.localPosition = closedDeckTransform.localPosition;
                movingCardController.Init(response.card.cardValue, response.card.suitValue);
                movingCardController.Activate();
                movingCardController.Move(playerCardSelectTransform.transform.position, OnCloseddeckMoveComplete,
                    0.80f);
            }
            else
            {
                movingCardController.gameObject.transform.localPosition = openDeckTransform.localPosition;
                movingCardController.Init(response.card.cardValue, response.card.suitValue);
                movingCardController.Activate();
                movingCardController.Move(playerCardSelectTransform.transform.position, OnOpendeckMoveComplete, 0.75f);
                if (gameplayManager.discardedCard == null)
                {
                    OpenTileCard.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log(response.discardPile.cardValue + ":" + response.discardPile.suitValue);
                    OpenTileCard.gameObject.SetActive(true);
                    OpenTileCard.Init(response.discardPile.cardValue, response.discardPile.suitValue);
                }
            }
        }

        void OnOpenDeckMoveComplete()
        {
            movingCardController.Deactivate();
        }

        public void MoveDiscardedCard(PlayerCard playerCard,Transform initalposition=null)
        {
            discardeCard = playerCard;
            movingCardController.gameObject.transform.localPosition = initalposition.localPosition;
            movingCardController.Init(playerCard.cardValue, playerCard.suitValue);
            movingCardController.Activate();
            movingCardController.Move(openDeckTransform.transform.position, OnDicardMoveComplete, 0.70f);
        }

        void OnDrawMoveComplete()
        {
            backCardController.gameObject.SetActive(false);
            GetGroupValidation();
        }

        private void OnCardDiscard()
        {
            Network.Card card = new Network.Card
            {
                suitValue = selectedObject.Keys.First().suitType,
                cardValue = selectedObject.Keys.First().cardValue,
            };
            gameplayManager.DiscardCard(card);
            discardBtn.gameObject.SetActive(false);
            DeclareBtn.gameObject.SetActive(false);
            GetGroupValidation();
        }

        public void MoveUserCard(PlayerCard playerCard)
        {
            var positionCard = childParentObject[selectedObject.Keys.First().gameObject];
            int _index = selectedObject.Keys.First().gameObject.transform.GetSiblingIndex();
            var _cardPosition = positionCard.transform.localPosition;
            _cardPosition.x += _index * cardLenght + cardLenght;
            _cardPosition.y = _cardPosition.y - (2 * cardLenght);
            onCardDiscard?.Invoke(selectedObject.Keys.First());
            MoveCardToOpenPile(playerCard, _cardPosition);
            RemoveAllSelectedCard();
        }

        PlayerCard discardeCard;
        private void MoveCardToOpenPile(PlayerCard player,Vector3 positionCard)
        {
            movingCardController.gameObject.transform.localPosition = positionCard;
            movingCardController.Init(player.cardValue, player.suitValue);
            discardeCard = player;
            movingCardController.Activate();
            movingCardController.Move(openDeckTransform.transform.position, OnDicardMoveComplete, 0.80f);

        }
        private void OnDicardMoveComplete()
        {
            movingCardController.Deactivate();
            OpenTileCard.Init(discardeCard.cardValue, discardeCard.suitValue);
            OpenTileCard.gameObject.SetActive(true);
        }

        #region Drop
        private void OnDropBtnClick()
        {
            //Show Confirmation Popup
            //if yes then Drop
            UiManager.GetInstance.ConfirmationPoup("Are you sure you want to Drop?", "Drop", DropCard);
        }

        private void DropCard()
        {
            gameplayManager.PlayerDrop();
            dropBtn.gameObject.SetActive(false);
            GameplayController.GetInstance.UpdateDropPanel(true);
            DeactivateGroupText();
        }

        internal void EnableDropButton(bool isGamestart = false)
        {
            dropBtn.gameObject.SetActive(selectedObject.Count == 0 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && !gameplayManager.isCardDrawn);
            //if(isGamestart)
            //{
            //    dropBtn.gameObject.SetActive(selectedObject.Count == 0 && gameplayManager.playerTurn == int.Parse(GameVariables.userId) && !gameplayManager.isCardDrawn);
            //}
        }

        #endregion

        #region Declare
        private void OnDeclareBtnClick()
        {
            UiManager.GetInstance.ConfirmationPoup("Are you sure you want to Declare?", "Declare", Declare);
        }

        internal void Declare()
        {
            discardBtn.gameObject.SetActive(false);
            DeclareBtn.gameObject.SetActive(false);
            List<Network.CardGroup> groupList = new List<Network.CardGroup>();
            GetGroupData(groupList);

            var showCard = GetShowCard();

            gameplayManager.PlayerDeclare(groupList, showCard);
            gameplayManager.IsPlayerDeclare = true;
        }

        private Network.Card GetShowCard()
        {
            Network.Card showCard = new Network.Card();
            if (selectedObject.Count > 0)
            {
                showCard.suitValue = selectedObject.ElementAt(0).Key.suitType;
                showCard.cardValue = selectedObject.ElementAt(0).Key.cardValue;
            }
            else
            {
                showCard = null;
            }

            return showCard;
        }

        private void GetGroupData(List<Network.CardGroup> groupList)
        {
            foreach (var group in cardGroupGameobject)
            {
                if (!@group.activeSelf)
                {
                    continue;
                }

                Network.CardGroup cardGroup = new Network.CardGroup()
                {
                    group_id = @group.transform.GetSiblingIndex() + 1,
                    card_set = new List<Network.Card>(),
                };
                for (int i = 0; i < @group.transform.childCount; i++)
                {
                    var card = @group.transform.GetChild(i).GetComponent<Card>().GetCard;
                    if (selectedObject.Count > 0 &&
                        (selectedObject.ElementAt(0).Value == @group.transform.GetChild(i).gameObject))
                    {
                        continue;
                    }

                    cardGroup.card_set.Add(card);
                }

                groupList.Add(cardGroup);
            }
        }

        internal void UpdateDeclareButtonState(bool state)
        {
            DeclareBtn.gameObject.SetActive(state);
        }

        #endregion

        internal void EnableOpenPile()
        {
            if (gameplayManager.discardedCard == null)
            {
                OpenTileCard.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(gameplayManager.discardedCard.cardValue + ":" + gameplayManager.discardedCard.suitValue);
                OpenTileCard.gameObject.SetActive(true);
                OpenTileCard.Init(gameplayManager.discardedCard.cardValue, gameplayManager.discardedCard.suitValue);
            }
        }

        private void GetGroupValidation()
        {
            List<Network.CardGroup> groupList = new List<Network.CardGroup>();
            GetGroupData(groupList);
            gameplayManager.CardGroupValidation(groupList);
        }

        internal void ValidateGroupSequense(GroupValidationResponse response)
        {
            int i = 0;
            //if(selectedObject.Count>0)
            //{
            //    return;
            //}
            foreach (var group in cardGroupGameobject)
            {
                if (!group.activeSelf)
                {
                    continue;
                }

                var group_id = group.transform.GetSiblingIndex() + 1;
                var groupValidation = response.cardGroup.Find((GroupValidation x) => x.groupId == group_id);
                SetGroupInfoText(group, groupSetText[i], groupValidation.handType, textValidation[i]);
                i++;
            }
        }

        private void SetGroupInfoText(GameObject parentObject, GameObject text, string message, TextValidationController textVal)
        {
            float _position = parentObject.transform.localPosition.x + 40.0f;// (parentObject.GetComponent<RectTransform>().rect.width / 4);
            //_position += (parentObject.GetComponent<RectTransform>().rect.width);
            text.transform.localPosition = new Vector3(_position, text.transform.localPosition.y, text.transform.localPosition.z);
            //text.text = message;
            textVal.SetDetails(message);
            text.gameObject.SetActive(true);
        }

        internal void DeactivateAllButtons()
        {
            createGroupBtn.gameObject.SetActive(false);
            sortCardBtn.gameObject.SetActive(false);
            discardBtn.gameObject.SetActive(false);
            dropBtn.gameObject.SetActive(false);
            DeclareBtn.gameObject.SetActive(false);
        }
    }
}