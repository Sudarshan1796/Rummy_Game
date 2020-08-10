using com.Rummy.GameVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.Rummy.Gameplay
{
    public class Card : MonoBehaviour, ICardManager, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image cardImage;

        private bool isSelected;
        private CardGroupController cardGroupController;

        internal GameVariables.SuitType suitType;
        internal GameVariables.CardType cardValue;
        internal int cardIndexInParent;

        private readonly Vector3 activePosition     = new Vector3(0, 25, 0);
        private readonly Vector3 inactivePosition  = new Vector3(0, -20, 0);
        private LTDescr leanTweenObject;
        private float speed = 0.50f;
        private void Start()
        {
            cardGroupController = CardGroupController.GetInstance;
        }

        public void Draw()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Vector3 _destinationPoint)
        {
            leanTweenObject = LeanTween.move(gameObject, _destinationPoint, speed * 0.5f).setEase(LeanTweenType.linear);
        }
        

        /// <summary>
        /// This method will be called on the start of the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            DeselectCard();
            cardGroupController.OnDragBegin(this, this.gameObject);
        }

        /// <summary>
        /// This method will be called during the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        /// <summary>
        /// This method will be called at the end of mouse drag
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            cardGroupController.OnCardDragEnd(this, gameObject);
        }

        internal void Init(int index, GameVariables.CardType cardValue, GameVariables.SuitType suitType)
        {
            this.cardValue = cardValue;
            this.suitType = suitType;
            cardIndexInParent = index;
            cardImage.sprite = CardController.GetInstance.GetSprite(cardValue, suitType);
        }
        /// <summary>
        /// OnClick Event
        /// </summary>
        public void OnCardClicked()
        {
            if (isSelected)
            {
                DeselectCard();
            }
            else
            {
                SelectCard();
            }
            cardGroupController.OnCardSelect(this, gameObject, isSelected);
        }

        /// <summary>
        /// Select Item
        /// </summary>
        private void SelectCard()
        {
            isSelected = true;
            cardImage.color = Color.grey;
            cardImage.gameObject.transform.localPosition = activePosition;
        }

        /// <summary>
        /// Deselect item
        /// </summary>
        public void DeselectCard()
        {
            isSelected = false;
            cardImage.color = Color.white;
            cardImage.gameObject.transform.localPosition = inactivePosition;
        }
    }
}
