using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CommonPopUpUiController : MonoBehaviour
{
    [SerializeField] private GameObject uiHolder;
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
        onClickOkayAction?.Invoke();
        onClickOkayAction = null;
        ClocePopUp();
    }

    private void OnClickCancelButton()
    {
        onClickCancelAction?.Invoke();
        onClickCancelAction = null;
        ClocePopUp();
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
        _ = LeanTween.move(uiHolder, toObject.transform.position, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }

    internal void ClocePopUp()
    {
        _ = LeanTween.move(uiHolder, fromObject.transform.position, 0.25f).setEase(LeanTweenType.linear).setOnComplete(()=> { transparentBg.gameObject.SetActive(false); });
    }
}
