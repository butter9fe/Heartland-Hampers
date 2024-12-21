using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;

public class SignUpManager : MonoBehaviour
{
    [SerializeField] PopupManager popupManager;
    [SerializeField] SplashManager splashManager;
    [Header("Fields")]
    [SerializeField] TMP_InputField field_fullname;
    [SerializeField] TMP_InputField field_email, field_pw, field_confirmpw, field_phone;
    [Header("Errors")]
    [SerializeField] TMP_Text error_name;
    [SerializeField] TMP_Text error_email, error_pw, error_confirmpw, error_phone;
    [Header("Buttons")]
    [SerializeField] ButtonLoader button_register;
    [SerializeField] Button button_showpw, button_showconfirmpw;

    private void OnDisable()
    {
        if (field_fullname == null)
            return;

        // Reset fields on disable
        field_fullname.text = field_email.text = field_pw.text = field_confirmpw.text = "";

        // Hide errors
        error_name.gameObject.SetActive(false);
        error_email.gameObject.SetActive(false);
        error_pw.gameObject.SetActive(false);
        error_confirmpw.gameObject.SetActive(false);
        error_phone.gameObject.SetActive(false);
    }

    private bool ValidateFields()
    {
        error_name.gameObject.SetActive(false);
        error_email.gameObject.SetActive(false);
        error_pw.gameObject.SetActive(false);
        error_confirmpw.gameObject.SetActive(false);
        error_phone.gameObject.SetActive(false);

        if (!Regex.IsMatch(field_email.text, splashManager.EmailRegex))
        {
            error_email.gameObject.SetActive(true);
            return false;
        }

        if (field_pw.text.Length < 8)
        {
            error_pw.text = "Ensure your password has at least 8 characters.";
            error_pw.gameObject.SetActive(true);
            return false;
        }

        int charaGroupsMatched =
            System.Convert.ToInt32(Regex.IsMatch(field_pw.text, "[a-z]")) +
            System.Convert.ToInt32(Regex.IsMatch(field_pw.text, "[A-Z]")) +
            System.Convert.ToInt32(Regex.IsMatch(field_pw.text, "[0-9]")) +
            System.Convert.ToInt32(Regex.IsMatch(field_pw.text, "[[!@#$%&*()_+=|<>?{}\\[\\]~-]"));

        if (charaGroupsMatched < 3)
        {
            error_pw.text = "Ensure your password contains at least 3 of the following 4 types of characters: lower case letters [a-z], upper case letters [A-Z], numbers [0-9], special characters [eg: !@#$%^&*].";
            error_pw.gameObject.SetActive(true);
            return false;
        }

        if (field_pw.text != field_confirmpw.text)
        {
            error_confirmpw.gameObject.SetActive(true);
            return false;
        }

        if (field_fullname.text.Trim().Length == 0)
        {
            error_name.gameObject.SetActive(true);
            return false;
        }

        if (!Regex.IsMatch(field_phone.text, @"[8-9]\d{7}"))
        {
            error_phone.gameObject.SetActive(true);
            return false;
        }

        return true;
    }

    /*
     * Button Events
     */
    public void OnButtonRegUser()
    {
        button_register.SetLoading(true);

        // Validation
        if (!ValidateFields())
        {
            button_register.SetLoading(false);
            return;
        }

        var regReq = new RegisterPlayFabUserRequest // for button click
        {
            Email = field_email.text,
            Password = field_pw.text,
            DisplayName = field_fullname.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSucc, e => OnError("Error registering user!", e));
    }

    public void OnButtonShowPassword()
    {
        splashManager.OnButtonShowPassword(button_showpw, field_pw);
        splashManager.OnButtonShowPassword(button_showconfirmpw, field_confirmpw);
    }

    /*
     * Responses
     */
    void OnRegSucc(RegisterPlayFabUserResult r)
    {
        popupManager.ShowMessage("Success!", "Please login with your new account.", "OK", false, () => splashManager.OnSwitchPanel("Login"), () => splashManager.OnSwitchPanel("Login"));
        CloudScriptManager.Instance.ExecBasicCoudScriptFunction(CloudScriptType.OnUserRegister, null, e => OnError("Error assigning box ID.", e));
    }

    void OnError(string errorTitle, PlayFabError e)
    {
        if (e.Error.ToString() == "UsernameNotAvailable")
        {
            popupManager.ShowMessage("Oops!", "You already have an account under this phone number.", "OK", false);
        }
        else
        {
            string errorMessage = e.ErrorMessage;

            if (e.ErrorDetails != null)
                foreach (var pair in e.ErrorDetails)
                    foreach (var msg in pair.Value)
                        errorMessage += "\n" + msg;

            popupManager.ShowMessage(errorTitle, errorMessage, "OK", false);
        }

        Debug.LogError(e.GenerateErrorReport());
        Debug.LogError("Error Type: " + e.Error.ToString());
        Debug.LogError("Error Code: " + e.Error.GetTypeCode().ToString());

        button_register.SetLoading(false);
    }
}
