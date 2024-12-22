using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLoader : MonoBehaviour
{
    [SerializeField] GameObject loading;
    [Tooltip("Objects to hide during loading")]
    [SerializeField] List<GameObject> loadHideObjects;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        SetLoading(false);
    }

    private void OnEnable()
    {
        SetLoading(false);
    }

    public void SetLoading(bool isLoading)
    {
        if (button != null)
            button.interactable = !isLoading;

        loading.SetActive(isLoading);
        foreach (var obj in loadHideObjects)
        {
            obj.SetActive(!isLoading);
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        if (button != null)
            button.interactable = isInteractable;
    }
}
