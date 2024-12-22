using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Threading;

public class LoginManager : MonoBehaviour
{
    [SerializeField] SplashManager splashManager;
    [SerializeField] PopupManager popupManager;
    [SerializeField] GameObject splashImage;
    [Header("Fields")]
    [SerializeField] TMP_InputField field_email;
    [SerializeField] TMP_InputField field_pw;
    [Header("Errors")]
    [SerializeField] TMP_Text error_email;
    [SerializeField] TMP_Text error_pw;
    [SerializeField] ButtonLoader button_login;

    private IEnumerator splashCoroutine;
    void Start()
    {
        splashCoroutine = SplashCoroutine();
        StartCoroutine(splashCoroutine);
        // Check if logged in before and autologin
        if (PlayerPrefs.HasKey("HeartlandCustomID"))
        {
            splashImage.SetActive(true);
            StopCoroutine(splashCoroutine); // Only hide splash screen after logged in
            var deviceReq = new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.staticSettings.TitleId,
                CustomId = PlayerPrefs.GetString("HeartlandCustomID")
            };

            PlayFabClientAPI.LoginWithCustomID(
                deviceReq,
                r =>
                {
                    OnPlayerLoginSucc(r);
                },
                e => OnError("Error logging into saved account!", e)
            );
        }
    }

    private void OnDisable()
    {
        // Reset fields on disable
        field_email.text = field_pw.text = "";
        error_email.gameObject.SetActive(false);
        error_pw.gameObject.SetActive(false);
    }

    /*
     * Helper Functions
     */
    void OnRememberMe()
    {
        var customID = SystemInfo.deviceUniqueIdentifier;
        PlayerPrefs.SetString("HeartlandCustomID", customID);
        var linkReq = new LinkCustomIDRequest
        {
            CustomId = customID,
            ForceLink = true
        };

        PlayFabClientAPI.LinkCustomID(linkReq, (_) => { }, e => OnError("Error linking device ID for remember me!", e));
    }

    bool ValidateFields()
    {
        error_email.gameObject.SetActive(false);
        error_pw.gameObject.SetActive(false);

        if (!Regex.IsMatch(field_email.text, splashManager.EmailRegex))
        {
            error_email.gameObject.SetActive(true);
            return false;
        }

        if (field_pw.text.Length < 8)
        {
            error_pw.gameObject.SetActive(true);
            return false;
        }

        return true;
    }

    public IEnumerator SplashCoroutine()
    {
        splashImage.SetActive(true);
        yield return new WaitForSeconds(1f);
        splashImage.SetActive(false);
    }

    /*
     * Button Events
     */
    public void OnButtonLogin()
    {
        // Disable both buttons
        button_login.SetLoading(true);

        if (!ValidateFields())
        {
            button_login.SetLoading(false);
            return;
        }

        var loginReq = new LoginWithEmailAddressRequest
        {
            Email = field_email.text,
            Password = field_pw.text,

            // Get player profile, for display name
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginReq, OnPlayerLoginSucc, e => OnError("Error logging in with email!", e));
    }

    public void OnButtonShowPassword(TMP_InputField field_pw)
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        splashManager.OnButtonShowPassword(button, field_pw);
    }

    private IEnumerator LoadGameSceneAsync(string sceneToLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
            yield return null;

        splashManager.SetLoading(false);
        splashImage.SetActive(false);
    }

    /*
     * Responses
     */
    void OnPlayerLoginSucc(LoginResult r)
    {
        OnRememberMe(); // Save login info so player doesn't need to relogin
        splashManager.SetLoading(true);
        CloudScriptManager.Instance.ExecGetHasApprovedApplication(approved =>
        {
            string sceneToLoad = approved ? "HomePage" : "ApplicationScene";
            StartCoroutine(LoadGameSceneAsync(sceneToLoad));
        },
        e =>
        {
            Debug.LogError(e.ToString());
            splashManager.SetLoading(false);
        });
    }

    void OnError(string errorTitle, PlayFabError e)
    {
        if (e.Error.ToString() == "InvalidEmailOrPassword" || e.Error.ToString() == "AccountNotFound")
        {
            popupManager.ShowMessage();
        }
        else
        {
            string errorMessage = e.ErrorMessage;

            if (e.ErrorDetails != null)
                foreach (var pair in e.ErrorDetails)
                    foreach (var msg in pair.Value)
                        errorMessage += "\n" + msg;

            popupManager.ShowMessage(errorTitle, errorMessage);
        }

        Debug.LogError(e.GenerateErrorReport());
        Debug.LogError("Error Type: " + e.Error.ToString());
        Debug.LogError("Error Code: " + e.Error.GetTypeCode().ToString());

        // Reenable both buttons
        splashImage.SetActive(false);
        button_login.SetLoading(false);
    }
}
