using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private Button okayBtn, cancelBtn;
    [SerializeField] private TMP_Text messageText, headingText;
    private Action successAction;
    private Action failureAction;
    private void OnEnable()
    {
        okayBtn.onClick.AddListener(OnOkayButtonClick);
        cancelBtn.onClick.AddListener(OnCancelBtnClick);
    }
    private void OnDisable()
    {
        okayBtn.onClick.RemoveListener(OnOkayButtonClick);
        cancelBtn.onClick.RemoveListener(OnCancelBtnClick);
    }
    private void Activate()
    {
        gameObject.SetActive(true);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Popup Show
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messagePopupHeading"></param>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    internal void ShowPopup(string message, string messagePopupHeading, Action success = null, Action failure = null)
    {
        successAction = success;
        failureAction = failure;
        messageText.text = message;
        headingText.text = messagePopupHeading;
        Activate();
    }
    private void OnOkayButtonClick()
    {
        Deactivate();
        successAction?.Invoke();
    }

    private void OnCancelBtnClick()
    {
        Deactivate();
        failureAction?.Invoke();
    }
}
