using com.Rummy.GameVariable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.Gameplay
{
    public class CardGroupController : MonoBehaviour
    {
        [SerializeField] private GameObject tempParemtObject;
        [SerializeField] private List<GameObject> cardGroupGameobject;
        [SerializeField] private List<CardGroup> cardGroups;
        [SerializeField] private Button createGroupBtn, sortCardBtn;
        [SerializeField] private GameObject createGroupPanel;

        [SerializeField] private GameVariables.CardType[] tempcardValue;
        [SerializeField] private GameVariables.SuitType[] tempSuitTypes;

        private List<GameObject> cardGameobject;
        private List<Card> cards;
        private List<GameObject> inActiveGroups;
        private Dictionary<Card, GameObject> selectedObject;
        private Dictionary<GameObject, GameObject> childParentObject;
        private static CardGroupController instance;

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
            inActiveGroups = new List<GameObject>();
            selectedObject = new Dictionary<Card, GameObject>();
            childParentObject = new Dictionary<GameObject, GameObject>();
            //Below code is simply generate 13 cards
            //Todo add server init here
            TempCode();
        }

        private void OnEnable()
        {
            createGroupBtn.onClick.AddListener(CheckCanCreateGroup);
            sortCardBtn.onClick.AddListener(SortCards);
        }

        private void OnDisable()
        {
            createGroupBtn.onClick.RemoveListener(CheckCanCreateGroup);
            sortCardBtn.onClick.RemoveListener(SortCards);
        }

        private void TempCode()
        {
            cards = new List<Card>();
            cardGameobject = new List<GameObject>();
            for (int i = 0; i < 13; i++)
            {
                var _gameObject = CardController.GetInstance.GetObject(cardGroupGameobject[0].transform);
                var _card = _gameObject.GetComponent<Card>();
                _card.Init(i, tempcardValue[i], tempSuitTypes[i]);
                cards.Add(_card);
                cardGameobject.Add(_gameObject);
                childParentObject.Add(_gameObject, cardGroupGameobject[0]);
            }
            inActiveGroups.Clear();
            for (int i = 1; i < cardGroupGameobject.Count; i++)
            {
                cardGroupGameobject[i].transform.SetSiblingIndex(i);
                inActiveGroups.Add(cardGroupGameobject[i]);
            }
        }

        internal void OnDragBegin(Card card, GameObject cardGameObject)
        {
            createGroupPanel.SetActive(CanCreateGroup());
            cardGameObject.transform.SetParent(tempParemtObject.transform, false);
        }

        /// <summary>
        /// OnCard drag end 
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardGameObject"></param>
        internal void OnCardDragEnd( Card _card, GameObject _cardGameObject)
        {

            if (IsAboveNewGroupPanel(_card, _cardGameObject))
            {
                CreateNewGroup(_card, _cardGameObject);
            }
            else
            {
                var tempObject = childParentObject[_cardGameObject];
                var tempValue = Vector2.Distance(tempObject.transform.localPosition, _card.transform.localPosition);
                float distance = 0;
                AddCardToNearestGroup(_card, _cardGameObject, ref tempObject, ref tempValue, ref distance);
            }
            createGroupPanel.SetActive(false);
            RemoveAllSelectedCard();
            CheckForInActivaGroup();
        }
        /// <summary>
        /// Find distance between card and the group and add it to the nearest group
        /// </summary>
        /// <param name="_card"></param>
        /// <param name="_cardGameObject"></param>
        /// <param name="tempObject"></param>
        /// <param name="_tempValue"></param>
        /// <param name="_distance"></param>
        private void AddCardToNearestGroup(Card _card, GameObject _cardGameObject, ref GameObject tempObject, ref float _tempValue, ref float _distance)
        {
            foreach (var item in cardGroupGameobject)
            {
                if (item.activeSelf)
                {
                    _distance = Vector2.Distance(item.transform.localPosition, _card.transform.localPosition);
                    if (_distance < _tempValue)
                    {
                        _tempValue = _distance;
                        tempObject = item;
                    }
                }
            }
            childParentObject[_cardGameObject] = tempObject;
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
        private bool IsAboveNewGroupPanel(Card _card,GameObject _cardObject)
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
        public void OnCardSeelect(Card card, GameObject cardObject, bool isselected)
        {
            if (isselected)
            {
                selectedObject.Add(card, cardObject);
            }
            else
            {
                if (selectedObject.ContainsKey(card))
                {
                    selectedObject.Remove(card);
                }
            }
            if(CanCreateGroup())
            {
                createGroupBtn.gameObject.SetActive(selectedObject.Count > 0);
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
                item.Key.Deselect();
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
            RemoveAllSelectedCard();
            CheckForInActivaGroup();
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
                if(cardGroupGameobject[i].activeSelf)
                {
                    if (cardGroupGameobject[i].transform.childCount <= 0)
                    {
                        ReleaseGroup(cardGroupGameobject[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Release the group if there is no element in the group and add it to the Lat element
        /// </summary>
        /// <param name="_gObject"></param>
        private void ReleaseGroup(GameObject _gObject)
        {
            inActiveGroups.Add(_gObject);
            _gObject.transform.SetSiblingIndex(cardGroupGameobject.Count - 1);
            _gObject.SetActive(false);
        }

        /// <summary>
        /// This is used when you have six group Active and you tried to merge 2 groups
        /// </summary>
        /// <returns></returns>
        private bool CheckForMergeGroup()
        {
            if (selectedObject.Count <= 0)
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
            if(_parentObject!=null)
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
            for (int i = 0; i < cards.Count; i++)
            {
                SortDeck(_spadeDeck, _heartDeck, _clubDeck, _diamondDeck, i);
            }
            for (int i = 0; i < cardGroupGameobject.Count; i++)
            {
                ReleaseGroup(cardGroupGameobject[i]);
            }
            CreateSortedDeck(_spadeDeck);
            CreateSortedDeck(_heartDeck);
            CreateSortedDeck(_clubDeck);
            CreateSortedDeck(_diamondDeck);
            sortCardBtn.gameObject.SetActive(false);
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
            if (cards[i].suitType == GameVariables.SuitType.Diamonds)
            {
                _diamondDeck.Add(cardGameobject[i]);
            }
        }
    }
}