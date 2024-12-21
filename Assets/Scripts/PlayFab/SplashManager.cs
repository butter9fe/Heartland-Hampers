using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashManager : MonoBehaviour
{
    [Header("Panel Switching")]
    [SerializeField] List<SplashPanel> _panels = new List<SplashPanel>();
    [SerializeField] SplashPanelType _startPanel = SplashPanelType.Login;
    //[SerializeField] GameObject loadingObject;

    [Header("Showing Passwords")]
    [SerializeField] Sprite sprite_showpw;
    [SerializeField] Sprite sprite_hidepw;

    private SplashPanel _currPanel;
    public SplashPanel CurrPanel { get => _currPanel; }

    // Email Regex derived from https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/
    const string emailRegex = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
    public string EmailRegex { get => emailRegex; }

    void Start()
    {
        SetLoading(false);

        // Set all panels except for starting panel to inactive
        foreach (var panel in _panels)
        {
            if (panel.type == _startPanel)
            {
                panel.panelObj.SetActive(true);
                _currPanel = panel;
            }
            else
            {
                panel.panelObj.SetActive(false);
            }
        }
    }

    private SplashPanel FindPanelOfType(SplashPanelType type)
    {
        foreach (var panel in _panels)
        {
            if (panel.type == type)
                return panel;
        }

        return null;
    }

    public void OnSwitchPanel(string type)
    {
        // button params do not accept enums :( have to convert
        var newPanel = FindPanelOfType((SplashPanelType)System.Enum.Parse(typeof(SplashPanelType), type));

        if (newPanel != null)
        {
            _currPanel.panelObj.SetActive(false);
            newPanel.panelObj.SetActive(true);
            _currPanel = newPanel;
        }
    }

    // Singular button, this function should be called in conjunction with other buttons in Panel scripts
    public void OnButtonShowPassword(Button button, TMP_InputField field_password)
    {
        // Toggle password visibility
        if (field_password.contentType == TMP_InputField.ContentType.Password)
        {
            field_password.contentType = TMP_InputField.ContentType.Standard;
            button.GetComponent<Image>().sprite = sprite_showpw;
        }
        else
        {
            field_password.contentType = TMP_InputField.ContentType.Password;
            button.GetComponent<Image>().sprite = sprite_hidepw;
        }

        field_password.ForceLabelUpdate();
    }

    public void SetLoading(bool bIsLoading)
    {
        // TODO: loadingObject.SetActive(bIsLoading);
    }
}

[System.Serializable]
public class SplashPanel
{
    public SplashPanelType type;
    public GameObject panelObj;
}

[System.Serializable]
public enum SplashPanelType
{
    Login,
    Register,
    ForgotPassword
}
