using UnityEngine;
using TMPro;
using UnityEngine.UI;
using com.Rummy.GameVariable;

namespace com.Rummy.Ui
{
    public class ProfileScreenController : MonoBehaviour
    {
        [Header("User Profile Overview panel")]
        [SerializeField] private GameObject userProfileOverviewPanel;
        [SerializeField] private TextMeshProUGUI mobileNumberText;
        [SerializeField] private TextMeshProUGUI mobileNumberVerificationStatusText;
        [SerializeField] private TextMeshProUGUI emailText;
        [SerializeField] private TextMeshProUGUI emailVerificationStatusText;
        [SerializeField] private TextMeshProUGUI displayNameText;
        [SerializeField] private TextMeshProUGUI fullNameText;
        [Header("Email Update Panel")]
        [SerializeField] private GameObject emailUpdatePanel;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TextMeshProUGUI emailUpdateErrorText;
        [SerializeField] private Button emailSaveButton;
        [Header("Display Name Update Panel")]
        [SerializeField] private GameObject displayNameUpdatePanel;
        [SerializeField] private TMP_InputField displayNameInputField;
        [SerializeField] private TextMeshProUGUI displayNameUpdateErrorText;
        [SerializeField] private Button displayNameSaveButton;
        [Header("Full Name Update Panel")]
        [SerializeField] private GameObject fullNameUpdatePanel;
        [SerializeField] private TMP_InputField firstNameInputField;
        [SerializeField] private TMP_InputField lastNameInputField;
        [SerializeField] private TextMeshProUGUI fullNameUpdateErrorText;
        [SerializeField] private Button fullNameSaveButton;

        private GameObject openScreen;

        private void Start()
        {
            emailSaveButton.onClick.AddListener(OnClickEmailSaveButton);
            displayNameSaveButton.onClick.AddListener(OnClickDisplayNameSaveButton);
            fullNameSaveButton.onClick.AddListener(OnClickFullNameSaveButton);
        }

        private void OnEnable()
        {
            UiManager.GetInstance.StopUpdatingMainMenuGameTypeDynamicDataInIntervals();
        }

        private void OnDisable()
        {
            UiManager.GetInstance?.StartUpdatingMainMenuGameTypeDynamicDataInIntervals();
        }

        public void OnClickProfileOverViewButton(int buttonIndex)
        {
            switch(buttonIndex)
            {
                case 0: break;
                case 1:
                    if(string.IsNullOrEmpty(GameVariables.UserProfile.email))
                    {
                        emailInputField.text = GameVariables.UserProfile.temp_email;
                    }
                    else
                    {
                        emailInputField.text = GameVariables.UserProfile.email;
                    }
                    emailUpdateErrorText.text = "";
                    break;
                case 2:
                    displayNameInputField.text = GameVariables.UserProfile.user_name;
                    displayNameUpdateErrorText.text = "";
                    break;
                case 3:
                    firstNameInputField.text = GameVariables.UserProfile.first_name;
                    lastNameInputField.text = GameVariables.UserProfile.last_name;
                    fullNameUpdateErrorText.text = "";
                    break;
            }
        }

        private void OnClickEmailSaveButton()
        {
            if (string.IsNullOrEmpty(emailInputField.text))
            {
                emailUpdateErrorText.text = "Please enter valid email id";
                return;
            }
            else
            {
                emailUpdateErrorText.text = "";
            }
            openScreen = emailUpdatePanel;
            UiManager.GetInstance.UpdateEmail(emailInputField.text);
        }

        private void OnClickDisplayNameSaveButton()
        {
            if (string.IsNullOrEmpty(displayNameInputField.text))
            {
                displayNameUpdateErrorText.text = "Please enter valid name";
                return;
            }
            else if(displayNameInputField.text.Length < 3)
            {
                displayNameUpdateErrorText.text = "Min characters for username is 3";
                return;
            }
            else
            {
                displayNameUpdateErrorText.text = "";
            }
            openScreen = displayNameUpdatePanel;
            UiManager.GetInstance.UpdateDisplayName(displayNameInputField.text);
        }

        private void OnClickFullNameSaveButton()
        {
            if (string.IsNullOrEmpty(firstNameInputField.text))
            {
                fullNameUpdateErrorText.text = "Please enter valid first name";
                return;
            }
            else if (string.IsNullOrEmpty(lastNameInputField.text))
            {
                fullNameUpdateErrorText.text = "Please enter valid last name";
                return;
            }
            else
            {
                fullNameUpdateErrorText.text = "";
            }
            openScreen = fullNameUpdatePanel;
            UiManager.GetInstance.UpdateFullName(firstNameInputField.text, lastNameInputField.text);
        }

        internal void UpdatePersonalDetails(string mobileNumber, bool isMobileNumberVerified, string email, bool isEmailVerified, string displayName, string firstName, string lastName)
        {
            if (mobileNumber != null)
            {
                mobileNumberText.text = mobileNumber;
                if (isMobileNumberVerified)
                {
                    mobileNumberVerificationStatusText.text = "<color=green>(Verified)</color>";
                }
                else
                {
                    mobileNumberVerificationStatusText.text = "<color=red>(Not Verified)</color>";
                }
            }

            emailText.text = email;
            emailInputField.text = email;
            if (isEmailVerified)
            {
                emailVerificationStatusText.text = "<color=green>(Verified)</color>";
            }
            else
            {
                emailVerificationStatusText.text = "<color=red>(Not Verified- Check Email)</color>";
            }

            displayNameText.text = displayName;
            displayNameInputField.text = displayName;

            fullNameText.text = firstName + " " + lastName;
            if (firstName != "-")
            {
                firstNameInputField.text = firstName;
                lastNameInputField.text = lastName;
            }
        }

        internal void GoToOverviewScreen()
        {
            userProfileOverviewPanel.SetActive(true);
            openScreen.SetActive(false);
        }
    }
}
 