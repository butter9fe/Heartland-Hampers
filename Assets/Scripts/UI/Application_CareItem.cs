using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Application_CareItem : MonoBehaviour
{
    [SerializeField]
    private int maxCount = 999;
    [SerializeField]
    private Button addButton, removeButton;
    [SerializeField]
    private TMP_Text itemCountText;

    public delegate void OnCountUpdated(int newCount);
    public OnCountUpdated onCountUpdated;

    private int currCount = 0;
    public int CurrCount { get => currCount; set {
        currCount = value;
        itemCountText.text = currCount.ToString();
        onCountUpdated?.Invoke(currCount);

        // Update interactability of buttons
        addButton.interactable = currCount < maxCount;
        removeButton.interactable = currCount > 0;
    } }

    private void Awake()
    {
        CurrCount = 0;
    }

    public void AddItem()
    {
        CurrCount++;
    }

    public void RemoveItem() {  
        CurrCount--;
    }
}
