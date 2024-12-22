using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class Application_CarePackage : MonoBehaviour
{
    [SerializeField]
    private List<Application_CareItem> careItems = new List<Application_CareItem>();
    [SerializeField]
    private TMP_Text totalText;
    [SerializeField]
    private ButtonLoader submitButton;
    [SerializeField]
    private Button backButton;

    [SerializeField]
    private UnityEvent goNextStep;
    [SerializeField]
    private Color normalTotalColor, invalidTotalColor;

    private void Start()
    {
        // Assign callback to all items
        foreach (var item in careItems)
        {
            item.onCountUpdated += OnItemCountUpdated;
        }

        // Check for initial values
        CloudScriptManager.Instance.ExecGetFoodCounts(itemCounts =>
        {
            // Update current count
            for (int i = 0; i < careItems.Count; i++)
            {
                careItems[i].CurrCount = itemCounts[i];
            }
        },
        e => Debug.LogError(e.ToString()));

        OnItemCountUpdated(0);
    }

    private void OnItemCountUpdated(int newCount)
    {
        int newTotal = GetTotalCareItems();
        bool isValidSelection = newTotal == 10;
        submitButton.SetInteractable(isValidSelection);

        totalText.color = isValidSelection ? normalTotalColor : invalidTotalColor;
        totalText.text = " " + newTotal.ToString();
    }

    private int GetTotalCareItems()
    {
        int total = 0;
        foreach (Application_CareItem careItem in careItems)
        {
            total += careItem.CurrCount;
        }
        return total;
    }

    private void OnSubmissionComplete(bool success)
    {
        backButton.interactable = true;
        submitButton.SetLoading(false);

        if (success)
            goNextStep?.Invoke();
    }

    public void SubmitSelection()
    {
        List<int> counts = new List<int>();
        foreach (Application_CareItem careItem in careItems)
        {
            counts.Add(careItem.CurrCount);
        }

        // Send to server
        backButton.interactable = false;
        submitButton.SetLoading(true);
        Debug.Log(string.Join(",", counts.ToArray()));
        CloudScriptManager.Instance.ExecSubmitApplication(counts.ToArray(), _ => OnSubmissionComplete(true), _ => OnSubmissionComplete(false));
    }
}
