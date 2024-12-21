using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class QRPopupManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lockerText;
    [SerializeField]
    private RectTransform popupObject;

    System.Action onClose = null;

    public void ShowLockerPopup(int lockerNum, System.Action onClose)
    {
        lockerText.text = "Locker #" + lockerNum.ToString();
        gameObject.SetActive(true);

        popupObject.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack);

        this.onClose = onClose;
    }

    public void OnClose()
    {
        if (onClose != null)
            onClose();

        gameObject.SetActive(false);
    }
}
