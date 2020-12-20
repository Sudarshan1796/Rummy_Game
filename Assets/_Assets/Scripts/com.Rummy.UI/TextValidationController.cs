using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.Ui
{
    public class TextValidationController : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] validationType;
        internal void SetDetails(string groupValidationType)
        {
            text.text = groupValidationType;
            if (groupValidationType == "Invalid")
            {
                image.sprite = validationType[0];
            }
            else
            {
                image.sprite = validationType[1];
            }
        }
    }
}