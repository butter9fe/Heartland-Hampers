using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class HomePage : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(
            new GetAccountInfoRequest(),
            result =>
            {
                nameText.text = " " + (result.AccountInfo.TitleInfo.DisplayName ?? "Guest");
            },
            _ => { }
        );
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Logout()
    {
        // Unlink device ID from current account if it exists, and delete the playerprefs key
        // This is such that when the next user presses the Guest button it does not log into this account anymore
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result =>
            {
                // Check if there's a custom ID linked to this account, if there is, unlink!
                if (result.AccountInfo.CustomIdInfo != null && !string.IsNullOrEmpty(result.AccountInfo.CustomIdInfo.CustomId))
                {
                    // If there is, unlink!
                    PlayFabClientAPI.UnlinkCustomID(new UnlinkCustomIDRequest() { CustomId = SystemInfo.deviceUniqueIdentifier },
                    _ => {
                        PlayerPrefs.DeleteKey("HeartlandCustomID");
                        // Clear current session
                        PlayFabClientAPI.ForgetAllCredentials();
                        // Go back to login page
                        SceneManager.LoadScene("LoginScene");
                    },
                    error => Debug.LogError(error.ToString()));
                }
                else
                {
                    PlayFabClientAPI.ForgetAllCredentials();
                    SceneManager.LoadScene("LoginScene");
                }
            },
            error => Debug.LogError(error.ToString())
        );
    }
}
