using DG.Tweening;
using UnityEngine;

public class UpgradeButtonsPanel : MonoBehaviour
{
    [SerializeField] private float panelPosition = 170;
    [SerializeField] private RectTransform panel;
    void Start()
    {
        if (!MaxAdsManager.instance.IsAdsRemoved)
        {
            panel.DOAnchorPosY(panelPosition, 0.2f).SetEase(Ease.InQuart);
        }
        else
        {
            panel.DOAnchorPosY(0, 0.2f).SetEase(Ease.InQuart);
        }

        MaxAdsManager.instance.OnRemoveAdsStateChanged += OnAdsRemove;
    }

    private void OnAdsRemove()
    {
        panel.DOAnchorPosY(0, 0.2f).SetEase(Ease.InQuart);
    }
}
