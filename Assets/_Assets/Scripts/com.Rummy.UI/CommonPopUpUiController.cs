using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CommonPopUpUiController : MonoBehaviour
{
    [SerializeField] private GameObject commonPopUpUiHolder;
    [SerializeField] private GameObject internetAlertPopUpUiHolder;
    [SerializeField] private Transform fromObject;
    [SerializeField] private Transform toObject;
    [SerializeField] private TMP_Text popUpTitle;
    [SerializeField] private TMP_Text popUpMessage;
    [SerializeField] private Button okayButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image transparentBg;

    private Action onClickOkayAction;
    private Action onClickCancelAction;

    private void Start()
    {
        okayButton.onClick.AddListener(OnClickOkayButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    private void OnClickOkayButton()
    {
        if (!internetAlertPopUpUiHolder.activeSelf)
        {
            onClickOkayAction?.Invoke();
            onClickOkayAction = null;
            ClosePopUp();
        }
    }

    private void OnClickCancelButton()
    {
        if (!internetAlertPopUpUiHolder.activeSelf)
        {
            onClickCancelAction?.Invoke();
            onClickCancelAction = null;
            ClosePopUp();
        }
    }

    internal void ShowPopUp(string titleText, string messageText, bool enableOkayButton, bool enableCancelButton, Action onClickOkayButton = null, Action onClickCancelButton = null)
    {
        popUpTitle.text = titleText;
        popUpMessage.text = messageText;
        okayButton.transform.parent.gameObject.SetActive(enableOkayButton);
        cancelButton.transform.parent.gameObject.SetActive(enableCancelButton);
        onClickOkayAction = onClickOkayButton;
        onClickCancelAction = onClickCancelButton;
        transparentBg.gameObject.SetActive(true);
        commonPopUpUiHolder.SetActive(true);
        _ = LeanTween.move(commonPopUpUiHolder, toObject.transform.position, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }

    internal void ClosePopUp()
    {
        _ = LeanTween.move(commonPopUpUiHolder, fromObject.transform.position, 0.25f).setEase(LeanTweenType.linear).setOnComplete(()=> { transparentBg.gameObject.SetActive(false); commonPopUpUiHolder.SetActive(false); });
    }

    internal void ShowInternetDisconnectedPopUp()
    {
        transparentBg.gameObject.SetActive(true);
        internetAlertPopUpUiHolder.SetActive(true);
        _ = LeanTween.move(internetAlertPopUpUiHolder, toObject.transform.position, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }

    internal void CloseInternetDisconnectedPopUp()
    {
        _ = LeanTween.move(internetAlertPopUpUiHolder, fromObject.transform.position, 0.25f).setEase(LeanTweenType.linear).setOnComplete(() =>
        { if(!commonPopUpUiHolder.activeSelf) transparentBg.gameObject.SetActive(false); internetAlertPopUpUiHolder.SetActive(false); });
    }
}
