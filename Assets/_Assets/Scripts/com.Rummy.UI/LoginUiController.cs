using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace com.Rummy.Ui
{
    public class LoginUiController : MonoBehaviour
    {
        [Header("Mobile Number Input Panel")]
        [SerializeField] private GameObject mobNumberInputPanel;
        [SerializeField] private TMP_InputField mobNumberText;
        [SerializeField] private TextMeshProUGUI numberInputPanelErrorText;
        [SerializeField] private Button mobNumberSubmitButton;
        [Header("OTP Input Panel")]
        [SerializeField] private GameObject otpInputPanel;
        [SerializeField] private GameObject otpResendObject;
        [SerializeField] private TextMeshProUGUI otpSentText;
        [SerializeField] private TextMeshProUGUI otpInputPanelTimerText;
        [SerializeField] private TextMeshProUGUI otpInputPanelErrorText;
        [SerializeField] private List<TMP_InputField> otpText;
        [SerializeField] private Button otpResendButton;
        [SerializeField] private Button otpSubmitButton;
        [SerializeField] private Button backButtonButton;

        private Coroutine otpInputPanelTimer;

        private void Start()
        {
            mobNumberSubmitButton.onClick.AddListener(OnClickMobNumberSubmitButton);
            otpSubmitButton.onClick.AddListener(OnClickOtpSubmitButton);
            otpResendButton.onClick.AddListener(OnClickOtpResendButton);
            backButtonButton.onClick.AddListener(OnClickOtpPanelBackButton);
        }

        private void OnClickMobNumberSubmitButton()
        {
            if (mobNumberText.text.Length == 10)
            {
                ShowErrorMessageInNumberInputPanel("");
                UiManager.GetInstance.OnSubmitMobNumber(mobNumberText.text);
            }
            else
            {
                ShowErrorMessageInNumberInputPanel("Please Enter a valid mobile number");
            }
        }

        private void OnClickOtpSubmitButton()
        {
            string otp = null;
            foreach (var text in otpText)
            {
                otp += text.text;
            }
            UiManager.GetInstance.OnSubmitOtpNumber(mobNumberText.text, otp);
        }

        private void OnClickOtpResendButton()
        {
            UiManager.GetInstance.OnSubmitMobNumber(mobNumberText.text);
        }

        private void OnClickOtpPanelBackButton()
        {
            mobNumberInputPanel.SetActive(true);
            StopCoroutine(otpInputPanelTimer);
            otpInputPanel.SetActive(false);
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
            ClearDynamicTexts();
            otpSentText.text = $"OTP sent to {mobNumberText.text}";
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

        private IEnumerator StartOtpResendTimer(int remainingTime)
        {
            otpInputPanelTimerText.text = $"Resend OTP in <color=white>{remainingTime} sec</color>";
            EnableOtpInputPanelTimerText(true);
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                remainingTime--;
                otpInputPanelTimerText.text = $"Resend OTP in <color=white>{remainingTime} sec</color>";
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
            otpResendObject.SetActive(!shoulEnable);
        }

        private void ClearDynamicTexts()
        {
            foreach(var text in otpText)
            {
                text.text = "";
            }
            numberInputPanelErrorText.text = "";
            otpInputPanelErrorText.text = "";
        }
    }
}
