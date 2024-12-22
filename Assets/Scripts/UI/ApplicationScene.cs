using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
}
