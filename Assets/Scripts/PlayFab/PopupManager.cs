using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PopupManager : MonoBehaviour
{
    [SerializeField] TMP_Text _popupTitle, _popupSubtitle, _popupButton;
    [SerializeField] GameObject _forgetPanel; //TODO

    System.Action onClose = null, onContinue = null;

    public void OnContinue()
    {
        if (onContinue != null)
            onContinue();
        onClose = onContinue = null;
        gameObject.SetActive(false);
        _forgetPanel.SetActive(false);
    }

    public void OnClose()
    {
        if (onClose != null)
            onClose();
        onClose = onContinue = null;
        gameObject.SetActive(false);
    }

    public void ShowMessage(
        string title = "Oops!",
        string subtitle = "You have entered the wrong email or password. Please try again.\n\nIf you have forgotten your email or password,\n please click on the links below:",
        string buttonText = "OK",
        bool showForget = true,
        System.Action closeAction = null,
        System.Action continueAction = null)
    {
        _popupTitle.text = title;
        _popupSubtitle.text = subtitle;
        _popupButton.text = buttonText;
        onClose = closeAction;
        onContinue = continueAction;

        //_forgetPanel.SetActive(showForget);

        gameObject.SetActive(true);
    }
}
