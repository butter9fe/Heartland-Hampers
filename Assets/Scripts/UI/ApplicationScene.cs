using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


public class ApplicationScene : MonoBehaviour
{
    private RectTransform parentTransform;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private int currScreenIndex = 0;
    private Sequence transitionSequence;
    private Tween moveTween;

    private void Awake()
    {
        parentTransform = GetComponent<RectTransform>();
    }

    public void OnTransition(bool isBackwards)
    {
        transitionSequence.Complete();
        moveTween.Complete();
        currScreenIndex += isBackwards ? -1 : 1;

        // Begin animation
        moveTween = parentTransform.DOAnchorPosX(-1080 * currScreenIndex, 1.5f).SetEase(Ease.OutSine);

        transitionSequence = DOTween.Sequence();
        transitionSequence.AppendInterval(0.1f)
            .Append(canvasGroup.DOFade(0, 0.25f))
            .AppendInterval(0.5f)
            .Append(canvasGroup.DOFade(1, 0.25f));
    }

    public void SwapToHome()
    {
        SceneManager.LoadScene("HomePage");
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
