using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace com.Rummy.Ui
{
    public class LoginUiController : MonoBehaviour
    {
        [SerializeField] private GameObject mobNumberInputPanel;
        [SerializeField] private GameObject otpInputPanel;
        [SerializeField] private TextMeshProUGUI mobNumberText;
        [SerializeField] private TextMeshProUGUI otpText;
        [SerializeField] private TextMeshProUGUI numberInputPanelErrorText;
        [SerializeField] private TextMeshProUGUI otpInputPanelErrorText;
        [SerializeField] private TextMeshProUGUI otpInputPanelTimerText;
        [SerializeField] private Button mobNumberSubmitButton;
        [SerializeField] private Button otpSubmitButton;
        [SerializeField] private Button otpResendButton;

        private Coroutine otpInputPanelTimer;

        private void Start()
        {
            mobNumberSubmitButton.onClick.AddListener(OnClickMobNumberSubmitButton);
            otpSubmitButton.onClick.AddListener(OnClickOtpSubmitButton);
            otpResendButton.onClick.AddListener(OnClickOtpResendButton);
        }

        internal void EnableMobNumberInputPanel()
        {
            mobNumberInputPanel.SetActive(true);
            otpInputPanel.SetActive(false);
        }

        internal void EnableOtpInputPanel(int timeToResendOtp)
        {
            otpInputPanel.SetActive(true);
            mobNumberInputPanel.SetActive(false);
            otpInputPanelTimer = StartCoroutine(StartOtpResendTimer(timeToResendOtp));
        }

        internal void DisableOtpInputPanel()
        {
            otpInputPanel.SetActive(false);
            StopCoroutine(otpInputPanelTimer);
        }

        internal void ShowErrorMessageInNumberInputPanel(string errorMessage)
        {
            numberInputPanelErrorText.text = errorMessage;
        }

        internal void ShowErrorMessageInOtpInputPanel(string errorMessage)
        {
            otpInputPanelErrorText.text = errorMessage;
        }

        private void OnClickMobNumberSubmitButton()
        {
            UiManager.GetInstance.OnSubmitMobNumber(mobNumberText.text);
        }

        private void OnClickOtpSubmitButton()
        {
            UiManager.GetInstance.OnSubmitOtpNumber(mobNumberText.text, otpText.text);
        }

        private void OnClickOtpResendButton()
        {
            EnableOtpInputPanelTimerText(true);
            UiManager.GetInstance.OnSubmitMobNumber(mobNumberText.text);
        }

        private IEnumerator StartOtpResendTimer(int remainingTime)
        {
            EnableOtpInputPanelTimerText(true);
            otpInputPanelTimerText.text = $"Resend OTP in {remainingTime} sec";
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                remainingTime--;
                otpInputPanelTimerText.text = $"Resend OTP in {remainingTime} sec";
                if(remainingTime == 0)
                {
                    EnableOtpInputPanelTimerText(false);
                    break;
                }
            }
        }

        private void EnableOtpInputPanelTimerText(bool shoulEnable)
        {
            otpInputPanelTimerText.gameObject.SetActive(shoulEnable);
            otpResendButton.gameObject.SetActive(!shoulEnable);
        }
    }
}
